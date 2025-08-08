import { CommonModule } from '@angular/common';
import { Component, ElementRef, HostListener, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { PC } from '../../../_model/pc';
import { Employee } from '../../../_model/employee';
import { ActivatedRoute, Router } from '@angular/router';
import { PCService } from '../../../_service/pc.service';
import { CPUService } from '../../../_service/cpu.service';
import { MOBOService } from '../../../_service/mobo.service';
import { RAMService } from '../../../_service/ram.service';
import { GPUService } from '../../../_service/gpu.service';
import { PSUService } from '../../../_service/psu.service';
import { StorageService } from '../../../_service/storage.service';
import { NetworkCardService } from '../../../_service/networkcard.service';
import { MonitorService } from '../../../_service/monitor.service';
import { CPU, GPU, MOBO, Monitor, NetworkCard, PSU, RAM, StorageDevice } from '../../../_model/parts';
import { EmployeeService } from '../../../_service/employee.service';
import { Observable, Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-pc-edit',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  templateUrl: './pc-edit.component.html',
  styleUrl: './pc-edit.component.css'
})
export class PCEditComponent implements OnInit {
  @ViewChild('reportInput') reportInput!: ElementRef;
  pc: PC | null = null;
  pcForm: FormGroup;
  haveSpeccy: boolean = false;
  selectedFile: File | null = null;
  private destroy$ = new Subject<void>();

  constructor(private activeRoute: ActivatedRoute,
              private router: Router,
              private fb: FormBuilder,
              private employeeService: EmployeeService,
              private pcService: PCService,
              private cpuService: CPUService,
              private moboService: MOBOService,
              private ramService: RAMService,
              private gpuService: GPUService,
              private psuService: PSUService,
              private storageService: StorageService,
              private networkService: NetworkCardService,
              private monitorService: MonitorService)
  {
    this.pcForm = this.fb.group({
      pcName: [''],
      barcode: [''],
      serialNumber: [''],
      brand: [''],
      model: [''],
      administratorCode: [''],
      biosCode: [''],
      pdfReportPath: [''],
      domain: [''],
      ip: [''],
      externalIP: [''],
      subnetMask: [''],
      gateway: [''],
      dnS1: [''],
      dnS2: [''],
      cpuiDs: [],
      moboiDs: [],
      ramiDs: [],
      gpuiDs: [],
      psuiDs: [],
      storageIDs: [],
      networkCardIDs: [],
      monitorIDs: []
    });
  }
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
        this.pcForm.patchValue(pc);
        if (pc.pdfReportPath) this.haveSpeccy = true;
        this.loadComponents(pc.cpuiDs, pc.moboiDs, pc.ramiDs, pc.gpuiDs, pc.psuiDs, pc.storageIDs, pc.networkCardIDs, pc.monitorIDs);
      }
    });
    this.pcService.getRelatedEmployees(pc_id).subscribe({
      next: response => {
        if (response.length != 0) {
          this.employees = response;
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
  goToEmployeeDetails(employee_id: number) {
    this.router.navigate(['catalog/', employee_id]);
  }
  savePC() {
    if (this.pcForm.valid && this.pc) {
      const updatedPC = { ...this.pc, ...this.pcForm.value };
      if (!this.selectedFile) {
        updatedPC.pdfReportPath = this.pc.pdfReportPath;
      }
      this.pcService.updatePC(updatedPC, this.selectedFile).subscribe({
        next: () => {
          this.pcForm.markAsPristine();
          if (this.reportInput) {
            this.reportInput.nativeElement.value = "";
          }
          this.selectedFile = null;
        },
        error: (err) => {
          console.error("Error updating PC:", err);
        }
      });
    }
  }
  onFileSelected(event: Event) {
    const fileInput = event.target as HTMLInputElement;
    if (fileInput.files && fileInput.files.length > 0) {
      this.selectedFile = fileInput.files[0];
      this.pcForm.markAsDirty();
    }
  }
  canDeactivate(): Observable<boolean> | Promise<boolean> | boolean {
    if (this.pcForm.dirty || this.selectedFile) {
      return confirm('Έχετε αλλαγές που δεν είναι αποθηκευμένες. Θέλετε να φύγετε?');
    }
    return true;
  }
  goBack() {
    this.router.navigate(['/parts/pc/', this.pc!.id]);
  }
  toggleSelection(componentName: string) {
    if (componentName == 'Employee') {
      this.loadAssigns(componentName);
      this.showEmployeeSelection = !this.showEmployeeSelection;
    }
    if (componentName == 'CPU') {
      this.loadAssigns(componentName);
      this.showCPUSelection = !this.showCPUSelection;
    }
    if (componentName == 'MOBO') {
      this.loadAssigns(componentName);
      this.showMOBOSelection = !this.showMOBOSelection;
    }
    if (componentName == 'RAM') {
      this.loadAssigns(componentName);
      this.showRAMSelection = !this.showRAMSelection;
    }
    if (componentName == 'GPU') {
      this.loadAssigns(componentName);
      this.showGPUSelection = !this.showGPUSelection;
    }
    if (componentName == 'PSU') {
      this.loadAssigns(componentName);
      this.showPSUSelection = !this.showPSUSelection;
    }
    if (componentName == 'Storage') {
      this.loadAssigns(componentName);
      this.showStorageSelection = !this.showStorageSelection;
    }
    if (componentName == 'NetworkCard') {
      this.loadAssigns(componentName);
      this.showNetworkCardSelection = !this.showNetworkCardSelection;
    }
    if (componentName == 'Monitor') {
      this.loadAssigns(componentName);
      this.showMonitorSelection = !this.showMonitorSelection;
    }
  }
  filtered(componentName: string) {
    if (componentName == 'Employee') {
      this.filteredEmployees = this.unassignedEmployees.filter(employee =>
        employee.lastName.toLowerCase().includes(this.employeeSearch.toLowerCase())
      );
    }
    if (componentName == 'CPU') {
      this.filteredCPUs = this.unassignedCPUs.filter(cpu =>
        cpu.barcode.toLowerCase().includes(this.cpuSearch.toLowerCase())
      );
    }
    if (componentName == 'MOBO') {
      this.filteredMOBOs = this.unassignedMOBOs.filter(mobo =>
        mobo.barcode.toLowerCase().includes(this.moboSearch.toLowerCase())
      );
    }
    if (componentName == 'RAM') {
      this.filteredRAMs = this.unassignedRAMs.filter(ram =>
        ram.barcode.toLowerCase().includes(this.ramSearch.toLowerCase())
      );
    }
    if (componentName == 'GPU') {
      this.filteredGPUs = this.unassignedGPUs.filter(gpu =>
        gpu.barcode.toLowerCase().includes(this.gpuSearch.toLowerCase())
      );
    }
    if (componentName == 'PSU') {
      this.filteredPSUs = this.unassignedPSUs.filter(psu =>
        psu.barcode.toLowerCase().includes(this.psuSearch.toLowerCase())
      );
    }
    if (componentName == 'Storage') {
      this.filteredStorages = this.unassignedStorages.filter(storage =>
        storage.barcode.toLowerCase().includes(this.storageSearch.toLowerCase())
      );
    }
    if (componentName == 'NetworkCard') {
      this.filteredNetworkCards = this.unassignedNetworkCards.filter(networkcard =>
        networkcard.barcode.toLowerCase().includes(this.networkcardSearch.toLowerCase())
      );
    }
    if (componentName == 'Monitor') {
      this.filteredMonitors = this.unassignedMonitors.filter(monitor =>
        monitor.barcode.toLowerCase().includes(this.monitorSearch.toLowerCase())
      );
    }
  }
  private loadAssigns(componentName: string) {
    if (componentName == 'Employee') {
      this.employeeService.getEmployees().subscribe({
        next: (employees) => {
          this.unassignedEmployees = employees;
        }
      });
    }
    if (componentName == 'CPU') {
      this.cpuService.getCPUs().subscribe({
        next: (cpus) => {
          this.unassignedCPUs = cpus;
        }
      });
    }
    if (componentName == 'MOBO') {
      this.moboService.getMOBOs().subscribe({
        next: (mobos) => {
          this.unassignedMOBOs = mobos;
        }
      });
    }
    if (componentName == 'RAM') {
      this.ramService.getRAMs().subscribe({
        next: (rams) => {
          this.unassignedRAMs = rams;
        }
      });
    }
    if (componentName == 'GPU') {
      this.gpuService.getGPUs().subscribe({
        next: (gpus) => {
          this.unassignedGPUs = gpus;
        }
      });
    }
    if (componentName == 'PSU') {
      this.psuService.getPSUs().subscribe({
        next: (psus) => {
          this.unassignedPSUs = psus;
        }
      });
    }
    if (componentName == 'Storage') {
      this.storageService.getStorages().subscribe({
        next: (storages) => {
          this.unassignedStorages = storages;
        }
      });
    }
    if (componentName == 'NetworkCard') {
      this.networkService.getNetworkCards().subscribe({
        next: (networkcards) => {
          this.unassignedNetworkCards = networkcards;
        }
      });
    }
    if (componentName == 'Monitor') {
      this.monitorService.getMonitors().subscribe({
        next: (monitors) => {
          this.unassignedMonitors = monitors;
        }
      });
    }
  }
  @HostListener('document:click', ['$event'])
  onClickOutside(event: Event) {
    setTimeout(() => {
      const inputField = document.querySelector('.form-control');
      const dropdown = document.querySelector('.dropdown-menu');
      if ( inputField && inputField.contains(event.target as Node) || dropdown && dropdown.contains(event.target as Node)) return;
      this.filteredEmployees = [];
      this.filteredCPUs = [];
      this.filteredMOBOs = [];
      this.filteredRAMs = [];
      this.filteredGPUs = [];
      this.filteredPSUs = [];
      this.filteredStorages = [];
      this.filteredNetworkCards = [];
      this.filteredMonitors = [];
    }, 100);
  }

  // Employees
  employees: Employee[] = [];
  unassignedEmployees: Employee[] = [];
  showEmployeeSelection: boolean = false;
  employeeSearch: string = "";
  filteredEmployees: any[] = [];
  assignEmployee(employee_id: number) {
    if (!employee_id) return;
    this.pcService.assignEmployee(this.pc!.id, employee_id).subscribe({
      next: (employee) => {
        this.showEmployeeSelection = false;
        this.pc!.employeeIDs.push(employee_id);
        this.employees.push(employee);
        this.employeeSearch = "";
        this.filteredEmployees = [];
      },
      error: (err) => {
        console.error('Error assigning Employee:', err);
        this.showEmployeeSelection = false;
      }
    });
  }
  removeEmployee(employee_id: number) {
    if (!employee_id) return;
    this.pcService.removeEmployee(this.pc!.id, employee_id).subscribe({
      next: () => {
        if (this.pc) {
          this.pc.employeeIDs = this.pc.employeeIDs.filter(id => id !== employee_id);
          this.employees = this.employees.filter(employee => employee.id !== employee_id);
        }
      },
      error: (err) => {
        console.error("Error removing Employee:", err);
      }
    });
  }
  // CPU
  cpus: CPU[] = [];
  unassignedCPUs: CPU[] = [];
  showCPUSelection: boolean = false;
  cpuSearch: string = "";
  filteredCPUs: any[] = [];
  assignCPU(cpu_id: number) {
    if (!cpu_id) return;
    this.pcService.assignCPU(this.pc!.id, cpu_id).subscribe({
      next: (cpu) => {
        this.showCPUSelection = false;
        this.pc!.cpuiDs.push(cpu_id);
        this.cpus.push(cpu);
        this.cpuSearch = "";
        this.filteredCPUs = [];
      },
      error: (err) => {
        console.error('Error assigning CPU:', err);
        this.showCPUSelection = false;
      }
    });
  }
  removeCPU(cpu_id: number) {
    if (!cpu_id) return;
    this.pcService.removeCPU(this.pc!.id, cpu_id).subscribe({
      next: () => {
        if (this.pc) {
          this.pc.cpuiDs = this.pc.cpuiDs.filter(id => id !== cpu_id);
          this.cpus = this.cpus.filter(cpu => cpu.id !== cpu_id);
        }
      },
      error: (err) => {
        console.error("Error removing CPU:", err);
      }
    });
  }
  goToAddCPU() {
    this.router.navigate(['parts/cpu/add/', this.pc!.id]);
  }
  // MOBO
  mobos: MOBO[] = [];
  unassignedMOBOs: MOBO[] = [];
  showMOBOSelection: boolean = false;
  moboSearch: string = "";
  filteredMOBOs: any[] = [];
  assignMOBO(mobo_id: number) {
    if (!mobo_id) return;
    this.pcService.assignMOBO(this.pc!.id, mobo_id).subscribe({
      next: (mobo) => {
        this.showMOBOSelection = false;
        this.pc!.moboiDs.push(mobo_id);
        this.mobos.push(mobo);
        this.moboSearch = "";
        this.filteredMOBOs = [];
      },
      error: (err) => {
        console.error('Error assigning MOBO:', err);
        this.showMOBOSelection = false;
      }
    });
  }
  removeMOBO(mobo_id: number) {
    if (!mobo_id) return;
    this.pcService.removeMOBO(this.pc!.id, mobo_id).subscribe({
      next: () => {
        if (this.pc) {
          this.pc.moboiDs = this.pc.moboiDs.filter(id => id !== mobo_id);
          this.mobos = this.mobos.filter(mobo => mobo.id !== mobo_id);
        }
      },
      error: (err) => {
        console.error("Error removing MOBO:", err);
      }
    });
  }
  goToAddMOBO() {
    this.router.navigate(['parts/mobo/add/', this.pc!.id]);
  }
  // RAM
  rams: RAM[] = [];
  unassignedRAMs: RAM[] = [];
  showRAMSelection: boolean = false;
  ramSearch: string = "";
  filteredRAMs: any[] = [];
  assignRAM(ram_id: number) {
    if (!ram_id) return;
    this.pcService.assignRAM(this.pc!.id, ram_id).subscribe({
      next: (ram) => {
        this.showRAMSelection = false;
        this.pc!.ramiDs.push(ram_id);
        this.rams.push(ram);
        this.ramSearch = "";
        this.filteredRAMs = [];
      },
      error: (err) => {
        console.error('Error assigning RAM:', err);
        this.showRAMSelection = false;
      }
    });
  }
  removeRAM(ram_id: number) {
    if (!ram_id) return;
    this.pcService.removeRAM(this.pc!.id, ram_id).subscribe({
      next: () => {
        if (this.pc) {
          this.pc.ramiDs = this.pc.ramiDs.filter(id => id !== ram_id);
          this.rams = this.rams.filter(ram => ram.id !== ram_id);
        }
      },
      error: (err) => {
        console.error("Error removing RAM:", err);
      }
    });
  }
  goToAddRAM() {
    this.router.navigate(['parts/ram/add/', this.pc!.id]);
  }
  // GPU
  gpus: GPU[] = [];
  unassignedGPUs: GPU[] = [];
  showGPUSelection: boolean = false;
  gpuSearch: string = "";
  filteredGPUs: any[] = [];
  assignGPU(gpu_id: number) {
    if (!gpu_id) return;
    this.pcService.assignGPU(this.pc!.id, gpu_id).subscribe({
      next: (gpu) => {
        this.showGPUSelection = false;
        this.pc!.gpuiDs.push(gpu_id);
        this.gpus.push(gpu);
        this.gpuSearch = "";
        this.filteredGPUs = [];
      },
      error: (err) => {
        console.error('Error assigning GPU:', err);
        this.showGPUSelection = false;
      }
    });
  }
  removeGPU(gpu_id: number) {
    if (!gpu_id) return;
    this.pcService.removeGPU(this.pc!.id, gpu_id).subscribe({
      next: () => {
        if (this.pc) {
          this.pc.gpuiDs = this.pc.gpuiDs.filter(id => id !== gpu_id);
          this.gpus = this.gpus.filter(gpu => gpu.id !== gpu_id);
        }
      },
      error: (err) => {
        console.error("Error removing GPU:", err);
      }
    });
  }
  goToAddGPU() {
    this.router.navigate(['parts/gpu/add/', this.pc!.id]);
  }
  // PSU
  psus: PSU[] = [];
  unassignedPSUs: PSU[] = [];
  showPSUSelection: boolean = false;
  psuSearch: string = "";
  filteredPSUs: any[] = [];
  assignPSU(psu_id: number) {
    if (!psu_id) return;
    this.pcService.assignPSU(this.pc!.id, psu_id).subscribe({
      next: (psu) => {
        this.showPSUSelection = false;
        this.pc!.psuiDs.push(psu_id);
        this.psus.push(psu);
        this.psuSearch = "";
        this.filteredPSUs = [];
      },
      error: (err) => {
        console.error('Error assigning PSU:', err);
        this.showPSUSelection = false;
      }
    });
  }
  removePSU(psu_id: number) {
    if (!psu_id) return;
    this.pcService.removePSU(this.pc!.id, psu_id).subscribe({
      next: () => {
        if (this.pc) {
          this.pc.psuiDs = this.pc.psuiDs.filter(id => id !== psu_id);
          this.psus = this.psus.filter(psu => psu.id !== psu_id);
        }
      },
      error: (err) => {
        console.error("Error removing PSU:", err);
      }
    });
  }
  goToAddPSU() {
    this.router.navigate(['parts/psu/add/', this.pc!.id]);
  }
  // Storage
  storages: StorageDevice[] = [];
  unassignedStorages: StorageDevice[] = [];
  showStorageSelection: boolean = false;
  storageSearch: string = "";
  filteredStorages: any[] = [];
  assignStorage(storage_id: number) {
    if (!storage_id) return;
    this.pcService.assignStorage(this.pc!.id, storage_id).subscribe({
      next: (storage) => {
        this.showStorageSelection = false;
        this.pc!.storageIDs.push(storage_id);
        this.storages.push(storage);
        this.storageSearch = "";
        this.filteredStorages = [];
      },
      error: (err) => {
        console.error('Error assigning Storage:', err);
        this.showStorageSelection = false;
      }
    });
  }
  removeStorage(storage_id: number) {
    if (!storage_id) return;
    this.pcService.removeStorage(this.pc!.id, storage_id).subscribe({
      next: () => {
        if (this.pc) {
          this.pc.storageIDs = this.pc.storageIDs.filter(id => id !== storage_id);
          this.storages = this.storages.filter(storage => storage.id !== storage_id);
        }
      },
      error: (err) => {
        console.error("Error removing Storage:", err);
      }
    });
  }
  goToAddStorage() {
    this.router.navigate(['parts/storage/add/', this.pc!.id]);
  }
  // NetworkCard
  networkcards: NetworkCard[] = [];
  unassignedNetworkCards: NetworkCard[] = [];
  showNetworkCardSelection: boolean = false;
  networkcardSearch: string = "";
  filteredNetworkCards: any[] = [];
  assignNetworkCard(networkcard_id: number) {
    if (!networkcard_id) return;
    this.pcService.assignNetworkCard(this.pc!.id, networkcard_id).subscribe({
      next: (networkcard) => {
        this.showNetworkCardSelection = false;
        this.pc!.networkCardIDs.push(networkcard_id);
        this.networkcards.push(networkcard);
        this.networkcardSearch = "";
        this.filteredNetworkCards = [];
      },
      error: (err) => {
        console.error('Error assigning NetworkCard:', err);
        this.showNetworkCardSelection = false;
      }
    });
  }
  removeNetworkCard(networkcard_id: number) {
    if (!networkcard_id) return;
    this.pcService.removeNetworkCard(this.pc!.id, networkcard_id).subscribe({
      next: () => {
        if (this.pc) {
          this.pc.networkCardIDs = this.pc.networkCardIDs.filter(id => id !== networkcard_id);
          this.networkcards = this.networkcards.filter(networkcard => networkcard.id !== networkcard_id);
        }
      },
      error: (err) => {
        console.error("Error removing NetworkCard:", err);
      }
    });
  }
  goToAddNetworkCard() {
    this.router.navigate(['parts/networkcard/add/', this.pc!.id]);
  }
  // Monitor
  monitors: Monitor[] = [];
  unassignedMonitors: Monitor[] = [];
  showMonitorSelection: boolean = false;
  monitorSearch: string = "";
  filteredMonitors: any[] = [];
  assignMonitor(monitor_id: number) {
    if (!monitor_id) return;
    this.pcService.assignMonitor(this.pc!.id, monitor_id).subscribe({
      next: (monitor) => {
        this.showMonitorSelection = false;
        this.pc!.monitorIDs.push(monitor_id);
        this.monitors.push(monitor);
        this.monitorSearch = "";
        this.filteredMonitors = [];
      },
      error: (err) => {
        console.error('Error assigning Monitor:', err);
        this.showMonitorSelection = false;
      }
    });
  }
  removeMonitor(monitor_id: number) {
    if (!monitor_id) return;
    this.pcService.removeMonitor(this.pc!.id, monitor_id).subscribe({
      next: () => {
        if (this.pc) {
          this.pc.monitorIDs = this.pc.monitorIDs.filter(id => id !== monitor_id);
          this.monitors = this.monitors.filter(monitor => monitor.id !== monitor_id);
        }
      },
      error: (err) => {
        console.error("Error removing Monitor:", err);
      }
    });
  }
  goToAddMonitor() {
    this.router.navigate(['parts/monitor/add/', this.pc!.id]);
  }
}
