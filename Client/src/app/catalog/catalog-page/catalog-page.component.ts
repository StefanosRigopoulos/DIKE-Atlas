import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { EmployeeService } from '../../_service/employee.service';
import { EmployeeOnly } from '../../_model/employee';
import { Pagination } from '../../_model/pagination';
import { EmployeeParams } from '../../_model/params';
import { HasRoleDirective } from '../../_directives/has-role.directive';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-catalog-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule, HasRoleDirective],
  templateUrl: './catalog-page.component.html',
  styleUrl: './catalog-page.component.css'
})
export class CatalogPageComponent implements OnInit {
  baseUrl = environment.apiUrl;
  employees: EmployeeOnly[] = [];
  pagination: Pagination | undefined;
  employeeParams: EmployeeParams;
  profilePhotos: { [key: number]: string } = {};
  filterUnit: string[] = [];
  filterOffice: string[] = [];
  filterRank: string[] = [];
  filterSpeciality: string[] = [];

  constructor(private employeeService: EmployeeService, private router: Router) {
    this.employeeParams = this.employeeService.getEmployeeParams();
  }
  ngOnInit(): void {
    this.loadEmployees();
    this.getFilters();
  }
  loadEmployees(): void {
    if (this.employeeParams) {
      this.employeeService.setEmployeeParams(this.employeeParams);
      this.employeeService.getPagedEmployees(this.employeeParams).subscribe({
        next: response => {
          if (response.result && response.pagination) {
            this.employees = response.result;
            this.pagination = response.pagination;
            this.employees.forEach(employee => {
              const photoPath = employee.photoPath;
              if (photoPath && photoPath.trim() != '') {
                this.profilePhotos[employee.id] = this.baseUrl.replace('/api/', '') + employee.photoPath;
              } else {
                this.profilePhotos[employee.id] = './assets/catalog/user.png';
              }
            });
          } else {
            console.log("No Employees found or empty response");
          }
        },
        error: err => {
          console.error("Error in API call:", err);
        }
      });
    }
  }
  pageChanged(event: any) {
    if (this.employeeParams && this.employeeParams?.pageNumber !== event.page) {
      this.employeeParams.pageNumber = event.page;
      this.employeeService.setEmployeeParams(this.employeeParams);
      this.loadEmployees();
    }
  }
  applyFilters(): void {
    this.employeeParams.pageNumber = 1;
    this.loadEmployees();
  }
  resetFilters(): void {
    this.employeeParams = this.employeeService.resetEmployeeParams();
    this.loadEmployees();
  }
  goToDetails(employee_id: number) {
    this.router.navigate(['catalog/', employee_id]);
  }
  goToAdd() {
    this.router.navigateByUrl('catalog/add');
  }
  goBack() {
    this.router.navigateByUrl('main');
  }
  onImageError(event: Event) {
    (event.target as HTMLImageElement).src = './assets/catalog/user.png';
  }
  getFilters() {
    this.employeeService.getFilterUnit().subscribe({
      next: response => {
        this.filterUnit = response;
      }
    });
    this.employeeService.getFilterOffice().subscribe({
      next: response => {
        this.filterOffice = response;
      }
    });
    this.employeeService.getFilterRank().subscribe({
      next: response => {
        this.filterRank = response;
      }
    });
    this.employeeService.getFilterSpeciality().subscribe({
      next: response => {
        this.filterSpeciality = response;
      }
    });
  }
}
