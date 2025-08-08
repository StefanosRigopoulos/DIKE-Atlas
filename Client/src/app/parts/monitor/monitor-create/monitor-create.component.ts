import { Component, OnDestroy, OnInit } from '@angular/core';
import { Monitor } from '../../../_model/parts';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { MonitorService } from '../../../_service/monitor.service';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { Observable, Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-monitor-create',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  templateUrl: './monitor-create.component.html',
  styleUrl: './monitor-create.component.css'
})
export class MonitorCreateComponent implements OnInit, OnDestroy {
  monitor: Monitor | null = null;
  monitorForm: FormGroup;
  process: boolean = true;
  private destroy$ = new Subject<void>();

  constructor(private router: Router,
              private fb: FormBuilder,
              private monitorService: MonitorService)
  {
    this.monitorForm = this.fb.group({
      barcode: [''],
      serialNumber: [''],
      brand: [''],
      model: [''],
      resolution: [''],
      inches: ['', Validators.min(14)]
    });
  }
  ngOnInit() {
    this.monitorForm.markAsPristine();
    this.process = false;
    this.monitorForm.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => {
        if (!this.process && this.monitorForm.dirty) {
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
    this.router.navigateByUrl('parts/monitor');
  }
  createMonitor(): void {
    const serialValue = this.monitorForm.get('serialNumber')?.value;
    if (!serialValue) {
      this.monitorForm.patchValue({ serialNumber: "" });
    }
    const newCpu = this.monitorForm.value;
    this.monitorService.addMonitor(newCpu).subscribe({
      next: () => {
        this.process = false;
        this.router.navigateByUrl('parts/monitor');
      },
      error: (err) => {
        console.error("Error creating Monitor:", err);
      }
    });
  }
}
