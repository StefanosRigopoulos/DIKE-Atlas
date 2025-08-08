import { Component, OnDestroy, OnInit } from '@angular/core';
import { StorageDevice } from '../../../_model/parts';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { StorageService } from '../../../_service/storage.service';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { Observable, Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-storage-create',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  templateUrl: './storage-create.component.html',
  styleUrl: './storage-create.component.css'
})
export class StorageCreateComponent implements OnInit, OnDestroy {
  storage: StorageDevice | null = null;
  storageForm: FormGroup;
  process: boolean = true;
  private destroy$ = new Subject<void>();

  constructor(private router: Router,
              private fb: FormBuilder,
              private storageService: StorageService)
  {
    this.storageForm = this.fb.group({
      barcode: [''],
      serialNumber: [''],
      brand: [''],
      model: [''],
      type: [''],
      interface: [''],
      speed: [''],
      capacity: ['']
    });
  }
  ngOnInit() {
    this.storageForm.markAsPristine();
    this.process = false;
    this.storageForm.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => {
        if (!this.process && this.storageForm.dirty) {
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
    this.router.navigateByUrl('parts/storage');
  }
  createStorage(): void {
    const serialValue = this.storageForm.get('serialNumber')?.value;
    if (!serialValue) {
      this.storageForm.patchValue({ serialNumber: "" });
    }
    const newstorage = this.storageForm.value;
    this.storageService.addStorage(newstorage).subscribe({
      next: () => {
        this.process = false;
        this.router.navigateByUrl('parts/storage');
      },
      error: (err) => {
        console.error("Error creating Storage:", err);
      }
    });
  }
}
