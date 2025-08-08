import { Component, OnDestroy, OnInit } from '@angular/core';
import { RAM } from '../../../_model/parts';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { RAMService } from '../../../_service/ram.service';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { Observable, Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-ram-create',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  templateUrl: './ram-create.component.html',
  styleUrl: './ram-create.component.css'
})
export class RAMCreateComponent implements OnInit, OnDestroy {
  ram: RAM | null = null;
  ramForm: FormGroup;
  process: boolean = true;
  private destroy$ = new Subject<void>();

  constructor(private router: Router,
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
    this.ramForm.markAsPristine();
    this.process = false;
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
  canDeactivate(): Observable<boolean> | Promise<boolean> | boolean {
    if (this.process) {
      return confirm('Είστε σίγουρος πως θέλετε να ακυρώσετε την δημιουργία καινούργιου επεξεργαστή?');
    }
    return true;
  }
  goBack() {
    this.router.navigateByUrl('parts/ram');
  }
  createRAM(): void {
    const serialValue = this.ramForm.get('serialNumber')?.value;
    if (!serialValue) {
      this.ramForm.patchValue({ serialNumber: "" });
    }
    const newram = this.ramForm.value;
    this.ramService.addRAM(newram).subscribe({
      next: () => {
        this.process = false;
        this.router.navigateByUrl('parts/ram');
      },
      error: (err) => {
        console.error("Error creating RAM:", err);
      }
    });
  }
}
