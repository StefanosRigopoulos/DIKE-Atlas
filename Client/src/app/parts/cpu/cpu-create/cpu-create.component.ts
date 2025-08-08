import { Component, OnDestroy, OnInit } from '@angular/core';
import { CPU } from '../../../_model/parts';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { CPUService } from '../../../_service/cpu.service';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { Observable, Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-cpu-create',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  templateUrl: './cpu-create.component.html',
  styleUrl: './cpu-create.component.css'
})
export class CPUCreateComponent implements OnInit, OnDestroy {
  cpu: CPU | null = null;
  cpuForm: FormGroup;
  process: boolean = true;
  private destroy$ = new Subject<void>();

  constructor(private router: Router,
              private fb: FormBuilder,
              private cpuService: CPUService)
  {
    this.cpuForm = this.fb.group({
      barcode: [''],
      serialNumber: [''],
      brand: [''],
      model: [''],
      cores: ['', Validators.min(1)],
      threads: ['', Validators.min(1)],
      specification: [''],
      package: [''],
      chipset: [''],
      integratedGPUModel: ['']
    });
  }
  ngOnInit() {
    this.cpuForm.markAsPristine();
    this.process = false;
    this.cpuForm.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => {
        if (!this.process && this.cpuForm.dirty) {
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
    this.router.navigateByUrl('parts/cpu');
  }
  createCPU(): void {
    const serialValue = this.cpuForm.get('serialNumber')?.value;
    if (!serialValue) {
      this.cpuForm.patchValue({ serialNumber: "" });
    }
    const newCpu = this.cpuForm.value;
    this.cpuService.addCPU(newCpu).subscribe({
      next: () => {
        this.process = false;
        this.router.navigateByUrl('parts/cpu');
      },
      error: (err) => {
        console.error("Error creating CPU:", err);
      }
    });
  }
}
