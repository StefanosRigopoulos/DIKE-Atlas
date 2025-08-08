import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { NetworkCardService } from '../../../_service/networkcard.service';
import { NetworkCard } from '../../../_model/parts';
import { Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-networkcard-edit',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  templateUrl: './networkcard-edit.component.html',
  styleUrl: './networkcard-edit.component.css'
})
export class NetworkCardEditComponent implements OnInit, OnDestroy {
  networkcard: NetworkCard | null = null;
  networkcardForm: FormGroup;
  process: boolean = true;
  private destroy$ = new Subject<void>();

  constructor(private activeRoute: ActivatedRoute,
              private router: Router,
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
    this.activeRoute.paramMap.subscribe((params) => {
      const networkcard_id = +params.get('networkcard_id')!;
      if (networkcard_id) {
        this.loadNetworkCard(networkcard_id);
      }
    });
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
  loadNetworkCard(networkcard_id: number) {
    this.networkcardService.getNetworkCard(networkcard_id).subscribe({
      next: (networkcard) => {
        this.networkcard = networkcard;
        this.networkcardForm.patchValue(networkcard);
        this.networkcardForm.markAsPristine();
        this.process = false;
      },
      error: (err) => {
        console.error('Error getting NetworkCard:', err);
      }
    });
  }
  goBack() {
    this.router.navigate(['parts/networkcard/', this.networkcard!.id]);
  }
  saveNetworkCard() {
    if (this.networkcardForm.valid && this.networkcard) {
      const updatedPC = { ...this.networkcard, ...this.networkcardForm.value };
      this.networkcardService.updateNetworkCard(updatedPC).subscribe({
        next: () => {
          this.networkcardForm.markAsPristine();
        },
        error: (err) => {
          console.error("Error updating NetworkCard:", err);
        }
      });
    }
  }
}
