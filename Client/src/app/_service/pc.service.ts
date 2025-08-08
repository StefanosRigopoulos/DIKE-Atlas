import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { map, Observable, of } from 'rxjs';
import { getPaginatedResult, getPaginationHeaders } from '../_helper/paginationHelper';
import { PC, PCOnly } from '../_model/pc';
import { PCParams } from '../_model/params';
import { Employee } from '../_model/employee';
import { CPU, GPU, MOBO, Monitor, NetworkCard, PSU, RAM, StorageDevice } from '../_model/parts';

@Injectable({
  providedIn: 'root'
})
export class PCService {
  baseUrl = environment.apiUrl;
  pcs: PC[] = [];
  pcCache = new Map();
  pcParams: PCParams = new PCParams();

  constructor(private http: HttpClient) {}

  getPCParams() {
    return this.pcParams;
  }
  setPCParams(pcParams: PCParams) {
    this.pcParams = pcParams;
  }
  resetPCParams() {
    this.pcParams = new PCParams();
    return this.pcParams;
  }
  getPC(id: number): Observable<PC> {
    return this.http.get<PC>(this.baseUrl + 'pc/get/' + id).pipe(
      map(response => {
        return response;
      })
    );
  }
  getPCs(): Observable<PC[]> {
    return this.http.get<PC[]>(this.baseUrl + 'pc/get/all').pipe(
      map(response => {
        return response;
      })
    );
  }
  getSpeccyReport(pc_id: number) {
    return this.http.get(this.baseUrl + 'pc/get/' + pc_id + '/report', { responseType: 'blob' });
  }
  getPagedPCs(pcParams: PCParams) {
    const response = this.pcCache.get(Object.values(pcParams).join('-'));
    if (response) return of(response);
    let params = getPaginationHeaders(pcParams.pageNumber, pcParams.pageSize);
    params = params.append('barcode', pcParams.barcode);
    params = params.append('brand', pcParams.brand);
    params = params.append('model', pcParams.model);
    params = params.append('domain', pcParams.domain);
    params = params.append('orderBy', pcParams.orderBy);
    return getPaginatedResult<PCOnly[]>(this.baseUrl + 'pc/get/all/paged', params, this.http).pipe(
      map(response => {
        this.pcCache.set(Object.values(pcParams).join('-'), response);
        return response;
      })
    );
  }
  addPC(pc: PC, file: File | null): Observable<PC> {
    const formData = new FormData();
    if (pc.brand) formData.append('brand', pc.brand);
    if (pc.model) formData.append('model', pc.model);
    if (pc.pcName) formData.append('pcname', pc.pcName);
    if (pc.administratorCode) formData.append('administratorcode', pc.administratorCode);
    if (pc.biosCode) formData.append('bioscode', pc.biosCode);
    if (pc.domain) formData.append('domain', pc.domain);
    if (pc.ip) formData.append('ip', pc.ip);
    if (pc.subnetMask) formData.append('subnetmask', pc.subnetMask);
    if (pc.gateway) formData.append('gateway', pc.gateway);
    if (pc.dnS1) formData.append('dns1', pc.dnS1);
    if (pc.dnS2) formData.append('dns2', pc.dnS2);
    if (file) formData.append('reportpdf', file);
    return this.http.post<PC>(this.baseUrl + 'pc/add', formData).pipe(
      map(newPC => {
        this.pcCache.clear();
        return newPC;
      })
    );
  }
  updatePC(pc: PC, file: File | null): Observable<PC> {
    const formData = new FormData();
    if (pc.barcode) formData.append('barcode', pc.barcode);
    if (pc.brand) formData.append('brand', pc.brand);
    if (pc.model) formData.append('model', pc.model);
    if (pc.pcName) formData.append('pcname', pc.pcName);
    if (pc.administratorCode) formData.append('administratorcode', pc.administratorCode);
    if (pc.biosCode) formData.append('bioscode', pc.biosCode);
    if (pc.pdfReportPath) formData.append('pdfReportPath', pc.pdfReportPath);
    if (pc.domain) formData.append('domain', pc.domain);
    if (pc.ip) formData.append('ip', pc.ip);
    if (pc.subnetMask) formData.append('subnetmask', pc.subnetMask);
    if (pc.gateway) formData.append('gateway', pc.gateway);
    if (pc.dnS1) formData.append('dns1', pc.dnS1);
    if (pc.dnS2) formData.append('dns2', pc.dnS2);
    if (pc.pdfReportPath) formData.append('pdfReportPath', pc.pdfReportPath);
    if (file) formData.append('reportpdf', file);
    return this.http.put<PC>(this.baseUrl + 'pc/update/' + pc.id, formData).pipe(
      map(updatedpc => {
        this.pcCache.clear();
        return updatedpc;
      })
    );
  }
  deletePC(id: number): Observable<void> {
    return this.http.delete<void>(this.baseUrl + 'pc/delete/' + id).pipe(
      map(() => {
        this.pcCache.clear();
      })
    );
  }
  getRelatedEmployees(id: number) {
    return this.http.get<Employee[]>(this.baseUrl + 'pc/employee_get/' + id).pipe(
      map(response => {
        return response;
      })
    );
  }
  
  // Relationships
  assignEmployee(pc_id: number, employee_id: number): Observable<Employee> {
    return this.http.put<Employee>(this.baseUrl + 'pc/employee_assign/' + pc_id + '/' + employee_id, null).pipe(
      map(response => {
        return response;
      })
    );
  }
  removeEmployee(pc_id: number, employee_id: number) {
    return this.http.delete(this.baseUrl + 'pc/employee_remove/' + pc_id + '/' + employee_id);
  }
  assignCPU(pc_id: number, cpu_id: number): Observable<CPU> {
    return this.http.put<CPU>(this.baseUrl + 'pc/cpu_assign/' + pc_id + '/' + cpu_id, null).pipe(
      map(response => {
        return response;
      })
    );
  }
  removeCPU(pc_id: number, cpu_id: number) {
    return this.http.delete(this.baseUrl + 'pc/cpu_remove/' + pc_id + '/' + cpu_id);
  }
  assignMOBO(pc_id: number, mobo_id: number): Observable<MOBO> {
    return this.http.put<MOBO>(this.baseUrl + 'pc/mobo_assign/' + pc_id + '/' + mobo_id, null).pipe(
      map(response => {
        return response;
      })
    );
  }
  removeMOBO(pc_id: number, mobo_id: number) {
    return this.http.delete(this.baseUrl + 'pc/mobo_remove/' + pc_id + '/' + mobo_id);
  }
  assignRAM(pc_id: number, ram_id: number): Observable<RAM> {
    return this.http.put<RAM>(this.baseUrl + 'pc/ram_assign/' + pc_id + '/' + ram_id, null).pipe(
      map(response => {
        return response;
      })
    );
  }
  removeRAM(pc_id: number, ram_id: number) {
    return this.http.delete(this.baseUrl + 'pc/ram_remove/' + pc_id + '/' + ram_id);
  }
  assignGPU(pc_id: number, gpu_id: number): Observable<GPU> {
    return this.http.put<GPU>(this.baseUrl + 'pc/gpu_assign/' + pc_id + '/' + gpu_id, null).pipe(
      map(response => {
        return response;
      })
    );
  }
  removeGPU(pc_id: number, gpu_id: number) {
    return this.http.delete(this.baseUrl + 'pc/gpu_remove/' + pc_id + '/' + gpu_id);
  }
  assignPSU(pc_id: number, psu_id: number): Observable<PSU> {
    return this.http.put<PSU>(this.baseUrl + 'pc/psu_assign/' + pc_id + '/' + psu_id, null).pipe(
      map(response => {
        return response;
      })
    );
  }
  removePSU(pc_id: number, psu_id: number) {
    return this.http.delete(this.baseUrl + 'pc/psu_remove/' + pc_id + '/' + psu_id);
  }
  assignStorage(pc_id: number, storage_id: number): Observable<StorageDevice> {
    return this.http.put<StorageDevice>(this.baseUrl + 'pc/storage_assign/' + pc_id + '/' + storage_id, null).pipe(
      map(response => {
        return response;
      })
    );
  }
  removeStorage(pc_id: number, storage_id: number) {
    return this.http.delete(this.baseUrl + 'pc/storage_remove/' + pc_id + '/' + storage_id);
  }
  assignNetworkCard(pc_id: number, networkcard_id: number): Observable<NetworkCard> {
    return this.http.put<NetworkCard>(this.baseUrl + 'pc/networkcard_assign/' + pc_id + '/' + networkcard_id, null).pipe(
      map(response => {
        return response;
      })
    );
  }
  removeNetworkCard(pc_id: number, networkcard_id: number) {
    return this.http.delete(this.baseUrl + 'pc/networkcard_remove/' + pc_id + '/' + networkcard_id);
  }
  assignMonitor(pc_id: number, monitor_id: number): Observable<Monitor> {
    return this.http.put<Monitor>(this.baseUrl + 'pc/monitor_assign/' + pc_id + '/' + monitor_id, null).pipe(
      map(response => {
        return response;
      })
    );
  }
  removeMonitor(pc_id: number, monitor_id: number) {
    return this.http.delete(this.baseUrl + 'pc/monitor_remove/' + pc_id + '/' + monitor_id);
  }

  // Filter Options
  getFilterBrand(): Observable<string[]> {
    return this.http.get<string[]>(this.baseUrl + 'pc/filter/brand').pipe(
      map(response => {
        return response;
      })
    );
  }
  getFilterModel(): Observable<string[]> {
    return this.http.get<string[]>(this.baseUrl + 'pc/filter/model').pipe(
      map(response => {
        return response;
      })
    );
  }
  getFilterDomain(): Observable<string[]> {
    return this.http.get<string[]>(this.baseUrl + 'pc/filter/domain').pipe(
      map(response => {
        return response;
      })
    );
  }
}
