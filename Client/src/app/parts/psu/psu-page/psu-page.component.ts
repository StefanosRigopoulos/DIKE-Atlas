import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { PSU } from '../../../_model/parts';
import { Pagination } from '../../../_model/pagination';
import { PSUParams } from '../../../_model/params';
import { PSUService } from '../../../_service/psu.service';
import { FormsModule } from '@angular/forms';
import { HasRoleDirective } from '../../../_directives/has-role.directive';

@Component({
  selector: 'app-psu-page',
  standalone: true,
  imports: [CommonModule, FormsModule, HasRoleDirective],
  templateUrl: './psu-page.component.html',
  styleUrl: './psu-page.component.css'
})
export class PSUPageComponent implements OnInit {
  psus: PSU[] = [];
  pagination: Pagination | undefined;
  psuParams: PSUParams;
  filterBrand: string[] = [];
  filterModel: string[] = [];

  constructor(private psuService: PSUService, private router: Router) {
    this.psuParams = this.psuService.getPSUParams();
  }
  ngOnInit(): void {
    this.loadPSUs();
    this.getFilters();
  }
  loadPSUs(): void {
    if (this.psuParams) {
      this.psuService.setPSUParams(this.psuParams);
      this.psuService.getPagedPSUs(this.psuParams).subscribe({
        next: response => {
          if (response.result && response.pagination) {
            this.psus = response.result;
            this.pagination = response.pagination;
          } else {
            console.log("No PSUs found or empty response");
          }
        },
        error: err => {
          console.error("Error in API call:", err);
        }
      });
    }
  }
  pageChanged(event: any) {
    if (this.psuParams && this.psuParams?.pageNumber !== event.page) {
      this.psuParams.pageNumber = event.page;
      this.psuService.setPSUParams(this.psuParams);
      this.loadPSUs();
    }
  }
  applyFilters(): void {
    this.psuParams.pageNumber = 1;
    this.loadPSUs();
  }
  resetFilters(): void {
    this.psuParams = this.psuService.resetPSUParams();
    this.loadPSUs();
  }
  deletePSU(id: number): void {
    if (confirm('Είστε σίγουροι ότι θέλετε να διαγράψετε αυτό το τροφοδοτικό?')) {
      this.psuService.deletePSU(id).subscribe(() => {
        this.loadPSUs();
      });
    }
  }
  goToDetails(psu_id: number) {
    this.router.navigate(['parts/psu/', psu_id]);
  }
  goBack() {
    this.router.navigateByUrl('parts');
  }
  goToCreate() {
    this.router.navigateByUrl('parts/psu/create');
  }
  getFilters() {
    this.psuService.getFilterBrand().subscribe({
      next: response => {
        this.filterBrand = response;
      }
    });
    this.psuService.getFilterModel().subscribe({
      next: response => {
        this.filterModel = response;
      }
    });
  }
}