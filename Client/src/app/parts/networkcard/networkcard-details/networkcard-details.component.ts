import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { NetworkCardService } from '../../../_service/networkcard.service';
import { PC } from '../../../_model/pc';
import { NetworkCard } from '../../../_model/parts';
import { HasRoleDirective } from '../../../_directives/has-role.directive';
import { Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-networkcard-details',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule, HasRoleDirective],
  templateUrl: './networkcard-details.component.html',
  styleUrl: './networkcard-details.component.css'
})
export class NetworkCardDetailsComponent implements OnInit, OnDestroy {
  networkcard: NetworkCard | null = null;
  related_pcs: PC[] = [];
  private destroy$ = new Subject<void>();

  constructor(private activeRoute: ActivatedRoute,
              private router: Router,
              private networkcardService: NetworkCardService) {}
  ngOnInit() {
    this.activeRoute.paramMap
    .pipe(takeUntil(this.destroy$))
    .subscribe(params => {
      const networkcard_id = +params.get('networkcard_id')!;
      if (networkcard_id) {
        this.loadNetworkCard(networkcard_id);
      }
    });
  }
  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }
  loadNetworkCard(networkcard_id: number) {
    this.networkcardService.getNetworkCard(networkcard_id).subscribe({
      next: (networkcard) => {
        this.networkcard = networkcard;
      },
      error: (err) => {
        console.error('Error getting NetworkCard:', err);
      }
    });
    this.networkcardService.getRelatedPCs(networkcard_id).subscribe({
      next: (pcs) => {
        this.related_pcs = pcs;
      },
      error: (err) => {
        console.error('Error getting related PCs:', err);
      }
    });
  }
  goBack() {
    this.router.navigateByUrl('parts/networkcard');
  }
  goToRelatedPC(pc_id: number) {
    this.router.navigate(['parts/pc/', pc_id]);
  }
  editNetworkCard() {
    this.router.navigate(['parts/networkcard/edit/', this.networkcard!.id]);
  }
  deleteNetworkCard() {
    if (confirm("Είστε σίγουροι ότι θέλετε να διαγράψετε αυτόν τον επεξεργαστή;")) {
      this.networkcardService.deleteNetworkCard(this.networkcard!.id).subscribe({
        next: () => {
          this.router.navigate(['parts/networkcard']);
        },
        error: (err) => {
          console.error("Error deleting NetworkCard:", err);
        }
      });
    }
  }
}
