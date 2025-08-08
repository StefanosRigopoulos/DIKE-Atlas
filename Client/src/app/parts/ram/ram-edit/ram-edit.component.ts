import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { RAMService } from '../../../_service/ram.service';
import { RAM } from '../../../_model/parts';
import { Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-ram-edit',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  templateUrl: './ram-edit.component.html',
  styleUrl: './ram-edit.component.css'
})
export class RAMEditComponent implements OnInit, OnDestroy {
  ram: RAM | null = null;
  ramForm: FormGroup;
  process: boolean = true;
  private destroy$ = new Subject<void>();

  constructor(private activeRoute: ActivatedRoute,
              private router: Router,
              private fb: FormBuilder,
              private ramService: RAMService)
  {
    this.ramForm = this.fb.group({
      barcode: [''],
      serialNumber: [''],
      brand: [''],
      model: [''],
      type: [''],
      size: [''],
      frequency: [''],
      casLatency: ['']
    });
  }
  ngOnInit() {
    this.activeRoute.paramMap.subscribe((params) => {
      const ram_id = +params.get('ram_id')!;
      if (ram_id) {
        this.loadRAM(ram_id);
      }
    });
    this.ramForm.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => {
        if (!this.process && this.ramForm.dirty) {
          this.process = true;
        }
      });
  }
  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }
  loadRAM(ram_id: number) {
    this.ramService.getRAM(ram_id).subscribe({
      next: (ram) => {
        this.ram = ram;
        this.ramForm.patchValue(ram);
        this.ramForm.markAsPristine();
        this.process = false;
      },
      error: (err) => {
        console.error('Error getting RAM:', err);
      }
    });
  }
  goBack() {
    this.router.navigate(['parts/ram/', this.ram!.id]);
  }
  saveRAM() {
    if (this.ramForm.valid && this.ram) {
      const updatedPC = { ...this.ram, ...this.ramForm.value };
      this.ramService.updateRAM(updatedPC).subscribe({
        next: () => {
          this.ramForm.markAsPristine();
        },
        error: (err) => {
          console.error("Error updating RAM:", err);
        }
      });
    }
  }
}
