import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { GPUService } from '../../../_service/gpu.service';
import { GPU } from '../../../_model/parts';
import { Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-gpu-edit',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  templateUrl: './gpu-edit.component.html',
  styleUrl: './gpu-edit.component.css'
})
export class GPUEditComponent implements OnInit, OnDestroy {
  gpu: GPU | null = null;
  gpuForm: FormGroup;
  process: boolean = true;
  private destroy$ = new Subject<void>();

  constructor(private activeRoute: ActivatedRoute,
              private router: Router,
              private fb: FormBuilder,
              private gpuService: GPUService)
  {
    this.gpuForm = this.fb.group({
      barcode: [''],
      serialNumber: [''],
      brand: [''],
      model: [''],
      memory: [''],
      driverVersion: ['']
    });
  }
  ngOnInit() {
    this.activeRoute.paramMap.subscribe((params) => {
      const gpu_id = +params.get('gpu_id')!;
      if (gpu_id) {
        this.loadGPU(gpu_id);
      }
    });
    this.gpuForm.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => {
        if (!this.process && this.gpuForm.dirty) {
          this.process = true;
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
        this.gpuForm.patchValue(gpu);
        this.gpuForm.markAsPristine();
        this.process = false;
      },
      error: (err) => {
        console.error('Error getting GPU:', err);
      }
    });
  }
  goBack() {
    this.router.navigate(['parts/gpu/', this.gpu!.id]);
  }
  saveGPU() {
    if (this.gpuForm.valid && this.gpu) {
      const updatedPC = { ...this.gpu, ...this.gpuForm.value };
      this.gpuService.updateGPU(updatedPC).subscribe({
        next: () => {
          this.gpuForm.markAsPristine();
        },
        error: (err) => {
          console.error("Error updating GPU:", err);
        }
      });
    }
  }
}
