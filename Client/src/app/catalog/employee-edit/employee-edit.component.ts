import { Component, ElementRef, HostListener, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { Employee } from '../../_model/employee';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { EmployeeService } from '../../_service/employee.service';
import { CommonModule } from '@angular/common';
import { PC } from '../../_model/pc';
import { Observable, Subject, takeUntil } from 'rxjs';
import { PCService } from '../../_service/pc.service';

@Component({
  selector: 'app-employee-edit',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  templateUrl: './employee-edit.component.html',
  styleUrl: './employee-edit.component.css'
})
export class EmployeeEditComponent implements OnInit, OnDestroy {
  @ViewChild('fileInput') fileInput!: ElementRef;
  employee: Employee | null = null;
  employeeForm: FormGroup;
  selectedFile: File | null = null;
  related_pcs: PC[] = [];
  hasRelatedPCs: boolean = false;
  private destroy$ = new Subject<void>();
  
  constructor(private activeRoute: ActivatedRoute,
              private router: Router,
              private employeeService: EmployeeService,
              private pcService: PCService,
              private fb: FormBuilder)
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
    if (!employee_id) return;
    this.employeeService.getEmployee(employee_id).subscribe({
      next: (employee) => {
        this.employee = employee;
        this.employeeForm.patchValue(employee);
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
  saveEmployee() {
    if (this.employeeForm.valid && this.employee) {
      const updatedEmployee = { ...this.employee, ...this.employeeForm.value };
      if (!this.selectedFile) {
        updatedEmployee.photoPath = this.employee.photoPath;
      }
      this.employeeService.updateEmployee(updatedEmployee, this.selectedFile).subscribe({
        next: () => {
          this.employeeForm.markAsPristine();
          if (this.fileInput) {
            this.fileInput.nativeElement.value = "";
          }
          this.selectedFile = null;
        },
        error: (err) => {
          console.error("Error updating Employee:", err);
        }
      });
    }
  }
  onFileSelected(event: Event) {
    const fileInput = event.target as HTMLInputElement;
    if (fileInput.files && fileInput.files.length > 0) {
      this.selectedFile = fileInput.files[0];
      this.employeeForm.markAsDirty();
    }
  }
  canDeactivate(): Observable<boolean> | Promise<boolean> | boolean {
    if (this.employeeForm.dirty || this.selectedFile) {
      return confirm('Έχετε αλλαγές που δεν είναι αποθηκευμένες. Θέλετε να φύγετε?');
    }
    return true;
  }
  goBack() {
    this.router.navigate(['catalog', this.employee!.id]);
  }

  // Relationship
  unassignedPCs: PC[] = [];
  showPCList: boolean = true;
  showPCSelection: boolean = false;
  pcSearch: string = "";
  filteredPCs: any[] = [];
  assignPC(pc_id: number) {
    if (!pc_id) return;
    this.employeeService.assignPC(this.employee!.id, pc_id).subscribe({
      next: (pc) => {
        this.showPCSelection = false;
        this.employee!.pciDs.push(pc_id);
        this.related_pcs.push(pc);
        this.pcSearch = "";
        this.filteredPCs = [];
      },
      error: (err) => {
        console.error('Error assigning PC:', err);
        this.showPCSelection = false;
      }
    });
  }
  removePC(employee_id: number, pc_id: number) {
    this.employeeService.removePC(employee_id, pc_id).subscribe({
      next: () => {
        if (this.employee) {
          this.employee.pciDs = this.employee.pciDs.filter(id => id !== pc_id);
          this.related_pcs = this.related_pcs.filter(pc => pc.id !== pc_id);
          this.loadEmployee(employee_id);
        }
      },
      error: (err) => {
        console.error("Error removing PC:", err);
      }
    });
  }
  toggleSelection() {
    this.loadAssigns();
    this.showPCSelection = !this.showPCSelection;
  }
  filtered() {
    this.filteredPCs = this.unassignedPCs.filter(cpu =>
      cpu.barcode.toLowerCase().includes(this.pcSearch.toLowerCase())
    );
  }
  private loadAssigns() {
    this.pcService.getPCs().subscribe({
      next: (pcs) => {
        this.unassignedPCs = pcs;
      }
    });
  }
  @HostListener('document:click', ['$event'])
  onClickOutside(event: Event) {
    setTimeout(() => {
      const inputField = document.querySelector('.form-control');
      const dropdown = document.querySelector('.dropdown-menu');
      if ( inputField && inputField.contains(event.target as Node) || dropdown && dropdown.contains(event.target as Node)) return;
      this.filteredPCs = [];
    }, 100);
  }
}
