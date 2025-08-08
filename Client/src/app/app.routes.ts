import { Routes } from '@angular/router';
import { MainComponent } from './main/main.component';
import { AuthGuard } from './_guard/auth.guard';
import { PreventExitGuard } from './_guard/admin.guard';
import { AdminGuard } from './_guard/prevent-exit.guard';

import { AdminComponent } from './admin/admin.component';
import { SearchComponent } from './search/search.component';
import { CatalogPageComponent } from './catalog/catalog-page/catalog-page.component';
import { EmployeeDetailsComponent } from './catalog/employee-details/employee-details.component';
import { EmployeeAddComponent } from './catalog/employee-add/employee-add.component';
import { EmployeeEditComponent } from './catalog/employee-edit/employee-edit.component';
import { PartsPageComponent } from './parts/parts-page/parts-page.component';
import { PCPageComponent } from './parts/pc/pc-page/pc-page.component';
import { PCDetailsComponent } from './parts/pc/pc-details/pc-details.component';
import { PCEditComponent } from './parts/pc/pc-edit/pc-edit.component';
import { PCCreateComponent } from './parts/pc/pc-create/pc-create.component';
import { CPUPageComponent } from './parts/cpu/cpu-page/cpu-page.component';
import { CPUCreateComponent } from './parts/cpu/cpu-create/cpu-create.component';
import { CPUAddComponent } from './parts/cpu/cpu-add/cpu-add.component';
import { CPUEditComponent } from './parts/cpu/cpu-edit/cpu-edit.component';
import { CPUDetailsComponent } from './parts/cpu/cpu-details/cpu-details.component';
import { MOBOPageComponent } from './parts/mobo/mobo-page/mobo-page.component';
import { MOBOCreateComponent } from './parts/mobo/mobo-create/mobo-create.component';
import { MOBOAddComponent } from './parts/mobo/mobo-add/mobo-add.component';
import { MOBOEditComponent } from './parts/mobo/mobo-edit/mobo-edit.component';
import { MOBODetailsComponent } from './parts/mobo/mobo-details/mobo-details.component';
import { RAMPageComponent } from './parts/ram/ram-page/ram-page.component';
import { RAMCreateComponent } from './parts/ram/ram-create/ram-create.component';
import { RAMAddComponent } from './parts/ram/ram-add/ram-add.component';
import { RAMEditComponent } from './parts/ram/ram-edit/ram-edit.component';
import { RAMDetailsComponent } from './parts/ram/ram-details/ram-details.component';
import { GPUPageComponent } from './parts/gpu/gpu-page/gpu-page.component';
import { GPUCreateComponent } from './parts/gpu/gpu-create/gpu-create.component';
import { GPUAddComponent } from './parts/gpu/gpu-add/gpu-add.component';
import { GPUEditComponent } from './parts/gpu/gpu-edit/gpu-edit.component';
import { GPUDetailsComponent } from './parts/gpu/gpu-details/gpu-details.component';
import { PSUPageComponent } from './parts/psu/psu-page/psu-page.component';
import { PSUCreateComponent } from './parts/psu/psu-create/psu-create.component';
import { PSUAddComponent } from './parts/psu/psu-add/psu-add.component';
import { PSUEditComponent } from './parts/psu/psu-edit/psu-edit.component';
import { PSUDetailsComponent } from './parts/psu/psu-details/psu-details.component';
import { StoragePageComponent } from './parts/storage/storage-page/storage-page.component';
import { StorageCreateComponent } from './parts/storage/storage-create/storage-create.component';
import { StorageAddComponent } from './parts/storage/storage-add/storage-add.component';
import { StorageEditComponent } from './parts/storage/storage-edit/storage-edit.component';
import { StorageDetailsComponent } from './parts/storage/storage-details/storage-details.component';
import { NetworkCardPageComponent } from './parts/networkcard/networkcard-page/networkcard-page.component';
import { NetworkCardCreateComponent } from './parts/networkcard/networkcard-create/networkcard-create.component';
import { NetworkCardAddComponent } from './parts/networkcard/networkcard-add/networkcard-add.component';
import { NetworkCardEditComponent } from './parts/networkcard/networkcard-edit/networkcard-edit.component';
import { NetworkCardDetailsComponent } from './parts/networkcard/networkcard-details/networkcard-details.component';
import { MonitorPageComponent } from './parts/monitor/monitor-page/monitor-page.component';
import { MonitorCreateComponent } from './parts/monitor/monitor-create/monitor-create.component';
import { MonitorAddComponent } from './parts/monitor/monitor-add/monitor-add.component';
import { MonitorEditComponent } from './parts/monitor/monitor-edit/monitor-edit.component';
import { MonitorDetailsComponent } from './parts/monitor/monitor-details/monitor-details.component';


export const routes: Routes = [
    {path: '', component: MainComponent},
    {path: '',
      runGuardsAndResolvers: 'always',
      canActivate: [AuthGuard],
      children: [
        {path: '', component: MainComponent},
        {path: 'admin', component: AdminComponent, canActivate: [AdminGuard]},
        {path: 'search', component: SearchComponent},
        {path: 'catalog', component: CatalogPageComponent},
        {path: 'catalog/add', component: EmployeeAddComponent, canDeactivate: [PreventExitGuard]},
        {path: 'catalog/:employee_id', component: EmployeeDetailsComponent},
        {path: 'catalog/edit/:employee_id', component: EmployeeEditComponent, canDeactivate: [PreventExitGuard]},
        {path: 'parts', component: PartsPageComponent},
        {path: 'parts/pc', component: PCPageComponent},
        {path: 'parts/pc/create', component: PCCreateComponent, canDeactivate: [PreventExitGuard]},
        {path: 'parts/pc/:pc_id', component: PCDetailsComponent},
        {path: 'parts/pc/edit/:pc_id', component: PCEditComponent, canDeactivate: [PreventExitGuard]},
        {path: 'parts/cpu', component: CPUPageComponent},
        {path: 'parts/cpu/create', component: CPUCreateComponent, canDeactivate: [PreventExitGuard]},
        {path: 'parts/cpu/add/:pc_id', component: CPUAddComponent, canDeactivate: [PreventExitGuard]},
        {path: 'parts/cpu/edit/:cpu_id', component: CPUEditComponent, canDeactivate: [PreventExitGuard]},
        {path: 'parts/cpu/:cpu_id', component: CPUDetailsComponent},
        {path: 'parts/mobo', component: MOBOPageComponent},
        {path: 'parts/mobo/create', component: MOBOCreateComponent, canDeactivate: [PreventExitGuard]},
        {path: 'parts/mobo/add/:pc_id', component: MOBOAddComponent, canDeactivate: [PreventExitGuard]},
        {path: 'parts/mobo/edit/:mobo_id', component: MOBOEditComponent, canDeactivate: [PreventExitGuard]},
        {path: 'parts/mobo/:mobo_id', component: MOBODetailsComponent},
        {path: 'parts/ram', component: RAMPageComponent},
        {path: 'parts/ram/create', component: RAMCreateComponent, canDeactivate: [PreventExitGuard]},
        {path: 'parts/ram/add/:pc_id', component: RAMAddComponent, canDeactivate: [PreventExitGuard]},
        {path: 'parts/ram/edit/:ram_id', component: RAMEditComponent, canDeactivate: [PreventExitGuard]},
        {path: 'parts/ram/:ram_id', component: RAMDetailsComponent},
        {path: 'parts/gpu', component: GPUPageComponent},
        {path: 'parts/gpu/create', component: GPUCreateComponent, canDeactivate: [PreventExitGuard]},
        {path: 'parts/gpu/add/:pc_id', component: GPUAddComponent, canDeactivate: [PreventExitGuard]},
        {path: 'parts/gpu/edit/:gpu_id', component: GPUEditComponent, canDeactivate: [PreventExitGuard]},
        {path: 'parts/gpu/:gpu_id', component: GPUDetailsComponent},
        {path: 'parts/psu', component: PSUPageComponent},
        {path: 'parts/psu/create', component: PSUCreateComponent, canDeactivate: [PreventExitGuard]},
        {path: 'parts/psu/add/:pc_id', component: PSUAddComponent, canDeactivate: [PreventExitGuard]},
        {path: 'parts/psu/edit/:psu_id', component: PSUEditComponent, canDeactivate: [PreventExitGuard]},
        {path: 'parts/psu/:psu_id', component: PSUDetailsComponent},
        {path: 'parts/storage', component: StoragePageComponent},
        {path: 'parts/storage/create', component: StorageCreateComponent, canDeactivate: [PreventExitGuard]},
        {path: 'parts/storage/add/:pc_id', component: StorageAddComponent, canDeactivate: [PreventExitGuard]},
        {path: 'parts/storage/edit/:storage_id', component: StorageEditComponent, canDeactivate: [PreventExitGuard]},
        {path: 'parts/storage/:storage_id', component: StorageDetailsComponent},
        {path: 'parts/networkcard', component: NetworkCardPageComponent},
        {path: 'parts/networkcard/create', component: NetworkCardCreateComponent, canDeactivate: [PreventExitGuard]},
        {path: 'parts/networkcard/add/:pc_id', component: NetworkCardAddComponent, canDeactivate: [PreventExitGuard]},
        {path: 'parts/networkcard/edit/:networkcard_id', component: NetworkCardEditComponent, canDeactivate: [PreventExitGuard]},
        {path: 'parts/networkcard/:networkcard_id', component: NetworkCardDetailsComponent},
        {path: 'parts/monitor', component: MonitorPageComponent},
        {path: 'parts/monitor/create', component: MonitorCreateComponent, canDeactivate: [PreventExitGuard]},
        {path: 'parts/monitor/add/:pc_id', component: MonitorAddComponent, canDeactivate: [PreventExitGuard]},
        {path: 'parts/monitor/edit/:monitor_id', component: MonitorEditComponent, canDeactivate: [PreventExitGuard]},
        {path: 'parts/monitor/:monitor_id', component: MonitorDetailsComponent},
      ]
    },
    {path: '**', component: MainComponent, pathMatch: 'full'}
  ];