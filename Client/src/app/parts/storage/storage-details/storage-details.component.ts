import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { StorageService } from '../../../_service/storage.service';
import { PC } from '../../../_model/pc';
import { StorageDevice } from '../../../_model/parts';
import { HasRoleDirective } from '../../../_directives/has-role.directive';
import { Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-storage-details',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule, HasRoleDirective],
  templateUrl: './storage-details.component.html',
  styleUrl: './storage-details.component.css'
})
export class StorageDetailsComponent implements OnInit, OnDestroy {
  storage: StorageDevice | null = null;
  related_pcs: PC[] = [];
  private destroy$ = new Subject<void>();

  constructor(private activeRoute: ActivatedRoute,
              private router: Router,
              private storageService: StorageService) {}
  ngOnInit() {
    this.activeRoute.paramMap
    .pipe(takeUntil(this.destroy$))
    .subscribe(params => {
      const storage_id = +params.get('storage_id')!;
      if (storage_id) {
        this.loadStorage(storage_id);
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
      },
      error: (err) => {
        console.error('Error getting Storage:', err);
      }
    });
    this.storageService.getRelatedPCs(storage_id).subscribe({
      next: (pcs) => {
        this.related_pcs = pcs;
      },
      error: (err) => {
        console.error('Error getting related PCs:', err);
      }
    });
  }
  goBack() {
    this.router.navigateByUrl('parts/storage');
  }
  goToRelatedPC(pc_id: number) {
    this.router.navigate(['parts/pc/', pc_id]);
  }
  editStorage() {
    this.router.navigate(['parts/storage/edit/', this.storage!.id]);
  }
  deleteStorage() {
    if (confirm("Είστε σίγουροι ότι θέλετε να διαγράψετε αυτόν τον επεξεργαστή;")) {
      this.storageService.deleteStorage(this.storage!.id).subscribe({
        next: () => {
          this.router.navigate(['parts/storage']);
        },
        error: (err) => {
          console.error("Error deleting Storage:", err);
        }
      });
    }
  }
}
