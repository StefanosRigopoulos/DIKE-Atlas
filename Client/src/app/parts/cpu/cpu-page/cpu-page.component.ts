import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { CPU } from '../../../_model/parts';
import { Pagination } from '../../../_model/pagination';
import { CPUParams } from '../../../_model/params';
import { CPUService } from '../../../_service/cpu.service';
import { FormsModule } from '@angular/forms';
import { HasRoleDirective } from '../../../_directives/has-role.directive';

@Component({
  selector: 'app-cpu-page',
  standalone: true,
  imports: [CommonModule, FormsModule, HasRoleDirective],
  templateUrl: './cpu-page.component.html',
  styleUrl: './cpu-page.component.css'
})
export class CPUPageComponent implements OnInit {
  cpus: CPU[] = [];
  pagination: Pagination | undefined;
  cpuParams: CPUParams;
  filterBrand: string[] = [];
  filterModel: string[] = [];
  filterCore: string[] = [];

  constructor(private cpuService: CPUService, private router: Router) {
    this.cpuParams = this.cpuService.getCPUParams();
  }
  ngOnInit(): void {
    this.loadCPUs();
    this.getFilters();
  }
  loadCPUs(): void {
    if (this.cpuParams) {
      this.cpuService.setCPUParams(this.cpuParams);
      this.cpuService.getPagedCPUs(this.cpuParams).subscribe({
        next: response => {
          if (response.result && response.pagination) {
            this.cpus = response.result;
            this.pagination = response.pagination;
          } else {
            console.log("No CPUs found or empty response");
          }
        },
        error: err => {
          console.error("Error in API call:", err);
        }
      });
    }
  }
  pageChanged(event: any) {
    if (this.cpuParams && this.cpuParams?.pageNumber !== event.page) {
      this.cpuParams.pageNumber = event.page;
      this.cpuService.setCPUParams(this.cpuParams);
      this.loadCPUs();
    }
  }
  applyFilters(): void {
    this.cpuParams.pageNumber = 1;
    this.loadCPUs();
  }
  resetFilters(): void {
    this.cpuParams = this.cpuService.resetCPUParams();
    this.loadCPUs();
  }
  deleteCPU(id: number): void {
    if (confirm('Είστε σίγουροι ότι θέλετε να διαγράψετε αυτόν το επεξεργαστή?')) {
      this.cpuService.deleteCPU(id).subscribe(() => {
        this.loadCPUs();
      });
    }
  }
  goToDetails(cpu_id: number) {
    this.router.navigate(['parts/cpu/', cpu_id]);
  }
  goBack() {
    this.router.navigateByUrl('parts');
  }
  goToCreate() {
    this.router.navigateByUrl('parts/cpu/create');
  }
  getFilters() {
    this.cpuService.getFilterBrand().subscribe({
      next: response => {
        this.filterBrand = response;
      }
    });
    this.cpuService.getFilterModel().subscribe({
      next: response => {
        this.filterModel = response;
      }
    });
    this.cpuService.getFilterCore().subscribe({
      next: response => {
        this.filterCore = response;
      }
    });
  }
}