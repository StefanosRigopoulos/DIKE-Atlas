import { Component, OnInit } from '@angular/core';
import { NavigationEnd, Router, RouterOutlet } from '@angular/router';
import { AccountService } from './_service/account.service';
import { NgxSpinnerComponent } from 'ngx-spinner';
import { User } from './_model/user';
import { FooterComponent } from './footer/footer.component';
import { NavbarComponent } from './navbar/navbar.component';
import { MatDialog } from '@angular/material/dialog';
import { LoginModalComponent } from './_modals/login-modal/login-modal.component';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
  standalone: true,
  imports: [NgxSpinnerComponent, NavbarComponent, RouterOutlet, FooterComponent]
})
export class AppComponent implements OnInit {
  title = 'DIKE Atlas';
  
  constructor(private accountService: AccountService, private dialog: MatDialog, private router: Router) {
    this.router.events.subscribe(event => {
      if (event instanceof NavigationEnd) {
        window.scrollTo(0, 0);
      }
    });
  }
  ngOnInit(): void {
    this.setCurrentUser();
    if (!this.accountService.currentUser()) {
      this.openLoginModal();
    }
  }
  openLoginModal() {
    const dialogRef = this.dialog.open(LoginModalComponent, {
      disableClose: true
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (!result) {
        this.openLoginModal();
      }
    });
  }
  setCurrentUser(){
    const user: User = JSON.parse(localStorage.getItem('user')!);
    this.accountService.setCurrentUser(user);
  }
}