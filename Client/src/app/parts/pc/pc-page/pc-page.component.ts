import { Component, OnInit } from '@angular/core';
import { PCParams } from '../../../_model/params';
import { PCService } from '../../../_service/pc.service';
import { Pagination } from '../../../_model/pagination';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { PCOnly } from '../../../_model/pc';
import { FileService } from '../../../_service/file.service';
import { Router } from '@angular/router';
import { HasRoleDirective } from '../../../_directives/has-role.directive';

@Component({
  selector: 'app-pc-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule, HasRoleDirective],
  templateUrl: './pc-page.component.html',
  styleUrl: './pc-page.component.css'
})
export class PCPageComponent implements OnInit {
  pcs: PCOnly[] = [];
  pagination: Pagination | undefined;
  pcParams: PCParams;
  generatedReport: Blob | null = null;
  filterBrand: string[] = [];
  filterModel: string[] = [];
  filterDomain: string[] = [];

  constructor(private pcService: PCService,
              private fileService: FileService,
              private router: Router)
  {
    this.pcParams = this.pcService.getPCParams();
  }
  ngOnInit(): void {
    this.loadPCs();
    this.getFilters();
  }
  loadPCs(): void {
    if (this.pcParams) {
      this.pcService.setPCParams(this.pcParams);
      this.pcService.getPagedPCs(this.pcParams).subscribe({
        next: response => {
          if (response.result && response.pagination) {
            this.pcs = response.result;
            this.pagination = response.pagination;
          } else {
            console.log("No PCs found or empty response");
          }
        },
        error: err => {
          console.error("Error in API call:", err);
        }
      });
    }
  }
  pageChanged(event: any) {
    if (this.pcParams && this.pcParams?.pageNumber !== event.page) {
      this.pcParams.pageNumber = event.page;
      this.pcService.setPCParams(this.pcParams);
      this.loadPCs();
    }
  }
  applyFilters(): void {
    this.pcParams.pageNumber = 1;
    this.loadPCs();
  }
  resetFilters(): void {
    this.pcParams = this.pcService.resetPCParams();
    this.loadPCs();
  }
  deletePC(pc_id: number): void {
    if (confirm('Είστε σίγουροι ότι θέλετε να διαγράψετε αυτόν το Η/Υ?')) {
      this.pcService.deletePC(pc_id).subscribe(() => {
        this.loadPCs();
      });
    }
  }
  getPCReport(pc_id: number) {
    this.fileService.getPCReport(pc_id).subscribe({
      next: (blob: Blob) => {
        const url = window.URL.createObjectURL(blob);
        window.open(url, "_blank");
      },
      error: () => {
        console.log("Error processing the report!");
      }
    });
  }
  goToDetails(pc_id: number) {
    this.router.navigate(['parts/pc/', pc_id]);
  }
  goBack() {
    this.router.navigateByUrl('parts');
  }
  goToCreate() {
    this.router.navigateByUrl('parts/pc/create');
  }
  getFilters() {
    this.pcService.getFilterBrand().subscribe({
      next: response => {
        this.filterBrand = response;
      }
    });
    this.pcService.getFilterModel().subscribe({
      next: response => {
        this.filterModel = response;
      }
    });
    this.pcService.getFilterDomain().subscribe({
      next: response => {
        this.filterDomain = response;
      }
    });
  }
}