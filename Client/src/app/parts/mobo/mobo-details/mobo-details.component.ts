import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { MOBOService } from '../../../_service/mobo.service';
import { PC } from '../../../_model/pc';
import { MOBO } from '../../../_model/parts';
import { HasRoleDirective } from '../../../_directives/has-role.directive';
import { Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-mobo-details',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule, HasRoleDirective],
  templateUrl: './mobo-details.component.html',
  styleUrl: './mobo-details.component.css'
})
export class MOBODetailsComponent implements OnInit, OnDestroy {
  mobo: MOBO | null = null;
  related_pcs: PC[] = [];
  private destroy$ = new Subject<void>();

  constructor(private activeRoute: ActivatedRoute,
              private router: Router,
              private moboService: MOBOService) {}
  ngOnInit() {
    this.activeRoute.paramMap
    .pipe(takeUntil(this.destroy$))
    .subscribe(params => {
      const mobo_id = +params.get('mobo_id')!;
      if (mobo_id) {
        this.loadMOBO(mobo_id);
      }
    });
  }
  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }
  loadMOBO(mobo_id: number) {
    this.moboService.getMOBO(mobo_id).subscribe({
      next: (mobo) => {
        this.mobo = mobo;
      },
      error: (err) => {
        console.error('Error getting MOBO:', err);
      }
    });
    this.moboService.getRelatedPCs(mobo_id).subscribe({
      next: (pcs) => {
        this.related_pcs = pcs;
      },
      error: (err) => {
        console.error('Error getting related PCs:', err);
      }
    });
  }
  goBack() {
    this.router.navigateByUrl('parts/mobo');
  }
  goToRelatedPC(pc_id: number) {
    this.router.navigate(['parts/pc/', pc_id]);
  }
  editMOBO() {
    this.router.navigate(['parts/mobo/edit/', this.mobo!.id]);
  }
  deleteMOBO() {
    if (confirm("Είστε σίγουροι ότι θέλετε να διαγράψετε αυτόν τον επεξεργαστή;")) {
      this.moboService.deleteMOBO(this.mobo!.id).subscribe({
        next: () => {
          this.router.navigate(['parts/mobo']);
        },
        error: (err) => {
          console.error("Error deleting MOBO:", err);
        }
      });
    }
  }
}
