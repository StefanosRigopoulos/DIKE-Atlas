import { Component, OnDestroy, OnInit } from '@angular/core';
import { Monitor } from '../../../_model/parts';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { MonitorService } from '../../../_service/monitor.service';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { Observable, Subject, takeUntil } from 'rxjs';
import { PCService } from '../../../_service/pc.service';

@Component({
  selector: 'app-monitor-add',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  templateUrl: './monitor-add.component.html',
  styleUrl: './monitor-add.component.css'
})
export class MonitorAddComponent implements OnInit, OnDestroy {
  monitor: Monitor | null = null;
  monitorForm: FormGroup;
  pc_id: number = 0;
  process: boolean = true;
  private destroy$ = new Subject<void>();

  constructor(private activeRoute: ActivatedRoute,
              private router: Router,
              private fb: FormBuilder,
              private monitorService: MonitorService,
              private pcService: PCService)
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
    this.activeRoute.paramMap
    .pipe(takeUntil(this.destroy$))
    .subscribe(params => {
      const pc_id = +params.get('pc_id')!;
      if (pc_id) {
        this.pc_id = pc_id;
      }
    });
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
    this.router.navigate(['parts/pc/edit/', this.pc_id]);
  }
  createMonitor(): void {
    const serialValue = this.monitorForm.get('serialNumber')?.value;
    if (!serialValue) {
      this.monitorForm.patchValue({ serialNumber: "" });
    }
    const newCpu = this.monitorForm.value;
    this.monitorService.addMonitor(newCpu).subscribe({
      next: (newMonitor) => {
        this.assignMonitor(newMonitor.id);
      },
      error: (err) => {
        console.error("Error creating Monitor:", err);
      }
    });
  }
  private assignMonitor(monitor_id: number) {
    this.pcService.assignMonitor(this.pc_id, monitor_id).subscribe({
      next: () => {
        this.process = false;
        this.router.navigate(['parts/pc/edit/', this.pc_id]);
      },
      error: (err) => {
        console.error("Error assigning Monitor:", err);
      }
    });
  }
}
