import { Component, OnDestroy, OnInit } from '@angular/core';
import { NetworkCard } from '../../../_model/parts';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { NetworkCardService } from '../../../_service/networkcard.service';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { Observable, Subject, takeUntil } from 'rxjs';
import { PCService } from '../../../_service/pc.service';

@Component({
  selector: 'app-networkcard-add',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  templateUrl: './networkcard-add.component.html',
  styleUrl: './networkcard-add.component.css'
})
export class NetworkCardAddComponent implements OnInit, OnDestroy {
  networkcard: NetworkCard | null = null;
  networkcardForm: FormGroup;
  pc_id: number = 0;
  process: boolean = true;
  private destroy$ = new Subject<void>();

  constructor(private activeRoute: ActivatedRoute,
              private router: Router,
              private fb: FormBuilder,
              private networkcardService: NetworkCardService,
              private pcService: PCService)
  {
    this.networkcardForm = this.fb.group({
      barcode: [''],
      serialNumber: [''],
      brand: [''],
      model: ['']
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
    this.router.navigate(['parts/pc/edit/', this.pc_id]);
  }
  createNetworkCard(): void {
    const serialValue = this.networkcardForm.get('serialNumber')?.value;
    if (!serialValue) {
      this.networkcardForm.patchValue({ serialNumber: "" });
    }
    const newnetworkcard = this.networkcardForm.value;
    this.networkcardService.addNetworkCard(newnetworkcard).subscribe({
      next: (newNetworkCard) => {
        this.assignNetworkCard(newNetworkCard.id);
      },
      error: (err) => {
        console.error("Error creating NetworkCard:", err);
      }
    });
  }
  private assignNetworkCard(networkcard_id: number) {
    this.pcService.assignNetworkCard(this.pc_id, networkcard_id).subscribe({
      next: () => {
        this.process = false;
        this.router.navigate(['parts/pc/edit/', this.pc_id]);
      },
      error: (err) => {
        console.error("Error assigning NetworkCard:", err);
      }
    });
  }
}
