import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatDialogRef } from '@angular/material/dialog';
import { NgClass } from '@angular/common';
import { AccountService } from '../../_service/account.service';

@Component({
  selector: 'app-login-modal',
  templateUrl: './login-modal.component.html',
  styleUrls: ['./login-modal.component.css'],
  standalone: true,
  imports: [FormsModule, ReactiveFormsModule, NgClass]
})
export class LoginModalComponent implements OnInit {
  loginForm: FormGroup = new FormGroup({});
  isError = false;

  constructor(private dialogRef: MatDialogRef<LoginModalComponent>,
              public accountService: AccountService,
              private fb: FormBuilder) {}
  ngOnInit(): void {
    this.initializeForm();
  }
  initializeForm() {
    this.loginForm = this.fb.group({
      username: ['', Validators.required],
      password: ['', Validators.required]
    });
  }
  submit() {
    this.accountService.login(this.loginForm.value).subscribe({
      next: () => {
        console.log('Successful login!');
        this.dialogRef.close(true);
        window.location.reload();
      },
      error: (error) => {
        if (error.status === 401) {
          this.triggerErrorAnimation();
        }
      }
    })
  }
  triggerErrorAnimation() {
    this.isError = true;
    setTimeout(() => {
      this.isError = false;
    }, 700);
  }
}