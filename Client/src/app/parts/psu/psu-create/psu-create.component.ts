import { Component, OnDestroy, OnInit } from '@angular/core';
import { PSU } from '../../../_model/parts';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { PSUService } from '../../../_service/psu.service';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { Observable, Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-psu-create',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  templateUrl: './psu-create.component.html',
  styleUrl: './psu-create.component.css'
})
export class PSUCreateComponent implements OnInit, OnDestroy {
  psu: PSU | null = null;
  psuForm: FormGroup;
  process: boolean = true;
  private destroy$ = new Subject<void>();

  constructor(private router: Router,
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
    this.psuForm.markAsPristine();
    this.process = false;
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
  canDeactivate(): Observable<boolean> | Promise<boolean> | boolean {
    if (this.process) {
      return confirm('Είστε σίγουρος πως θέλετε να ακυρώσετε την δημιουργία καινούργιου επεξεργαστή?');
    }
    return true;
  }
  goBack() {
    this.router.navigateByUrl('parts/psu');
  }
  createPSU(): void {
    const serialValue = this.psuForm.get('serialNumber')?.value;
    if (!serialValue) {
      this.psuForm.patchValue({ serialNumber: "" });
    }
    const newpsu = this.psuForm.value;
    this.psuService.addPSU(newpsu).subscribe({
      next: () => {
        this.process = false;
        this.router.navigateByUrl('parts/psu');
      },
      error: (err) => {
        console.error("Error creating PSU:", err);
      }
    });
  }
}
