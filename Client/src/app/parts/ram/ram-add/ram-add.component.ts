import { Component, OnDestroy, OnInit } from '@angular/core';
import { RAM } from '../../../_model/parts';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { RAMService } from '../../../_service/ram.service';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { Observable, Subject, takeUntil } from 'rxjs';
import { PCService } from '../../../_service/pc.service';

@Component({
  selector: 'app-ram-add',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  templateUrl: './ram-add.component.html',
  styleUrl: './ram-add.component.css'
})
export class RAMAddComponent implements OnInit, OnDestroy {
  ram: RAM | null = null;
  ramForm: FormGroup;
  pc_id: number = 0;
  process: boolean = true;
  private destroy$ = new Subject<void>();

  constructor(private activeRoute: ActivatedRoute,
              private router: Router,
              private fb: FormBuilder,
              private ramService: RAMService,
              private pcService: PCService)
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
    this.activeRoute.paramMap
    .pipe(takeUntil(this.destroy$))
    .subscribe(params => {
      const pc_id = +params.get('pc_id')!;
      if (pc_id) {
        this.pc_id = pc_id;
      }
    });
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
    this.router.navigate(['parts/pc/edit/', this.pc_id]);
  }
  createRAM(): void {
    const serialValue = this.ramForm.get('serialNumber')?.value;
    if (!serialValue) {
      this.ramForm.patchValue({ serialNumber: "" });
    }
    const newram = this.ramForm.value;
    this.ramService.addRAM(newram).subscribe({
      next: (newRAM) => {
        this.assignRAM(newRAM.id);
      },
      error: (err) => {
        console.error("Error creating RAM:", err);
      }
    });
  }
  private assignRAM(ram_id: number) {
    this.pcService.assignRAM(this.pc_id, ram_id).subscribe({
      next: () => {
        this.process = false;
        this.router.navigate(['parts/pc/edit/', this.pc_id]);
      },
      error: (err) => {
        console.error("Error assigning RAM:", err);
      }
    });
  }
}
