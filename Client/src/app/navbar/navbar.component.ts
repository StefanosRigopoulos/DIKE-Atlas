import { Component } from '@angular/core';
import { AccountService } from '../_service/account.service';
import { Router } from '@angular/router';
import { HasRoleDirective } from '../_directives/has-role.directive';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [HasRoleDirective],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.css'
})
export class NavbarComponent {
  constructor(public accountService: AccountService, private router: Router) { }
  
  goToAdmin() {
    this.router.navigateByUrl('/admin');
  }
  goToSearch() {
    this.router.navigateByUrl('/search');
  }
  goToCatalog() {
    this.router.navigateByUrl('/catalog');
  }
  goToParts() {
    this.router.navigateByUrl('/parts');
  }
  logout() {
    this.router.navigateByUrl('');
    this.accountService.logout();
    location.reload();
  }
}
