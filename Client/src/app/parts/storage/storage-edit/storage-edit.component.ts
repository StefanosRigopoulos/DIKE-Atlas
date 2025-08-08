import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { StorageService } from '../../../_service/storage.service';
import { StorageDevice } from '../../../_model/parts';
import { Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-storage-edit',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  templateUrl: './storage-edit.component.html',
  styleUrl: './storage-edit.component.css'
})
export class StorageEditComponent implements OnInit, OnDestroy {
  storage: StorageDevice | null = null;
  storageForm: FormGroup;
  process: boolean = true;
  private destroy$ = new Subject<void>();

  constructor(private activeRoute: ActivatedRoute,
              private router: Router,
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
    this.activeRoute.paramMap.subscribe((params) => {
      const storage_id = +params.get('storage_id')!;
      if (storage_id) {
        this.loadStorage(storage_id);
      }
    });
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
  loadStorage(storage_id: number) {
    this.storageService.getStorage(storage_id).subscribe({
      next: (storage) => {
        this.storage = storage;
        this.storageForm.patchValue(storage);
        this.storageForm.markAsPristine();
        this.process = false;
      },
      error: (err) => {
        console.error('Error getting Storage:', err);
      }
    });
  }
  goBack() {
    this.router.navigate(['parts/storage/', this.storage!.id]);
  }
  saveStorage() {
    if (this.storageForm.valid && this.storage) {
      const updatedPC = { ...this.storage, ...this.storageForm.value };
      this.storageService.updateStorage(updatedPC).subscribe({
        next: () => {
          this.storageForm.markAsPristine();
        },
        error: (err) => {
          console.error("Error updating Storage:", err);
        }
      });
    }
  }
}
