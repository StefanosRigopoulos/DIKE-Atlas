import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { PCService } from '../../../_service/pc.service';
import { CommonModule } from '@angular/common';
import { PC } from '../../../_model/pc';
import { CPU, GPU, MOBO, Monitor, NetworkCard, StorageDevice, PSU, RAM } from '../../../_model/parts';
import { CPUService } from '../../../_service/cpu.service';
import { MOBOService } from '../../../_service/mobo.service';
import { RAMService } from '../../../_service/ram.service';
import { GPUService } from '../../../_service/gpu.service';
import { PSUService } from '../../../_service/psu.service';
import { StorageService } from '../../../_service/storage.service';
import { NetworkCardService } from '../../../_service/networkcard.service';
import { MonitorService } from '../../../_service/monitor.service';
import { FileService } from '../../../_service/file.service';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HasRoleDirective } from '../../../_directives/has-role.directive';
import { Employee } from '../../../_model/employee';
import { Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-pc-details',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule, HasRoleDirective],
  templateUrl: './pc-details.component.html',
  styleUrl: './pc-details.component.css'
})
export class PCDetailsComponent implements OnInit {
  pc: PC | null = null;
  haveSpeccy: boolean = false;
  related_employees: Employee[] = [];
  hasRelatedEmployees: boolean = false;
  private destroy$ = new Subject<void>();

  constructor(private activeRoute: ActivatedRoute,
              private router: Router,
              private fileService: FileService,
              private pcService: PCService,
              private cpuService: CPUService,
              private moboService: MOBOService,
              private ramService: RAMService,
              private gpuService: GPUService,
              private psuService: PSUService,
              private storageService: StorageService,
              private networkService: NetworkCardService,
              private monitorService: MonitorService) {}
  ngOnInit() {
    this.activeRoute.paramMap
    .pipe(takeUntil(this.destroy$))
    .subscribe(params => {
      const pc_id = +params.get('pc_id')!;
      if (pc_id) {
        this.loadPC(pc_id);
      }
    });
  }
  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }
  loadPC(pc_id: number) {
    this.pcService.getPC(pc_id).subscribe({
      next: (pc) => {
        this.pc = pc;
        if (pc.pdfReportPath && pc.pdfReportPath != "") this.haveSpeccy = true;
        this.loadComponents(pc.cpuiDs, pc.moboiDs, pc.ramiDs, pc.gpuiDs, pc.psuiDs, pc.storageIDs, pc.networkCardIDs, pc.monitorIDs);
      }
    });
    this.pcService.getRelatedEmployees(pc_id).subscribe({
      next: (employees) => {
        if (employees.length != 0) {
          this.related_employees = employees;
          this.hasRelatedEmployees = true;
        }
      },
      error: (err) => {
        console.error('Error getting related Employees:', err);
      }
    });
  }
  loadComponents(cpuiDs: number[], moboiDs: number[], ramiDs: number[], gpuiDs: number[], psuiDs: number[], storageIDs: number[], networkCardIDs: number[], monitorIDs: number[]) {
    if (cpuiDs.length > 0) {
      cpuiDs.forEach(id => {
        this.cpuService.getCPU(id).subscribe({
          next: response => {
            this.cpus.push(response);
          }
        })
      });
    }
    if (moboiDs.length > 0) {
      moboiDs.forEach(id => {
        this.moboService.getMOBO(id).subscribe({
          next: response => {
            this.mobos.push(response);
          }
        })
      });
    }
    if (ramiDs.length > 0) {
      ramiDs.forEach(id => {
        this.ramService.getRAM(id).subscribe({
          next: response => {
            this.rams.push(response);
          }
        })
      });
    }
    if (gpuiDs.length > 0) {
      gpuiDs.forEach(id => {
        this.gpuService.getGPU(id).subscribe({
          next: response => {
            this.gpus.push(response);
          }
        })
      });
    }
    if (psuiDs.length > 0) {
      psuiDs.forEach(id => {
        this.psuService.getPSU(id).subscribe({
          next: response => {
            this.psus.push(response);
          }
        })
      });
    }
    if (storageIDs.length > 0) {
      storageIDs.forEach(id => {
        this.storageService.getStorage(id).subscribe({
          next: response => {
            this.storages.push(response);
          }
        })
      });
    }
    if (networkCardIDs.length > 0) {
      networkCardIDs.forEach(id => {
        this.networkService.getNetworkCard(id).subscribe({
          next: response => {
            this.networkcards.push(response);
          }
        })
      });
    }
    if (monitorIDs.length > 0) {
      monitorIDs.forEach(id => {
        this.monitorService.getMonitor(id).subscribe({
          next: response => {
            this.monitors.push(response);
          }
        })
      });
    }
  }
  getSpeccyReport(pc_id: number) {
    this.pcService.getSpeccyReport(pc_id).subscribe({
      next: (blob: Blob) => {
        const url = window.URL.createObjectURL(blob);
        window.open(url, "_blank");
      },
      error: () => {
        console.log("Error processing the report!");
      }
    });
  }
  getPCReport(pc_id: number) {
    this.fileService.getPCReport(pc_id).subscribe({
      next: (blob: Blob) => {
        const url = window.URL.createObjectURL(blob);
        window.open(url, "_blank");
      },
      error: () => {
        console.log("Error processing the report!");
      }
    });
  }
  goToEmployeeDetails(employee_id: number) {
    this.router.navigate(['catalog/', employee_id]);
  }
  editPC() {
    this.router.navigate(['parts/pc/edit/', this.pc!.id]);
  }
  deletePC() {
    if (confirm("Είστε σίγουροι ότι θέλετε να διαγράψετε αυτόν τον H/Y;")) {
      if (!this.pc) return;
      this.pcService.deletePC(this.pc.id).subscribe({
          next: () => {
              this.router.navigateByUrl('/parts/pc');
          },
          error: (err) => {
              console.error('Error while deleting PC:', err);
          }
      });
    }
  }
  goBack() {
    this.router.navigateByUrl('parts/pc');
  }
  
  // Navigation to Components
  cpus: CPU[] = [];
  mobos: MOBO[] = [];
  rams: RAM[] = [];
  gpus: GPU[] = [];
  psus: PSU[] = [];
  storages: StorageDevice[] = [];
  networkcards: NetworkCard[] = [];
  monitors: Monitor[] = [];
  goToCPU(cpu_id: number) {
    this.router.navigate(['/parts/cpu/', cpu_id]);
  }
  goToMOBO(mobo_id: number) {
    this.router.navigate(['/parts/mobo/', mobo_id]);
  }
  goToRAM(ram_id: number) {
    this.router.navigate(['/parts/ram/', ram_id]);
  }
  goToGPU(gpu_id: number) {
    this.router.navigate(['/parts/gpu/', gpu_id]);
  }
  goToPSU(psu_id: number) {
    this.router.navigate(['/parts/psu/', psu_id]);
  }
  goToStorage(storage_id: number) {
    this.router.navigate(['/parts/storage/', storage_id]);
  }
  goToNetworkCard(networkcard_id: number) {
    this.router.navigate(['/parts/networkcard/', networkcard_id]);
  }
  goToMonitor(monitor_id: number) {
    this.router.navigate(['/parts/monitor/', monitor_id]);
  }
}
