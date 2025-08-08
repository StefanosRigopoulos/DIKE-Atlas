import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-main',
  standalone: true,
  imports: [],
  templateUrl: './main.component.html',
  styleUrl: './main.component.css'
})
export class MainComponent {

  constructor(private router: Router) {}

  navigateToSearch() {
    this.router.navigate(['/search']);
  }
  navigateToCatalog() {
    this.router.navigate(['/catalog']);
  }
  navigateToParts() {
    this.router.navigate(['/parts']);
  }
}
