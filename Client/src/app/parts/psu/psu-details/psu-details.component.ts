import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { PSUService } from '../../../_service/psu.service';
import { PC } from '../../../_model/pc';
import { PSU } from '../../../_model/parts';
import { HasRoleDirective } from '../../../_directives/has-role.directive';
import { Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-psu-details',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule, HasRoleDirective],
  templateUrl: './psu-details.component.html',
  styleUrl: './psu-details.component.css'
})
export class PSUDetailsComponent implements OnInit, OnDestroy {
  psu: PSU | null = null;
  related_pcs: PC[] = [];
  private destroy$ = new Subject<void>();

  constructor(private activeRoute: ActivatedRoute,
              private router: Router,
              private psuService: PSUService) {}
  ngOnInit() {
    this.activeRoute.paramMap
    .pipe(takeUntil(this.destroy$))
    .subscribe(params => {
      const psu_id = +params.get('psu_id')!;
      if (psu_id) {
        this.loadPSU(psu_id);
      }
    });
  }
  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }
  loadPSU(psu_id: number) {
    this.psuService.getPSU(psu_id).subscribe({
      next: (psu) => {
        this.psu = psu;
      },
      error: (err) => {
        console.error('Error getting PSU:', err);
      }
    });
    this.psuService.getRelatedPCs(psu_id).subscribe({
      next: (pcs) => {
        this.related_pcs = pcs;
      },
      error: (err) => {
        console.error('Error getting related PCs:', err);
      }
    });
  }
  goBack() {
    this.router.navigateByUrl('parts/psu');
  }
  goToRelatedPC(pc_id: number) {
    this.router.navigate(['parts/pc/', pc_id]);
  }
  editPSU() {
    this.router.navigate(['parts/psu/edit/', this.psu!.id]);
  }
  deletePSU() {
    if (confirm("Είστε σίγουροι ότι θέλετε να διαγράψετε αυτόν τον επεξεργαστή;")) {
      this.psuService.deletePSU(this.psu!.id).subscribe({
        next: () => {
          this.router.navigate(['parts/psu']);
        },
        error: (err) => {
          console.error("Error deleting PSU:", err);
        }
      });
    }
  }
}
