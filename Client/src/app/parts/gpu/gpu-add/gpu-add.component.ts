import { Component, OnDestroy, OnInit } from '@angular/core';
import { GPU } from '../../../_model/parts';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { GPUService } from '../../../_service/gpu.service';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { Observable, Subject, takeUntil } from 'rxjs';
import { PCService } from '../../../_service/pc.service';

@Component({
  selector: 'app-gpu-add',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  templateUrl: './gpu-add.component.html',
  styleUrl: './gpu-add.component.css'
})
export class GPUAddComponent implements OnInit, OnDestroy {
  gpu: GPU | null = null;
  gpuForm: FormGroup;
  pc_id: number = 0;
  process: boolean = true;
  private destroy$ = new Subject<void>();

  constructor(private activeRoute: ActivatedRoute,
              private router: Router,
              private fb: FormBuilder,
              private gpuService: GPUService,
              private pcService: PCService)
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
    this.activeRoute.paramMap
    .pipe(takeUntil(this.destroy$))
    .subscribe(params => {
      const pc_id = +params.get('pc_id')!;
      if (pc_id) {
        this.pc_id = pc_id;
      }
    });
    this.gpuForm.markAsPristine();
    this.process = false;
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
  canDeactivate(): Observable<boolean> | Promise<boolean> | boolean {
    if (this.process) {
      return confirm('Είστε σίγουρος πως θέλετε να ακυρώσετε την δημιουργία καινούργιου επεξεργαστή?');
    }
    return true;
  }
  goBack() {
    this.router.navigate(['parts/pc/edit/', this.pc_id]);
  }
  createGPU(): void {
    const serialValue = this.gpuForm.get('serialNumber')?.value;
    if (!serialValue) {
      this.gpuForm.patchValue({ serialNumber: "" });
    }
    const newCpu = this.gpuForm.value;
    this.gpuService.addGPU(newCpu).subscribe({
      next: (newGPU) => {
        this.assignGPU(newGPU.id);
      },
      error: (err) => {
        console.error("Error creating GPU:", err);
      }
    });
  }
  private assignGPU(gpu_id: number) {
    this.pcService.assignGPU(this.pc_id, gpu_id).subscribe({
      next: () => {
        this.process = false;
        this.router.navigate(['parts/pc/edit/', this.pc_id]);
      },
      error: (err) => {
        console.error("Error assigning GPU:", err);
      }
    });
  }
}
