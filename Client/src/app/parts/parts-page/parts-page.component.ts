import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-parts-page',
  standalone: true,
  imports: [],
  templateUrl: './parts-page.component.html',
  styleUrl: './parts-page.component.css'
})
export class PartsPageComponent {
  constructor(private router: Router) {}
  
  navigateToPC() {
    this.router.navigate(['/parts/pc']);
  }
  navigateToCPU() {
    this.router.navigate(['/parts/cpu']);
  }
  navigateToMOBO() {
    this.router.navigate(['/parts/mobo']);
  }
  navigateToRAM() {
    this.router.navigate(['/parts/ram']);
  }
  navigateToGPU() {
    this.router.navigate(['/parts/gpu']);
  }
  navigateToPSU() {
    this.router.navigate(['/parts/psu']);
  }
  navigateToStorage() {
    this.router.navigate(['/parts/storage']);
  }
  navigateToNetworkCard() {
    this.router.navigate(['/parts/networkcard']);
  }
  navigateToMonitor() {
    this.router.navigate(['/parts/monitor']);
  }
}
