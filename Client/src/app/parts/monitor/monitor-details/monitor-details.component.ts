import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { MonitorService } from '../../../_service/monitor.service';
import { PC } from '../../../_model/pc';
import { Monitor } from '../../../_model/parts';
import { HasRoleDirective } from '../../../_directives/has-role.directive';
import { Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-monitor-details',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule, HasRoleDirective],
  templateUrl: './monitor-details.component.html',
  styleUrl: './monitor-details.component.css'
})
export class MonitorDetailsComponent implements OnInit, OnDestroy {
  monitor: Monitor | null = null;
  related_pcs: PC[] = [];
  private destroy$ = new Subject<void>();

  constructor(private activeRoute: ActivatedRoute,
              private router: Router,
              private monitorService: MonitorService) {}
  ngOnInit() {
    this.activeRoute.paramMap
    .pipe(takeUntil(this.destroy$))
    .subscribe(params => {
      const monitor_id = +params.get('monitor_id')!;
      if (monitor_id) {
        this.loadMonitor(monitor_id);
      }
    });
  }
  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }
  loadMonitor(monitor_id: number) {
    this.monitorService.getMonitor(monitor_id).subscribe({
      next: (monitor) => {
        this.monitor = monitor;
      },
      error: (err) => {
        console.error('Error getting Monitor:', err);
      }
    });
    this.monitorService.getRelatedPCs(monitor_id).subscribe({
      next: (pcs) => {
        this.related_pcs = pcs;
      },
      error: (err) => {
        console.error('Error getting related PCs:', err);
      }
    });
  }
  goBack() {
    this.router.navigateByUrl('parts/monitor');
  }
  goToRelatedPC(pc_id: number) {
    this.router.navigate(['parts/pc/', pc_id]);
  }
  editMonitor() {
    this.router.navigate(['parts/monitor/edit/', this.monitor!.id]);
  }
  deleteMonitor() {
    if (confirm("Είστε σίγουροι ότι θέλετε να διαγράψετε αυτόν τον επεξεργαστή;")) {
      this.monitorService.deleteMonitor(this.monitor!.id).subscribe({
        next: () => {
          this.router.navigate(['parts/monitor']);
        },
        error: (err) => {
          console.error("Error deleting Monitor:", err);
        }
      });
    }
  }
}
