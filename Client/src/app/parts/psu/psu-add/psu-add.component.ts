import { Component, OnDestroy, OnInit } from '@angular/core';
import { PSU } from '../../../_model/parts';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { PSUService } from '../../../_service/psu.service';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { Observable, Subject, takeUntil } from 'rxjs';
import { PCService } from '../../../_service/pc.service';

@Component({
  selector: 'app-psu-add',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  templateUrl: './psu-add.component.html',
  styleUrl: './psu-add.component.css'
})
export class PSUAddComponent implements OnInit, OnDestroy {
  psu: PSU | null = null;
  psuForm: FormGroup;
  pc_id: number = 0;
  process: boolean = true;
  private destroy$ = new Subject<void>();

  constructor(private activeRoute: ActivatedRoute,
              private router: Router,
              private fb: FormBuilder,
              private psuService: PSUService,
              private pcService: PCService)
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
    this.activeRoute.paramMap
    .pipe(takeUntil(this.destroy$))
    .subscribe(params => {
      const pc_id = +params.get('pc_id')!;
      if (pc_id) {
        this.pc_id = pc_id;
      }
    });
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
    this.router.navigate(['parts/pc/edit/', this.pc_id]);
  }
  createPSU(): void {
    const serialValue = this.psuForm.get('serialNumber')?.value;
    if (!serialValue) {
      this.psuForm.patchValue({ serialNumber: "" });
    }
    const newpsu = this.psuForm.value;
    this.psuService.addPSU(newpsu).subscribe({
      next: (newPSU) => {
        this.assignPSU(newPSU.id);
      },
      error: (err) => {
        console.error("Error creating PSU:", err);
      }
    });
  }
  private assignPSU(psu_id: number) {
    this.pcService.assignPSU(this.pc_id, psu_id).subscribe({
      next: () => {
        this.process = false;
        this.router.navigate(['parts/pc/edit/', this.pc_id]);
      },
      error: (err) => {
        console.error("Error assigning PSU:", err);
      }
    });
  }
}
