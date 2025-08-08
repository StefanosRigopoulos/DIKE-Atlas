import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { CPUService } from '../../../_service/cpu.service';
import { PC } from '../../../_model/pc';
import { CPU } from '../../../_model/parts';
import { HasRoleDirective } from '../../../_directives/has-role.directive';
import { Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-cpu-details',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule, HasRoleDirective],
  templateUrl: './cpu-details.component.html',
  styleUrl: './cpu-details.component.css'
})
export class CPUDetailsComponent implements OnInit, OnDestroy {
  cpu: CPU | null = null;
  related_pcs: PC[] = [];
  private destroy$ = new Subject<void>();

  constructor(private activeRoute: ActivatedRoute,
              private router: Router,
              private cpuService: CPUService) {}
  ngOnInit() {
    this.activeRoute.paramMap
    .pipe(takeUntil(this.destroy$))
    .subscribe(params => {
      const cpu_id = +params.get('cpu_id')!;
      if (cpu_id) {
        this.loadCPU(cpu_id);
      }
    });
  }
  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }
  loadCPU(cpu_id: number) {
    this.cpuService.getCPU(cpu_id).subscribe({
      next: (cpu) => {
        this.cpu = cpu;
      },
      error: (err) => {
        console.error('Error getting CPU:', err);
      }
    });
    this.cpuService.getRelatedPCs(cpu_id).subscribe({
      next: (pcs) => {
        this.related_pcs = pcs;
      },
      error: (err) => {
        console.error('Error getting related PCs:', err);
      }
    });
  }
  goBack() {
    this.router.navigateByUrl('parts/cpu');
  }
  goToRelatedPC(pc_id: number) {
    this.router.navigate(['parts/pc/', pc_id]);
  }
  editCPU() {
    this.router.navigate(['parts/cpu/edit/', this.cpu!.id]);
  }
  deleteCPU() {
    if (confirm("Είστε σίγουροι ότι θέλετε να διαγράψετε αυτόν τον επεξεργαστή;")) {
      this.cpuService.deleteCPU(this.cpu!.id).subscribe({
        next: () => {
          this.router.navigate(['parts/cpu']);
        },
        error: (err) => {
          console.error("Error deleting CPU:", err);
        }
      });
    }
  }
}
