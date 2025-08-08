import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { RAMService } from '../../../_service/ram.service';
import { PC } from '../../../_model/pc';
import { RAM } from '../../../_model/parts';
import { HasRoleDirective } from '../../../_directives/has-role.directive';
import { Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-ram-details',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule, HasRoleDirective],
  templateUrl: './ram-details.component.html',
  styleUrl: './ram-details.component.css'
})
export class RAMDetailsComponent implements OnInit, OnDestroy {
  ram: RAM | null = null;
  related_pcs: PC[] = [];
  private destroy$ = new Subject<void>();

  constructor(private activeRoute: ActivatedRoute,
              private router: Router,
              private ramService: RAMService) {}
  ngOnInit() {
    this.activeRoute.paramMap
    .pipe(takeUntil(this.destroy$))
    .subscribe(params => {
      const ram_id = +params.get('ram_id')!;
      if (ram_id) {
        this.loadRAM(ram_id);
      }
    });
  }
  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }
  loadRAM(ram_id: number) {
    this.ramService.getRAM(ram_id).subscribe({
      next: (ram) => {
        this.ram = ram;
      },
      error: (err) => {
        console.error('Error getting RAM:', err);
      }
    });
    this.ramService.getRelatedPCs(ram_id).subscribe({
      next: (pcs) => {
        this.related_pcs = pcs;
      },
      error: (err) => {
        console.error('Error getting related PCs:', err);
      }
    });
  }
  goBack() {
    this.router.navigateByUrl('parts/ram');
  }
  goToRelatedPC(pc_id: number) {
    this.router.navigate(['parts/pc/', pc_id]);
  }
  editRAM() {
    this.router.navigate(['parts/ram/edit/', this.ram!.id]);
  }
  deleteRAM() {
    if (confirm("Είστε σίγουροι ότι θέλετε να διαγράψετε αυτόν τον επεξεργαστή;")) {
      this.ramService.deleteRAM(this.ram!.id).subscribe({
        next: () => {
          this.router.navigate(['parts/ram']);
        },
        error: (err) => {
          console.error("Error deleting RAM:", err);
        }
      });
    }
  }
}
