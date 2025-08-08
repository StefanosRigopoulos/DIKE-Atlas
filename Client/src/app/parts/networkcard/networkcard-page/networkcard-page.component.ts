import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { NetworkCard } from '../../../_model/parts';
import { Pagination } from '../../../_model/pagination';
import { NetworkCardParams } from '../../../_model/params';
import { NetworkCardService } from '../../../_service/networkcard.service';
import { FormsModule } from '@angular/forms';
import { HasRoleDirective } from '../../../_directives/has-role.directive';

@Component({
  selector: 'app-networkcard-page',
  standalone: true,
  imports: [CommonModule, FormsModule, HasRoleDirective],
  templateUrl: './networkcard-page.component.html',
  styleUrl: './networkcard-page.component.css'
})
export class NetworkCardPageComponent implements OnInit {
  networkcards: NetworkCard[] = [];
  pagination: Pagination | undefined;
  networkcardParams: NetworkCardParams;
  filterBrand: string[] = [];
  filterModel: string[] = [];

  constructor(private networkcardService: NetworkCardService, private router: Router) {
    this.networkcardParams = this.networkcardService.getNetworkCardParams();
  }
  ngOnInit(): void {
    this.loadNetworkCards();
    this.getFilters();
  }
  loadNetworkCards(): void {
    if (this.networkcardParams) {
      this.networkcardService.setNetworkCardParams(this.networkcardParams);
      this.networkcardService.getPagedNetworkCards(this.networkcardParams).subscribe({
        next: response => {
          if (response.result && response.pagination) {
            this.networkcards = response.result;
            this.pagination = response.pagination;
          } else {
            console.log("No NetworkCards found or empty response");
          }
        },
        error: err => {
          console.error("Error in API call:", err);
        }
      });
    }
  }
  pageChanged(event: any) {
    if (this.networkcardParams && this.networkcardParams?.pageNumber !== event.page) {
      this.networkcardParams.pageNumber = event.page;
      this.networkcardService.setNetworkCardParams(this.networkcardParams);
      this.loadNetworkCards();
    }
  }
  applyFilters(): void {
    this.networkcardParams.pageNumber = 1;
    this.loadNetworkCards();
  }
  resetFilters(): void {
    this.networkcardParams = this.networkcardService.resetNetworkCardParams();
    this.loadNetworkCards();
  }
  deleteNetworkCard(id: number): void {
    if (confirm('Είστε σίγουροι ότι θέλετε να διαγράψετε αυτήν την καρτά δικτύου?')) {
      this.networkcardService.deleteNetworkCard(id).subscribe(() => {
        this.loadNetworkCards();
      });
    }
  }
  goToDetails(networkcard_id: number) {
    this.router.navigate(['parts/networkcard/', networkcard_id]);
  }
  goBack() {
    this.router.navigateByUrl('parts');
  }
  goToCreate() {
    this.router.navigateByUrl('parts/networkcard/create');
  }
  getFilters() {
    this.networkcardService.getFilterBrand().subscribe({
      next: response => {
        this.filterBrand = response;
      }
    });
    this.networkcardService.getFilterModel().subscribe({
      next: response => {
        this.filterModel = response;
      }
    });
  }
}