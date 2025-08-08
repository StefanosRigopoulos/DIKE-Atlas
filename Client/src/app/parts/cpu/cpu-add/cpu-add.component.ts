import { Component, OnDestroy, OnInit } from '@angular/core';
import { CPU } from '../../../_model/parts';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { CPUService } from '../../../_service/cpu.service';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { Observable, Subject, takeUntil } from 'rxjs';
import { PCService } from '../../../_service/pc.service';

@Component({
  selector: 'app-cpu-add',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  templateUrl: './cpu-add.component.html',
  styleUrl: './cpu-add.component.css'
})
export class CPUAddComponent implements OnInit, OnDestroy {
  cpu: CPU | null = null;
  cpuForm: FormGroup;
  pc_id: number = 0;
  process: boolean = true;
  private destroy$ = new Subject<void>();

  constructor(private activeRoute: ActivatedRoute,
              private router: Router,
              private fb: FormBuilder,
              private cpuService: CPUService,
              private pcService: PCService)
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
    this.activeRoute.paramMap
    .pipe(takeUntil(this.destroy$))
    .subscribe(params => {
      const pc_id = +params.get('pc_id')!;
      if (pc_id) {
        this.pc_id = pc_id;
      }
    });
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
    this.router.navigate(['parts/pc/edit/', this.pc_id]);
  }
  createCPU(): void {
    const serialValue = this.cpuForm.get('serialNumber')?.value;
    if (!serialValue) {
      this.cpuForm.patchValue({ serialNumber: "" });
    }
    const newCpu = this.cpuForm.value;
    this.cpuService.addCPU(newCpu).subscribe({
      next: (newCPU) => {
        this.assignCPU(newCPU.id);
      },
      error: (err) => {
        console.error("Error creating CPU:", err);
      }
    });
  }
  private assignCPU(cpu_id: number) {
    this.pcService.assignCPU(this.pc_id, cpu_id).subscribe({
      next: () => {
        this.process = false;
        this.router.navigate(['parts/pc/edit/', this.pc_id]);
      },
      error: (err) => {
        console.error("Error assigning CPU:", err);
      }
    });
  }
}
