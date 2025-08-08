import { Component, OnDestroy, OnInit } from '@angular/core';
import { GPU } from '../../../_model/parts';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { GPUService } from '../../../_service/gpu.service';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { Observable, Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-gpu-create',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  templateUrl: './gpu-create.component.html',
  styleUrl: './gpu-create.component.css'
})
export class GPUCreateComponent implements OnInit, OnDestroy {
  gpu: GPU | null = null;
  gpuForm: FormGroup;
  process: boolean = true;
  private destroy$ = new Subject<void>();

  constructor(private router: Router,
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
    this.router.navigateByUrl('parts/gpu');
  }
  createGPU(): void {
    const serialValue = this.gpuForm.get('serialNumber')?.value;
    if (!serialValue) {
      this.gpuForm.patchValue({ serialNumber: "" });
    }
    const newCpu = this.gpuForm.value;
    this.gpuService.addGPU(newCpu).subscribe({
      next: () => {
        this.process = false;
        this.router.navigateByUrl('parts/gpu');
      },
      error: (err) => {
        console.error("Error creating GPU:", err);
      }
    });
  }
}
