import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { RAM } from '../../../_model/parts';
import { Pagination } from '../../../_model/pagination';
import { RAMParams } from '../../../_model/params';
import { RAMService } from '../../../_service/ram.service';
import { FormsModule } from '@angular/forms';
import { HasRoleDirective } from '../../../_directives/has-role.directive';

@Component({
  selector: 'app-ram-page',
  standalone: true,
  imports: [CommonModule, FormsModule, HasRoleDirective],
  templateUrl: './ram-page.component.html',
  styleUrl: './ram-page.component.css'
})
export class RAMPageComponent implements OnInit {
  rams: RAM[] = [];
  pagination: Pagination | undefined;
  ramParams: RAMParams;
  filterBrand: string[] = [];
  filterModel: string[] = [];

  constructor(private ramService: RAMService, private router: Router) {
    this.ramParams = this.ramService.getRAMParams();
  }
  ngOnInit(): void {
    this.loadRAMs();
    this.getFilters();
  }
  loadRAMs(): void {
    if (this.ramParams) {
      this.ramService.setRAMParams(this.ramParams);
      this.ramService.getPagedRAMs(this.ramParams).subscribe({
        next: response => {
          if (response.result && response.pagination) {
            this.rams = response.result;
            this.pagination = response.pagination;
          } else {
            console.log("No RAMs found or empty response");
          }
        },
        error: err => {
          console.error("Error in API call:", err);
        }
      });
    }
  }
  pageChanged(event: any) {
    if (this.ramParams && this.ramParams?.pageNumber !== event.page) {
      this.ramParams.pageNumber = event.page;
      this.ramService.setRAMParams(this.ramParams);
      this.loadRAMs();
    }
  }
  applyFilters(): void {
    this.ramParams.pageNumber = 1;
    this.loadRAMs();
  }
  resetFilters(): void {
    this.ramParams = this.ramService.resetRAMParams();
    this.loadRAMs();
  }
  deleteRAM(id: number): void {
    if (confirm('Είστε σίγουροι ότι θέλετε να διαγράψετε αυτήν την μνήμη?')) {
      this.ramService.deleteRAM(id).subscribe(() => {
        this.loadRAMs();
      });
    }
  }
  goToDetails(ram_id: number) {
    this.router.navigate(['parts/ram/', ram_id]);
  }
  goBack() {
    this.router.navigateByUrl('parts');
  }
  goToCreate() {
    this.router.navigateByUrl('parts/ram/create');
  }
  getFilters() {
    this.ramService.getFilterBrand().subscribe({
      next: response => {
        this.filterBrand = response;
      }
    });
    this.ramService.getFilterModel().subscribe({
      next: response => {
        this.filterModel = response;
      }
    });
  }
}