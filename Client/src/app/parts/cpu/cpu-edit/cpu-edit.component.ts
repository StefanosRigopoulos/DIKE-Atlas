import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { CPUService } from '../../../_service/cpu.service';
import { CPU } from '../../../_model/parts';
import { Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-cpu-edit',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  templateUrl: './cpu-edit.component.html',
  styleUrl: './cpu-edit.component.css'
})
export class CPUEditComponent implements OnInit, OnDestroy {
  cpu: CPU | null = null;
  cpuForm: FormGroup;
  process: boolean = true;
  private destroy$ = new Subject<void>();

  constructor(private activeRoute: ActivatedRoute,
              private router: Router,
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
    this.activeRoute.paramMap.subscribe((params) => {
      const cpu_id = +params.get('cpu_id')!;
      if (cpu_id) {
        this.loadCPU(cpu_id);
      }
    });
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
  loadCPU(cpu_id: number) {
    this.cpuService.getCPU(cpu_id).subscribe({
      next: (cpu) => {
        this.cpu = cpu;
        this.cpuForm.patchValue(cpu);
        this.cpuForm.markAsPristine();
        this.process = false;
      },
      error: (err) => {
        console.error('Error getting CPU:', err);
      }
    });
  }
  goBack() {
    this.router.navigate(['parts/cpu/', this.cpu!.id]);
  }
  saveCPU() {
    if (this.cpuForm.valid && this.cpu) {
      const updatedPC = { ...this.cpu, ...this.cpuForm.value };
      this.cpuService.updateCPU(updatedPC).subscribe({
        next: () => {
          this.cpuForm.markAsPristine();
        },
        error: (err) => {
          console.error("Error updating CPU:", err);
        }
      });
    }
  }
}
