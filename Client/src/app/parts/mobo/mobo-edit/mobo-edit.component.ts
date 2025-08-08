import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { MOBOService } from '../../../_service/mobo.service';
import { MOBO } from '../../../_model/parts';
import { Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-mobo-edit',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  templateUrl: './mobo-edit.component.html',
  styleUrl: './mobo-edit.component.css'
})
export class MOBOEditComponent implements OnInit, OnDestroy {
  mobo: MOBO | null = null;
  moboForm: FormGroup;
  process: boolean = true;
  private destroy$ = new Subject<void>();

  constructor(private activeRoute: ActivatedRoute,
              private router: Router,
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
    this.activeRoute.paramMap.subscribe((params) => {
      const mobo_id = +params.get('mobo_id')!;
      if (mobo_id) {
        this.loadMOBO(mobo_id);
      }
    });
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
  loadMOBO(mobo_id: number) {
    this.moboService.getMOBO(mobo_id).subscribe({
      next: (mobo) => {
        this.mobo = mobo;
        this.moboForm.patchValue(mobo);
        this.moboForm.markAsPristine();
        this.process = false;
      },
      error: (err) => {
        console.error('Error getting MOBO:', err);
      }
    });
  }
  goBack() {
    this.router.navigate(['parts/mobo/', this.mobo!.id]);
  }
  saveMOBO() {
    if (this.moboForm.valid && this.mobo) {
      const updatedPC = { ...this.mobo, ...this.moboForm.value };
      this.moboService.updateMOBO(updatedPC).subscribe({
        next: () => {
          this.moboForm.markAsPristine();
        },
        error: (err) => {
          console.error("Error updating MOBO:", err);
        }
      });
    }
  }
}
