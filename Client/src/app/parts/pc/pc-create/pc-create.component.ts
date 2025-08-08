import { Component } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { PC } from '../../../_model/pc';
import { PCService } from '../../../_service/pc.service';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-pc-create',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  templateUrl: './pc-create.component.html',
  styleUrl: './pc-create.component.css'
})
export class PCCreateComponent {
  pc: PC | null = null;
  pcForm: FormGroup;
  selectedFile: File | null = null;
  process: boolean = true;

  constructor(private router: Router,
              private fb: FormBuilder,
              private pcService: PCService)
  {
    this.pcForm = this.fb.group({
      pcName: [''],
      barcode: [''],
      serialNumber: [''],
      brand: [''],
      model: [''],
      administratorCode: [''],
      biosCode: [''],
      domain: [''],
      ip: [''],
      externalIP: [''],
      subnetMask: [''],
      gateway: [''],
      dnS1: [''],
      dnS2: ['']
    });
  }
  canDeactivate(): Observable<boolean> | Promise<boolean> | boolean {
    if (this.process) {
      return confirm('Είστε σίγουρος πως θέλετε να ακυρώσετε την δημιουργία καινούργιου Η/Υ?');
    }
    return true;
  }
  goBack() {
    this.router.navigateByUrl('parts/pc');
  }
  onFileSelected(event: Event) {
    const fileInput = event.target as HTMLInputElement;
    if (fileInput.files && fileInput.files.length > 0) {
      this.selectedFile = fileInput.files[0];
    }
  }
  addPC(): void {
    const serialValue = this.pcForm.get('serialNumber')?.value;
    if (!serialValue) {
      this.pcForm.patchValue({ serialNumber: "" });
    }
    this.pcService.addPC(this.pcForm.value, this.selectedFile).subscribe({
      next: () => {
        this.process = false;
        this.router.navigateByUrl('parts/pc');
      },
      error: (err) => {
        console.error("Error creating PC:", err);
      }
    });
  }
}
