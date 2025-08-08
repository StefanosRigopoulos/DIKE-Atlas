import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { PSUService } from '../../../_service/psu.service';
import { PSU } from '../../../_model/parts';
import { Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-psu-edit',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  templateUrl: './psu-edit.component.html',
  styleUrl: './psu-edit.component.css'
})
export class PSUEditComponent implements OnInit, OnDestroy {
  psu: PSU | null = null;
  psuForm: FormGroup;
  process: boolean = true;
  private destroy$ = new Subject<void>();

  constructor(private activeRoute: ActivatedRoute,
              private router: Router,
              private fb: FormBuilder,
              private psuService: PSUService)
  {
    this.psuForm = this.fb.group({
      barcode: [''],
      serialNumber: [''],
      brand: [''],
      model: [''],
      wattage: ['', Validators.min(200)],
      certification: ['']
    });
  }
  ngOnInit() {
    this.activeRoute.paramMap.subscribe((params) => {
      const psu_id = +params.get('psu_id')!;
      if (psu_id) {
        this.loadPSU(psu_id);
      }
    });
    this.psuForm.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => {
        if (!this.process && this.psuForm.dirty) {
          this.process = true;
        }
      });
  }
  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }
  loadPSU(psu_id: number) {
    this.psuService.getPSU(psu_id).subscribe({
      next: (psu) => {
        this.psu = psu;
        this.psuForm.patchValue(psu);
        this.psuForm.markAsPristine();
        this.process = false;
      },
      error: (err) => {
        console.error('Error getting PSU:', err);
      }
    });
  }
  goBack() {
    this.router.navigate(['parts/psu/', this.psu!.id]);
  }
  savePSU() {
    if (this.psuForm.valid && this.psu) {
      const updatedPC = { ...this.psu, ...this.psuForm.value };
      this.psuService.updatePSU(updatedPC).subscribe({
        next: () => {
          this.psuForm.markAsPristine();
        },
        error: (err) => {
          console.error("Error updating PSU:", err);
        }
      });
    }
  }
}
