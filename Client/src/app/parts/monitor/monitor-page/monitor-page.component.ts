import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { Monitor } from '../../../_model/parts';
import { Pagination } from '../../../_model/pagination';
import { MonitorParams } from '../../../_model/params';
import { MonitorService } from '../../../_service/monitor.service';
import { FormsModule } from '@angular/forms';
import { HasRoleDirective } from '../../../_directives/has-role.directive';

@Component({
  selector: 'app-monitor-page',
  standalone: true,
  imports: [CommonModule, FormsModule, HasRoleDirective],
  templateUrl: './monitor-page.component.html',
  styleUrl: './monitor-page.component.css'
})
export class MonitorPageComponent implements OnInit {
  monitors: Monitor[] = [];
  pagination: Pagination | undefined;
  monitorParams: MonitorParams;
  filterBrand: string[] = [];
  filterModel: string[] = [];

  constructor(private monitorService: MonitorService, private router: Router) {
    this.monitorParams = this.monitorService.getMonitorParams();
  }
  ngOnInit(): void {
    this.loadMonitors();
    this.getFilters();
  }
  loadMonitors(): void {
    if (this.monitorParams) {
      this.monitorService.setMonitorParams(this.monitorParams);
      this.monitorService.getPagedMonitors(this.monitorParams).subscribe({
        next: response => {
          if (response.result && response.pagination) {
            this.monitors = response.result;
            this.pagination = response.pagination;
          } else {
            console.log("No Monitors found or empty response");
          }
        },
        error: err => {
          console.error("Error in API call:", err);
        }
      });
    }
  }
  pageChanged(event: any) {
    if (this.monitorParams && this.monitorParams?.pageNumber !== event.page) {
      this.monitorParams.pageNumber = event.page;
      this.monitorService.setMonitorParams(this.monitorParams);
      this.loadMonitors();
    }
  }
  applyFilters(): void {
    this.monitorParams.pageNumber = 1;
    this.loadMonitors();
  }
  resetFilters(): void {
    this.monitorParams = this.monitorService.resetMonitorParams();
    this.loadMonitors();
  }
  deleteMonitor(id: number): void {
    if (confirm('Είστε σίγουροι ότι θέλετε να διαγράψετε αυτήν την οθόνη?')) {
      this.monitorService.deleteMonitor(id).subscribe(() => {
        this.loadMonitors();
      });
    }
  }
  goToDetails(monitor_id: number) {
    this.router.navigate(['parts/monitor/', monitor_id]);
  }
  goBack() {
    this.router.navigateByUrl('parts');
  }
  goToCreate() {
    this.router.navigateByUrl('parts/monitor/create');
  }
  getFilters() {
    this.monitorService.getFilterBrand().subscribe({
      next: response => {
        this.filterBrand = response;
      }
    });
    this.monitorService.getFilterModel().subscribe({
      next: response => {
        this.filterModel = response;
      }
    });
  }
}