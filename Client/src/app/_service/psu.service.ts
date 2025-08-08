import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { getPaginatedResult, getPaginationHeaders } from '../_helper/paginationHelper';
import { map, Observable, of } from 'rxjs';
import { PC } from '../_model/pc';
import { PSU } from '../_model/parts';
import { PSUParams } from '../_model/params';

@Injectable({
  providedIn: 'root'
})
export class PSUService {
  baseUrl = environment.apiUrl;
  psus: PSU[] = [];
  psuCache = new Map();
  psuParams: PSUParams = new PSUParams();

  constructor(private http: HttpClient) {}

  getPSUParams() {
    return this.psuParams;
  }
  setPSUParams(psuParams: PSUParams) {
    this.psuParams = psuParams;
  }
  resetPSUParams() {
    this.psuParams = new PSUParams();
    return this.psuParams;
  }
  getPSU(id: number) {
    return this.http.get<PSU>(this.baseUrl + 'psu/get/' + id).pipe(
      map(response => {
        return response;
      })
    );
  }
  getPagedPSUs(psuParams: PSUParams) {
    const response = this.psuCache.get(Object.values(psuParams).join('-'));
    if (response) return of(response);
    let params = getPaginationHeaders(psuParams.pageNumber, psuParams.pageSize);
    params = params.append('barcode', psuParams.barcode);
    params = params.append('brand', psuParams.brand);
    params = params.append('model', psuParams.model);
    params = params.append('orderBy', psuParams.orderBy);
    return getPaginatedResult<PSU[]>(this.baseUrl + 'psu/get/paged', params, this.http).pipe(
      map(response => {
        this.psuCache.set(Object.values(psuParams).join('-'), response);
        return response;
      })
    );
  }
  getPSUs() {
    return this.http.get<PSU[]>(this.baseUrl + 'psu/get/all').pipe(
      map(response => {
        return response;
      })
    );
  }
  addPSU(psu: PSU): Observable<PSU> {
    return this.http.post<PSU>(this.baseUrl + 'psu/add', psu).pipe(
      map(newpsu => {
        this.psuCache.clear();
        return newpsu;
      })
    );
  }
  updatePSU(psu: PSU): Observable<PSU> {
    return this.http.put<PSU>(this.baseUrl + 'psu/update/' + psu.id, psu).pipe(
      map(updatedpsu => {
        this.psuCache.clear();
        return updatedpsu;
      })
    );
  }
  deletePSU(id: number): Observable<void> {
    return this.http.delete<void>(this.baseUrl + 'psu/delete/' + id).pipe(
      map(() => {
        this.psuCache.clear();
      })
    );
  }
  getRelatedPCs(id: number) {
    return this.http.get<PC[]>(this.baseUrl + 'psu/get/related_pc/' + id).pipe(
      map(response => {
        return response;
      })
    );
  }

  // Filter Options
  getFilterBrand(): Observable<string[]> {
    return this.http.get<string[]>(this.baseUrl + 'psu/filter/brand').pipe(
      map(response => {
        return response;
      })
    );
  }
  getFilterModel(): Observable<string[]> {
    return this.http.get<string[]>(this.baseUrl + 'psu/filter/model').pipe(
      map(response => {
        return response;
      })
    );
  }
}