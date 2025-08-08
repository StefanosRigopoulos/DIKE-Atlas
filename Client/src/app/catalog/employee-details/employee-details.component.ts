import { Component, OnDestroy, OnInit } from '@angular/core';
import { Employee } from '../../_model/employee';
import { ActivatedRoute, Router } from '@angular/router';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { EmployeeService } from '../../_service/employee.service';
import { CommonModule } from '@angular/common';
import { PC } from '../../_model/pc';
import { HasRoleDirective } from '../../_directives/has-role.directive';
import { Subject, takeUntil } from 'rxjs';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-employee-details',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule, HasRoleDirective],
  templateUrl: './employee-details.component.html',
  styleUrl: './employee-details.component.css'
})
export class EmployeeDetailsComponent implements OnInit, OnDestroy {
  baseUrl = environment.apiUrl;
  employee: Employee | null = null;
  haveProfile: boolean = false;
  profilePhoto: string = "";
  related_pcs: PC[] = [];
  hasRelatedPCs: boolean = false;
  private destroy$ = new Subject<void>();

  constructor(private activeRoute: ActivatedRoute,
              private router: Router,
              private employeeService: EmployeeService) {}
  ngOnInit() {
    this.activeRoute.paramMap
    .pipe(takeUntil(this.destroy$))
    .subscribe(params => {
      const employee_id = +params.get('employee_id')!;
      if (employee_id) {
        this.loadEmployee(employee_id);
      }
    });
  }
  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }
  loadEmployee(employee_id: number) {
    this.employeeService.getEmployee(employee_id).subscribe({
      next: (employee) => {
        this.employee = employee;
        const photoPath = employee.photoPath;
        if (photoPath && photoPath.trim() != '') {
          this.profilePhoto = this.baseUrl.replace('/api/', '') + employee.photoPath;
        } else {
          this.profilePhoto = './assets/catalog/user.png';
        }
      },
      error: () => {
        this.profilePhoto = './assets/catalog/user.png';
      }
    });
    this.employeeService.getRelatedPCs(employee_id).subscribe({
      next: (pcs) => {
        if (pcs.length != 0) {
          this.related_pcs = pcs;
          this.hasRelatedPCs = true;
        }
      },
      error: (err) => {
        console.error('Error getting related PCs:', err);
      }
    });
  }
  onImageError(event: Event) {
    (event.target as HTMLImageElement).src = './assets/catalog/user.png';
  }
  goToPCDetails(pc_id: number) {
    this.router.navigate(['parts/pc/', pc_id]);
  }
  editProfile() {
    this.router.navigate(['catalog/edit/', this.employee!.id]);
  }
  deleteProfile() {
    if (confirm("Είστε σίγουροι ότι θέλετε να διαγράψετε αυτό το στέλεχος?")) {
      this.employeeService.deleteEmployee(this.employee!.id).subscribe({
        next: () => {
          this.router.navigateByUrl('catalog');
        },
        error: () => {
          alert("Υπήρξε πρόβλημα κατά την διαγραφή του στελέχους.");
        }
      });
    }
  }
  goBack() {
    this.router.navigateByUrl('catalog');
  }
}
