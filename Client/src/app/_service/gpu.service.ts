import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { getPaginatedResult, getPaginationHeaders } from '../_helper/paginationHelper';
import { map, Observable, of } from 'rxjs';
import { PC } from '../_model/pc';
import { GPU } from '../_model/parts';
import { GPUParams } from '../_model/params';

@Injectable({
  providedIn: 'root'
})
export class GPUService {
  baseUrl = environment.apiUrl;
  gpus: GPU[] = [];
  gpuCache = new Map();
  gpuParams: GPUParams = new GPUParams();

  constructor(private http: HttpClient) {}

  getGPUParams() {
    return this.gpuParams;
  }
  setGPUParams(gpuParams: GPUParams) {
    this.gpuParams = gpuParams;
  }
  resetGPUParams() {
    this.gpuParams = new GPUParams();
    return this.gpuParams;
  }
  getGPU(id: number) {
    return this.http.get<GPU>(this.baseUrl + 'gpu/get/' + id).pipe(
      map(response => {
        return response;
      })
    );
  }
  getPagedGPUs(gpuParams: GPUParams) {
    const response = this.gpuCache.get(Object.values(gpuParams).join('-'));
    if (response) return of(response);
    let params = getPaginationHeaders(gpuParams.pageNumber, gpuParams.pageSize);
    params = params.append('barcode', gpuParams.barcode);
    params = params.append('brand', gpuParams.brand);
    params = params.append('model', gpuParams.model);
    params = params.append('memory', gpuParams.memory);
    params = params.append('orderBy', gpuParams.orderBy);
    return getPaginatedResult<GPU[]>(this.baseUrl + 'gpu/get/paged', params, this.http).pipe(
      map(response => {
        this.gpuCache.set(Object.values(gpuParams).join('-'), response);
        return response;
      })
    );
  }
  getGPUs() {
    return this.http.get<GPU[]>(this.baseUrl + 'gpu/get/all').pipe(
      map(response => {
        return response;
      })
    );
  }
  addGPU(gpu: GPU): Observable<GPU> {
    return this.http.post<GPU>(this.baseUrl + 'gpu/add', gpu).pipe(
      map(newgpu => {
        this.gpuCache.clear();
        return newgpu;
      })
    );
  }
  updateGPU(gpu: GPU): Observable<GPU> {
    return this.http.put<GPU>(this.baseUrl + 'gpu/update/' + gpu.id, gpu).pipe(
      map(updatedgpu => {
        this.gpuCache.clear();
        return updatedgpu;
      })
    );
  }
  deleteGPU(id: number): Observable<void> {
    return this.http.delete<void>(this.baseUrl + 'gpu/delete/' + id).pipe(
      map(() => {
        this.gpuCache.clear();
      })
    );
  }
  getRelatedPCs(id: number) {
    return this.http.get<PC[]>(this.baseUrl + 'gpu/get/related_pc/' + id).pipe(
      map(response => {
        return response;
      })
    );
  }

  // Filter Options
  getFilterBrand(): Observable<string[]> {
    return this.http.get<string[]>(this.baseUrl + 'gpu/filter/brand').pipe(
      map(response => {
        return response;
      })
    );
  }
  getFilterModel(): Observable<string[]> {
    return this.http.get<string[]>(this.baseUrl + 'gpu/filter/model').pipe(
      map(response => {
        return response;
      })
    );
  }
}