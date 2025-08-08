import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { MonitorService } from '../../../_service/monitor.service';
import { Monitor } from '../../../_model/parts';
import { Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-monitor-edit',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  templateUrl: './monitor-edit.component.html',
  styleUrl: './monitor-edit.component.css'
})
export class MonitorEditComponent implements OnInit, OnDestroy {
  monitor: Monitor | null = null;
  monitorForm: FormGroup;
  process: boolean = true;
  private destroy$ = new Subject<void>();

  constructor(private activeRoute: ActivatedRoute,
              private router: Router,
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
    this.activeRoute.paramMap.subscribe((params) => {
      const monitor_id = +params.get('monitor_id')!;
      if (monitor_id) {
        this.loadMonitor(monitor_id);
      }
    });
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
  loadMonitor(monitor_id: number) {
    this.monitorService.getMonitor(monitor_id).subscribe({
      next: (monitor) => {
        this.monitor = monitor;
        this.monitorForm.patchValue(monitor);
        this.monitorForm.markAsPristine();
        this.process = false;
      },
      error: (err) => {
        console.error('Error getting Monitor:', err);
      }
    });
  }
  goBack() {
    this.router.navigate(['parts/monitor/', this.monitor!.id]);
  }
  saveMonitor() {
    if (this.monitorForm.valid && this.monitor) {
      const updatedPC = { ...this.monitor, ...this.monitorForm.value };
      this.monitorService.updateMonitor(updatedPC).subscribe({
        next: () => {
          this.monitorForm.markAsPristine();
        },
        error: (err) => {
          console.error("Error updating Monitor:", err);
        }
      });
    }
  }
}
