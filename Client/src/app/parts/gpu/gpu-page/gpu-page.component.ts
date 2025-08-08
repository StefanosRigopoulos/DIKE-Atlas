import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { GPU } from '../../../_model/parts';
import { Pagination } from '../../../_model/pagination';
import { GPUParams } from '../../../_model/params';
import { GPUService } from '../../../_service/gpu.service';
import { FormsModule } from '@angular/forms';
import { HasRoleDirective } from '../../../_directives/has-role.directive';

@Component({
  selector: 'app-gpu-page',
  standalone: true,
  imports: [CommonModule, FormsModule, HasRoleDirective],
  templateUrl: './gpu-page.component.html',
  styleUrl: './gpu-page.component.css'
})
export class GPUPageComponent implements OnInit {
  gpus: GPU[] = [];
  pagination: Pagination | undefined;
  gpuParams: GPUParams;
  filterBrand: string[] = [];
  filterModel: string[] = [];

  constructor(private gpuService: GPUService, private router: Router) {
    this.gpuParams = this.gpuService.getGPUParams();
  }
  ngOnInit(): void {
    this.loadGPUs();
    this.getFilters();
  }
  loadGPUs(): void {
    if (this.gpuParams) {
      this.gpuService.setGPUParams(this.gpuParams);
      this.gpuService.getPagedGPUs(this.gpuParams).subscribe({
        next: response => {
          if (response.result && response.pagination) {
            this.gpus = response.result;
            this.pagination = response.pagination;
          } else {
            console.log("No GPUs found or empty response");
          }
        },
        error: err => {
          console.error("Error in API call:", err);
        }
      });
    }
  }
  pageChanged(event: any) {
    if (this.gpuParams && this.gpuParams?.pageNumber !== event.page) {
      this.gpuParams.pageNumber = event.page;
      this.gpuService.setGPUParams(this.gpuParams);
      this.loadGPUs();
    }
  }
  applyFilters(): void {
    this.gpuParams.pageNumber = 1;
    this.loadGPUs();
  }
  resetFilters(): void {
    this.gpuParams = this.gpuService.resetGPUParams();
    this.loadGPUs();
  }
  deleteGPU(id: number): void {
    if (confirm('Είστε σίγουροι ότι θέλετε να διαγράψετε αυτήν την κάρτα γραφικών?')) {
      this.gpuService.deleteGPU(id).subscribe(() => {
        this.loadGPUs();
      });
    }
  }
  goToDetails(gpu_id: number) {
    this.router.navigate(['parts/gpu/', gpu_id]);
  }
  goBack() {
    this.router.navigateByUrl('parts');
  }
  goToCreate() {
    this.router.navigateByUrl('parts/gpu/create');
  }
  getFilters() {
    this.gpuService.getFilterBrand().subscribe({
      next: response => {
        this.filterBrand = response;
      }
    });
    this.gpuService.getFilterModel().subscribe({
      next: response => {
        this.filterModel = response;
      }
    });
  }
}