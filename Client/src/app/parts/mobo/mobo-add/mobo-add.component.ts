import { Component, OnDestroy, OnInit } from '@angular/core';
import { MOBO } from '../../../_model/parts';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { MOBOService } from '../../../_service/mobo.service';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { Observable, Subject, takeUntil } from 'rxjs';
import { PCService } from '../../../_service/pc.service';

@Component({
  selector: 'app-mobo-add',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  templateUrl: './mobo-add.component.html',
  styleUrl: './mobo-add.component.css'
})
export class MOBOAddComponent implements OnInit, OnDestroy {
  mobo: MOBO | null = null;
  moboForm: FormGroup;
  pc_id: number = 0;
  process: boolean = true;
  private destroy$ = new Subject<void>();

  constructor(private activeRoute: ActivatedRoute,
              private router: Router,
              private fb: FormBuilder,
              private moboService: MOBOService,
              private pcService: PCService)
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
    this.activeRoute.paramMap
    .pipe(takeUntil(this.destroy$))
    .subscribe(params => {
      const pc_id = +params.get('pc_id')!;
      if (pc_id) {
        this.pc_id = pc_id;
      }
    });
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
    this.router.navigate(['parts/pc/edit/', this.pc_id]);
  }
  createMOBO(): void {
    const serialValue = this.moboForm.get('serialNumber')?.value;
    if (!serialValue) {
      this.moboForm.patchValue({ serialNumber: "" });
    }
    const newCpu = this.moboForm.value;
    this.moboService.addMOBO(newCpu).subscribe({
      next: (newMOBO) => {
        this.assignMOBO(newMOBO.id);
      },
      error: (err) => {
        console.error("Error creating MOBO:", err);
      }
    });
  }
  private assignMOBO(mobo_id: number) {
    this.pcService.assignMOBO(this.pc_id, mobo_id).subscribe({
      next: () => {
        this.process = false;
        this.router.navigate(['parts/pc/edit/', this.pc_id]);
      },
      error: (err) => {
        console.error("Error assigning MOBO:", err);
      }
    });
  }
}
