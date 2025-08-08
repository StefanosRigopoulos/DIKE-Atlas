import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { MOBO } from '../../../_model/parts';
import { Pagination } from '../../../_model/pagination';
import { MOBOParams } from '../../../_model/params';
import { MOBOService } from '../../../_service/mobo.service';
import { FormsModule } from '@angular/forms';
import { HasRoleDirective } from '../../../_directives/has-role.directive';

@Component({
  selector: 'app-mobo-page',
  standalone: true,
  imports: [CommonModule, FormsModule, HasRoleDirective],
  templateUrl: './mobo-page.component.html',
  styleUrl: './mobo-page.component.css'
})
export class MOBOPageComponent implements OnInit {
  mobos: MOBO[] = [];
  pagination: Pagination | undefined;
  moboParams: MOBOParams;
  filterBrand: string[] = [];
  filterModel: string[] = [];

  constructor(private moboService: MOBOService, private router: Router) {
    this.moboParams = this.moboService.getMOBOParams();
  }
  ngOnInit(): void {
    this.loadMOBOs();
    this.getFilters();
  }
  loadMOBOs(): void {
    if (this.moboParams) {
      this.moboService.setMOBOParams(this.moboParams);
      this.moboService.getPagedMOBOs(this.moboParams).subscribe({
        next: response => {
          if (response.result && response.pagination) {
            this.mobos = response.result;
            this.pagination = response.pagination;
          } else {
            console.log("No MOBOs found or empty response");
          }
        },
        error: err => {
          console.error("Error in API call:", err);
        }
      });
    }
  }
  pageChanged(event: any) {
    if (this.moboParams && this.moboParams?.pageNumber !== event.page) {
      this.moboParams.pageNumber = event.page;
      this.moboService.setMOBOParams(this.moboParams);
      this.loadMOBOs();
    }
  }
  applyFilters(): void {
    this.moboParams.pageNumber = 1;
    this.loadMOBOs();
  }
  resetFilters(): void {
    this.moboParams = this.moboService.resetMOBOParams();
    this.loadMOBOs();
  }
  deleteMOBO(id: number): void {
    if (confirm('Είστε σίγουροι ότι θέλετε να διαγράψετε αυτήν την μητρική καρτα?')) {
      this.moboService.deleteMOBO(id).subscribe(() => {
        this.loadMOBOs();
      });
    }
  }
  goToDetails(mobo_id: number) {
    this.router.navigate(['parts/mobo/', mobo_id]);
  }
  goBack() {
    this.router.navigateByUrl('parts');
  }
  goToCreate() {
    this.router.navigateByUrl('parts/mobo/create');
  }
  getFilters() {
    this.moboService.getFilterBrand().subscribe({
      next: response => {
        this.filterBrand = response;
      }
    });
    this.moboService.getFilterModel().subscribe({
      next: response => {
        this.filterModel = response;
      }
    });
  }
}