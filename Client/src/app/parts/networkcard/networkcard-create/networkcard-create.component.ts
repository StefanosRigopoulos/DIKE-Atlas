import { Component, OnDestroy, OnInit } from '@angular/core';
import { NetworkCard } from '../../../_model/parts';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { NetworkCardService } from '../../../_service/networkcard.service';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { Observable, Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-networkcard-create',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  templateUrl: './networkcard-create.component.html',
  styleUrl: './networkcard-create.component.css'
})
export class NetworkCardCreateComponent implements OnInit, OnDestroy {
  networkcard: NetworkCard | null = null;
  networkcardForm: FormGroup;
  process: boolean = true;
  private destroy$ = new Subject<void>();

  constructor(private router: Router,
              private fb: FormBuilder,
              private networkcardService: NetworkCardService)
  {
    this.networkcardForm = this.fb.group({
      barcode: [''],
      serialNumber: [''],
      brand: [''],
      model: ['']
    });
  }
  ngOnInit() {
    this.networkcardForm.markAsPristine();
    this.process = false;
    this.networkcardForm.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => {
        if (!this.process && this.networkcardForm.dirty) {
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
    this.router.navigateByUrl('parts/networkcard');
  }
  createNetworkCard(): void {
    const serialValue = this.networkcardForm.get('serialNumber')?.value;
    if (!serialValue) {
      this.networkcardForm.patchValue({ serialNumber: "" });
    }
    const newnetworkcard = this.networkcardForm.value;
    this.networkcardService.addNetworkCard(newnetworkcard).subscribe({
      next: () => {
        this.process = false;
        this.router.navigateByUrl('parts/networkcard');
      },
      error: (err) => {
        console.error("Error creating NetworkCard:", err);
      }
    });
  }
}
