import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { StorageDevice } from '../../../_model/parts';
import { Pagination } from '../../../_model/pagination';
import { StorageParams } from '../../../_model/params';
import { StorageService } from '../../../_service/storage.service';
import { FormsModule } from '@angular/forms';
import { HasRoleDirective } from '../../../_directives/has-role.directive';

@Component({
  selector: 'app-storage-page',
  standalone: true,
  imports: [CommonModule, FormsModule, HasRoleDirective],
  templateUrl: './storage-page.component.html',
  styleUrl: './storage-page.component.css'
})
export class StoragePageComponent implements OnInit {
  storages: StorageDevice[] = [];
  pagination: Pagination | undefined;
  storageParams: StorageParams;
  filterBrand: string[] = [];
  filterModel: string[] = [];
  filterType: string[] = [];
  filterInterface: string[] = [];

  constructor(private storageService: StorageService, private router: Router) {
    this.storageParams = this.storageService.getStorageParams();
  }
  ngOnInit(): void {
    this.loadStorages();
    this.getFilters();
  }
  loadStorages(): void {
    if (this.storageParams) {
      this.storageService.setStorageParams(this.storageParams);
      this.storageService.getPagedStorages(this.storageParams).subscribe({
        next: response => {
          if (response.result && response.pagination) {
            this.storages = response.result;
            this.pagination = response.pagination;
          } else {
            console.log("No Storages found or empty response");
          }
        },
        error: err => {
          console.error("Error in API call:", err);
        }
      });
    }
  }
  pageChanged(event: any) {
    if (this.storageParams && this.storageParams?.pageNumber !== event.page) {
      this.storageParams.pageNumber = event.page;
      this.storageService.setStorageParams(this.storageParams);
      this.loadStorages();
    }
  }
  applyFilters(): void {
    this.storageParams.pageNumber = 1;
    this.loadStorages();
  }
  resetFilters(): void {
    this.storageParams = this.storageService.resetStorageParams();
    this.loadStorages();
  }
  deleteStorage(id: number): void {
    if (confirm('Είστε σίγουροι ότι θέλετε να διαγράψετε αυτό το αποθηκευτικό μέσο?')) {
      this.storageService.deleteStorage(id).subscribe(() => {
        this.loadStorages();
      });
    }
  }
  goToDetails(storage_id: number) {
    this.router.navigate(['parts/storage/', storage_id]);
  }
  goBack() {
    this.router.navigateByUrl('parts');
  }
  goToCreate() {
    this.router.navigateByUrl('parts/storage/create');
  }
  getFilters() {
    this.storageService.getFilterBrand().subscribe({
      next: response => {
        this.filterBrand = response;
      }
    });
    this.storageService.getFilterModel().subscribe({
      next: response => {
        this.filterModel = response;
      }
    });
    this.storageService.getFilterType().subscribe({
      next: response => {
        this.filterType = response;
      }
    });
    this.storageService.getFilterInterface().subscribe({
      next: response => {
        this.filterInterface = response;
      }
    });
  }
}