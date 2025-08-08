import { Component } from '@angular/core';
import { SearchResult } from '../_model/search';
import { SearchService } from '../_service/search.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

@Component({
  selector: 'app-search',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './search.component.html',
  styleUrl: './search.component.css'
})
export class SearchComponent {
  searchTerm = '';
  foundNothing: boolean = false;
  employeeResults: SearchResult[] = [];
  pcResults: SearchResult[] = [];
  cpuResults: SearchResult[] = [];
  moboResults: SearchResult[] = [];
  ramResults: SearchResult[] = [];
  gpuResults: SearchResult[] = [];
  psuResults: SearchResult[] = [];
  storageResults: SearchResult[] = [];
  networkcardResults: SearchResult[] = [];
  monitorResults: SearchResult[] = [];

  constructor(private searchService: SearchService,
              private router: Router) {}
  onSearch() {
    this.searchService.search(this.searchTerm).subscribe({
      next: (result) => {
        this.employeeResults = [];
        this.pcResults = [];
        this.cpuResults = [];
        this.moboResults = [];
        this.ramResults = [];
        this.gpuResults = [];
        this.psuResults = [];
        this.storageResults = [];
        this.networkcardResults = [];
        this.monitorResults = [];
        this.foundNothing = false;
        if (result.length > 0) {
          this.employeeResults = result.filter(r => r.type === 'Employee');
          this.pcResults = result.filter(r => r.type === 'PC');
          this.cpuResults = result.filter(r => r.type === 'CPU');
          this.moboResults = result.filter(r => r.type === 'MOBO');
          this.ramResults = result.filter(r => r.type === 'RAM');
          this.gpuResults = result.filter(r => r.type === 'GPU');
          this.psuResults = result.filter(r => r.type === 'PSU');
          this.storageResults = result.filter(r => r.type === 'Storage');
          this.networkcardResults = result.filter(r => r.type === 'NetworkCard');
          this.monitorResults = result.filter(r => r.type === 'Monitor');
          this.foundNothing = false;
        } else {
          this.foundNothing = true;
        }
      }
    });
  }
  goToDetails(result: SearchResult) {
    switch (result.type.toLowerCase()) {
      case 'employee': this.router.navigate(['catalog/', result.id]); break;
      case 'pc': this.router.navigate(['/parts/pc/', result.id]); break;
      case 'cpu': this.router.navigate(['/parts/cpu/', result.id]); break;
      case 'mobo': this.router.navigate(['/parts/mobo/', result.id]); break;
      case 'ram': this.router.navigate(['/parts/ram/', result.id]); break;
      case 'gpu': this.router.navigate(['/parts/gpu/', result.id]); break;
      case 'psu': this.router.navigate(['/parts/psu/', result.id]); break;
      case 'storage': this.router.navigate(['/parts/storage/', result.id]); break;
      case 'networkcard': this.router.navigate(['/parts/networkcard/', result.id]); break;
      case 'monitor': this.router.navigate(['/parts/monitor/', result.id]); break;
    }
  }
  toGreek(result: SearchResult) {
    switch (result.type.toLowerCase()) {
      case 'employee': return 'Στέλεχος';
      case 'pc': return 'Ηλεκτρονικός Υπολογιστής';
      case 'cpu': return 'Επεξεργαστής';
      case 'mobo': return 'Μητρική Κάρτα';
      case 'ram': return 'Μνήμη';
      case 'gpu': return 'Κάρτα Γραφικών';
      case 'psu': return 'Τροφοδοτικό';
      case 'storage': return 'Αποθηκευτικό Μέσο';
      case 'networkcard': return 'Κάρτα Δικτύου';
      case 'monitor': return 'Οθόνη';
      default: return 'Άγνωστο';
    }
  }
}
