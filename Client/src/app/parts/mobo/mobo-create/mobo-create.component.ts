import { Component, OnDestroy, OnInit } from '@angular/core';
import { MOBO } from '../../../_model/parts';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { MOBOService } from '../../../_service/mobo.service';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { Observable, Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-mobo-create',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  templateUrl: './mobo-create.component.html',
  styleUrl: './mobo-create.component.css'
})
export class MOBOCreateComponent implements OnInit, OnDestroy {
  mobo: MOBO | null = null;
  moboForm: FormGroup;
  process: boolean = true;
  private destroy$ = new Subject<void>();

  constructor(private router: Router,
              private fb: FormBuilder,
              private moboService: MOBOService)
  {
    this.moboForm = this.fb.group({
      barcode: [''],
      serialNumber: [''],
      brand: [''],
      model: [''],
      dramSlots: ['', Validators.min(1)],
      chipsetVendor: [''],
      chipsetModel: [''],
      biosBrand: [''],
      biosVersion: [''],
      onBoardGPUBrand: [''],
      onBoardGPUModel: [''],
      onBoardNetworkAdapterBrand: [''],
      onBoardNetworkAdapterModel: ['']
    });
  }
  ngOnInit() {
    this.moboForm.markAsPristine();
    this.process = false;
    this.moboForm.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => {
        if (!this.process && this.moboForm.dirty) {
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
    this.router.navigateByUrl('parts/mobo');
  }
  createMOBO(): void {
    const serialValue = this.moboForm.get('serialNumber')?.value;
    if (!serialValue) {
      this.moboForm.patchValue({ serialNumber: "" });
    }
    const newCpu = this.moboForm.value;
    this.moboService.addMOBO(newCpu).subscribe({
      next: () => {
        this.process = false;
        this.router.navigateByUrl('parts/mobo');
      },
      error: (err) => {
        console.error("Error creating MOBO:", err);
      }
    });
  }
}
