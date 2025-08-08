import { Component, ElementRef, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { User } from '../_model/user';
import { CommonModule } from '@angular/common';
import { AdminService } from '../_service/admin.service';

@Component({
  selector: 'app-admin',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './admin.component.html',
  styleUrl: './admin.component.css'
})
export class AdminComponent {
  activeTab: string = 'users';

  constructor(private fb: FormBuilder,
              private adminService: AdminService)
  {
    this.registerForm = this.fb.group({
      id: [null],
      userName: ['', Validators.required],
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      password: ['', [Validators.required, Validators.min(6), Validators.max(12)]],
      role: ['User', Validators.required]
    });
    this.updateForm = this.fb.group({
      id: [null],
      userName: [''],
      firstName: [''],
      lastName: [''],
      newPassword: ['', [Validators.min(6), Validators.max(12)]],
      role: ['User', Validators.required]
    });
  }
  ngOnInit(): void {
    this.loadUsers();
  }
  setActiveTab(tab: string) {
    this.activeTab = tab;
  }

  // User Management
  users: User[] = [];
  registerForm: FormGroup;
  updateForm: FormGroup;
  selectedUser: User | null = null;
  updateTab: boolean = false;
  registerTab: boolean = false;
  loadUsers(): void {
    this.adminService.getUsers().subscribe(users => {
      this.users = users;
    });
  }
  registerUser(): void {
    if (this.registerForm.valid) {
      this.adminService.registerUser(this.registerForm.value, this.registerForm.get('password')!.value).subscribe(() => {
        this.loadUsers();
        this.toggleRegister();
      });
    }
  }
  updateUser(): void {
    if (this.updateForm.valid) {
      this.adminService.updateUser(this.updateForm.value, this.updateForm.get('newPassword')?.value).subscribe(() => {
        this.loadUsers();
        this.selectedUser = null;
      });
    }
  }
  deleteUser(id: number): void {
    if (confirm('Είστε σίγουροι πως θέλετε να διαγράψετε αυτόν το χρήστη?')) {
      this.adminService.deleteUser(id).subscribe(() => {
        this.loadUsers();
        this.selectedUser = null;
      });
    }
  }
  selectUser(user: User): void {
    if (this.registerTab) {
      this.registerTab = false;
      this.updateForm.reset();
      this.registerForm.reset();
    }
    this.selectedUser = user;
    this.updateForm.patchValue(user);
  }
  toggleRegister() {
    if (this.registerTab) {
      this.registerTab = false;
      this.registerForm.reset();
    } else {
      if (this.selectedUser) {
        this.selectedUser = null;
        this.updateForm.reset();
        this.registerForm.reset();
      }
      this.registerTab = true;
    }
  }

  // Database Management
  getDatabase() {
    this.adminService.getDatabase().subscribe({
      next: (blob: Blob) => {
        const fileName = 'Database.xlsx';
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = fileName;
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
        window.URL.revokeObjectURL(url);
      },
      error: () => {
        console.log("Error processing the request!");
      }
    });
  }
  getTable(tableName: string) {
    this.adminService.getTable(tableName).subscribe({
      next: (blob: Blob) => {
        const fileName = `${tableName}.xlsx`;
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = fileName;
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
        window.URL.revokeObjectURL(url);
      },
      error: () => {
        console.log("Error processing the request!");
      }
    });
  }

  // Backup
  @ViewChild('importInput') importInput!: ElementRef;
  importedFile: File | null = null;
  exportDatabase() {
    this.adminService.exportDatabase().subscribe({
      next: (blob: Blob) => {
        const fileName = 'db_backup.json';
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = fileName;
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
        window.URL.revokeObjectURL(url);
      },
      error: () => {
        console.log("Error processing the request!");
      }
    });
  }
  importDatabase() {
    if (this.importedFile) {
      this.adminService.importDatabase(this.importedFile);
      this.importInput.nativeElement.value = "";
    }
  }
  onImportSelected(event: Event) {
    const fileInput = event.target as HTMLInputElement;
    if (fileInput.files && fileInput.files.length > 0) {
      this.importedFile = fileInput.files[0];
    }
  }

  // Miscellaneous
  @ViewChild('backgroundInput') backgroundInput!: ElementRef;
  backgroundFile: File | null = null;
  uploadBackgroundImage() {
    if (this.backgroundFile) {
      this.adminService.uploadBackgroundImage(this.backgroundFile);
      this.backgroundInput.nativeElement.value = "";
    }
  }
  onBackgroundSelected(event: Event) {
    const fileInput = event.target as HTMLInputElement;
    if (fileInput.files && fileInput.files.length > 0) {
      this.backgroundFile = fileInput.files[0];
    }
  }
}