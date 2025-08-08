import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { EmployeeService } from '../../_service/employee.service';
import { Observable, Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-employee-add',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  templateUrl: './employee-add.component.html',
  styleUrl: './employee-add.component.css'
})
export class EmployeeAddComponent implements OnInit, OnDestroy {
  employeeForm: FormGroup;
  selectedFile: File | null = null;
  process: boolean = true;
  private destroy$ = new Subject<void>();

  constructor(private router: Router,
              private fb: FormBuilder,
              private employeeService: EmployeeService)
  {
    this.employeeForm = this.fb.group({
      am: [''],
      rank: [''],
      speciality: [''],
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      unit: ['', Validators.required],
      office: ['', Validators.required],
      position: [''],
      pcUsername: [''],
      shdaedUsername: [''],
      phone: [''],
      mobile: [''],
      email: ['', Validators.email],
      photoPath: ['']
    });
  }
  ngOnInit() {
    this.employeeForm.markAsPristine();
    this.process = false;
    this.employeeForm.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => {
        if (!this.process && this.employeeForm.dirty) {
          this.process = true;
        }
      });
  }
  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }
  addEmployee() {
    if (this.employeeForm.valid) {
      this.employeeService.addEmployee(this.employeeForm.value, this.selectedFile).subscribe({
        next: () => {
          this.process = false;
          this.router.navigateByUrl('catalog');
        },
        error: (error) => {
          console.error("Error creating Employee:", error);
        }
      });
    }
  }
  onFileSelected(event: Event) {
    const fileInput = event.target as HTMLInputElement;
    if (fileInput.files && fileInput.files.length > 0) {
      this.selectedFile = fileInput.files[0];
    }
  }
  canDeactivate(): Observable<boolean> | Promise<boolean> | boolean {
    if (this.process) {
      return confirm('Do you really want to leave the ongoing process?');
    }
    return true;
  }
  goBack() {
    this.router.navigateByUrl('catalog');
  }
}
