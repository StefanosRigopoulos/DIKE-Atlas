import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { map, Observable, of } from 'rxjs';
import { getPaginatedResult, getPaginationHeaders } from '../_helper/paginationHelper';
import { PC } from '../_model/pc';
import { CPU } from '../_model/parts';
import { CPUParams } from '../_model/params';

@Injectable({
  providedIn: 'root'
})
export class CPUService {
  baseUrl = environment.apiUrl;
  cpus: CPU[] = [];
  cpuCache = new Map();
  cpuParams: CPUParams = new CPUParams();

  constructor(private http: HttpClient) {}

  getCPUParams() {
    return this.cpuParams;
  }
  setCPUParams(cpuParams: CPUParams) {
    this.cpuParams = cpuParams;
  }
  resetCPUParams() {
    this.cpuParams = new CPUParams();
    return this.cpuParams;
  }
  getCPU(id: number): Observable<CPU> {
    return this.http.get<CPU>(this.baseUrl + 'cpu/get/' + id).pipe(
      map(response => {
        return response;
      })
    );
  }
  getPagedCPUs(cpuParams: CPUParams) {
    const response = this.cpuCache.get(Object.values(cpuParams).join('-'));
    if (response) return of(response);
    let params = getPaginationHeaders(cpuParams.pageNumber, cpuParams.pageSize);
    params = params.append('barcode', cpuParams.barcode);
    params = params.append('brand', cpuParams.brand);
    params = params.append('model', cpuParams.model);
    params = params.append('cores', cpuParams.cores);
    params = params.append('orderBy', cpuParams.orderBy);
    return getPaginatedResult<CPU[]>(this.baseUrl + 'cpu/get/paged', params, this.http).pipe(
      map(response => {
        this.cpuCache.set(Object.values(cpuParams).join('-'), response);
        return response;
      })
    );
  }
  getCPUs() {
    return this.http.get<CPU[]>(this.baseUrl + 'cpu/get/all').pipe(
      map(response => {
        return response;
      })
    );
  }
  addCPU(cpu: CPU): Observable<CPU> {
    return this.http.post<CPU>(this.baseUrl + 'cpu/add', cpu).pipe(
      map(newCpu => {
        this.cpuCache.clear();
        return newCpu;
      })
    );
  }
  updateCPU(cpu: CPU): Observable<CPU> {
    return this.http.put<CPU>(this.baseUrl + 'cpu/update/' + cpu.id, cpu).pipe(
      map(updatedCpu => {
        this.cpuCache.clear();
        return updatedCpu;
      })
    );
  }
  deleteCPU(id: number): Observable<void> {
    return this.http.delete<void>(this.baseUrl + 'cpu/delete/' + id).pipe(
      map(() => {
        this.cpuCache.clear();
      })
    );
  }
  getRelatedPCs(id: number) {
    return this.http.get<PC[]>(this.baseUrl + 'cpu/get/related_pc/' + id).pipe(
      map(response => {
        return response;
      })
    );
  }

  // Filter Options
  getFilterBrand(): Observable<string[]> {
    return this.http.get<string[]>(this.baseUrl + 'cpu/filter/brand').pipe(
      map(response => {
        return response;
      })
    );
  }
  getFilterModel(): Observable<string[]> {
    return this.http.get<string[]>(this.baseUrl + 'cpu/filter/model').pipe(
      map(response => {
        return response;
      })
    );
  }
  getFilterCore(): Observable<string[]> {
    return this.http.get<string[]>(this.baseUrl + 'cpu/filter/core').pipe(
      map(response => {
        return response;
      })
    );
  }
}
