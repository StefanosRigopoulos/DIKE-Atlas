import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { GPUService } from '../../../_service/gpu.service';
import { PC } from '../../../_model/pc';
import { GPU } from '../../../_model/parts';
import { HasRoleDirective } from '../../../_directives/has-role.directive';
import { Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-gpu-details',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule, HasRoleDirective],
  templateUrl: './gpu-details.component.html',
  styleUrl: './gpu-details.component.css'
})
export class GPUDetailsComponent implements OnInit, OnDestroy {
  gpu: GPU | null = null;
  related_pcs: PC[] = [];
  private destroy$ = new Subject<void>();

  constructor(private activeRoute: ActivatedRoute,
              private router: Router,
              private gpuService: GPUService) {}
  ngOnInit() {
    this.activeRoute.paramMap
    .pipe(takeUntil(this.destroy$))
    .subscribe(params => {
      const gpu_id = +params.get('gpu_id')!;
      if (gpu_id) {
        this.loadGPU(gpu_id);
      }
    });
  }
  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }
  loadGPU(gpu_id: number) {
    this.gpuService.getGPU(gpu_id).subscribe({
      next: (gpu) => {
        this.gpu = gpu;
      },
      error: (err) => {
        console.error('Error getting GPU:', err);
      }
    });
    this.gpuService.getRelatedPCs(gpu_id).subscribe({
      next: (pcs) => {
        this.related_pcs = pcs;
      },
      error: (err) => {
        console.error('Error getting related PCs:', err);
      }
    });
  }
  goBack() {
    this.router.navigateByUrl('parts/gpu');
  }
  goToRelatedPC(pc_id: number) {
    this.router.navigate(['parts/pc/', pc_id]);
  }
  editGPU() {
    this.router.navigate(['parts/gpu/edit/', this.gpu!.id]);
  }
  deleteGPU() {
    if (confirm("Είστε σίγουροι ότι θέλετε να διαγράψετε αυτόν τον επεξεργαστή;")) {
      this.gpuService.deleteGPU(this.gpu!.id).subscribe({
        next: () => {
          this.router.navigate(['parts/gpu']);
        },
        error: (err) => {
          console.error("Error deleting GPU:", err);
        }
      });
    }
  }
}
