import { Component, OnDestroy, OnInit } from '@angular/core';
import { StorageDevice } from '../../../_model/parts';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { StorageService } from '../../../_service/storage.service';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { Observable, Subject, takeUntil } from 'rxjs';
import { PCService } from '../../../_service/pc.service';

@Component({
  selector: 'app-storage-add',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  templateUrl: './storage-add.component.html',
  styleUrl: './storage-add.component.css'
})
export class StorageAddComponent implements OnInit, OnDestroy {
  storage: StorageDevice | null = null;
  storageForm: FormGroup;
  pc_id: number = 0;
  process: boolean = true;
  private destroy$ = new Subject<void>();

  constructor(private activeRoute: ActivatedRoute,
              private router: Router,
              private fb: FormBuilder,
              private storageService: StorageService,
              private pcService: PCService)
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
    this.activeRoute.paramMap
    .pipe(takeUntil(this.destroy$))
    .subscribe(params => {
      const pc_id = +params.get('pc_id')!;
      if (pc_id) {
        this.pc_id = pc_id;
      }
    });
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
    this.router.navigate(['parts/pc/edit/', this.pc_id]);
  }
  createStorage(): void {
    const serialValue = this.storageForm.get('serialNumber')?.value;
    if (!serialValue) {
      this.storageForm.patchValue({ serialNumber: "" });
    }
    const newstorage = this.storageForm.value;
    this.storageService.addStorage(newstorage).subscribe({
      next: (newStorage) => {
        this.assignStorage(newStorage.id);
      },
      error: (err) => {
        console.error("Error creating Storage:", err);
      }
    });
  }
  private assignStorage(storage_id: number) {
    this.pcService.assignStorage(this.pc_id, storage_id).subscribe({
      next: () => {
        this.process = false;
        this.router.navigate(['parts/pc/edit/', this.pc_id]);
      },
      error: (err) => {
        console.error("Error assigning Storage:", err);
      }
    });
  }
}
