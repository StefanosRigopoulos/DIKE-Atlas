import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { getPaginatedResult, getPaginationHeaders } from '../_helper/paginationHelper';
import { map, Observable, of } from 'rxjs';
import { PC } from '../_model/pc';
import { MOBO } from '../_model/parts';
import { MOBOParams } from '../_model/params';

@Injectable({
  providedIn: 'root'
})
export class MOBOService {
  baseUrl = environment.apiUrl;
  mobos: MOBO[] = [];
  moboCache = new Map();
  moboParams: MOBOParams = new MOBOParams();

  constructor(private http: HttpClient) {}

  getMOBOParams() {
    return this.moboParams;
  }
  setMOBOParams(moboParams: MOBOParams) {
    this.moboParams = moboParams;
  }
  resetMOBOParams() {
    this.moboParams = new MOBOParams();
    return this.moboParams;
  }
  getMOBO(id: number): Observable<MOBO> {
    return this.http.get<MOBO>(this.baseUrl + 'mobo/get/' + id).pipe(
      map(response => {
        return response;
      })
    );
  }
  getPagedMOBOs(moboParams: MOBOParams) {
    const response = this.moboCache.get(Object.values(moboParams).join('-'));
    if (response) return of(response);
    let params = getPaginationHeaders(moboParams.pageNumber, moboParams.pageSize);
    params = params.append('barcode', moboParams.barcode);
    params = params.append('brand', moboParams.brand);
    params = params.append('model', moboParams.model);
    params = params.append('orderBy', moboParams.orderBy);
    return getPaginatedResult<MOBO[]>(this.baseUrl + 'mobo/get/paged', params, this.http).pipe(
      map(response => {
        this.moboCache.set(Object.values(moboParams).join('-'), response);
        return response;
      })
    );
  }
  getMOBOs() {
    return this.http.get<MOBO[]>(this.baseUrl + 'mobo/get/all').pipe(
      map(response => {
        return response;
      })
    );
  }
  addMOBO(mobo: MOBO): Observable<MOBO> {
    return this.http.post<MOBO>(this.baseUrl + 'mobo/add', mobo).pipe(
      map(newmobo => {
        this.moboCache.clear();
        return newmobo;
      })
    );
  }
  updateMOBO(mobo: MOBO): Observable<MOBO> {
    return this.http.put<MOBO>(this.baseUrl + 'mobo/update/' + mobo.id, mobo).pipe(
      map(updatedmobo => {
        this.moboCache.clear();
        return updatedmobo;
      })
    );
  }
  deleteMOBO(id: number): Observable<void> {
    return this.http.delete<void>(this.baseUrl + 'mobo/delete/' + id).pipe(
      map(() => {
        this.moboCache.clear();
      })
    );
  }
  getRelatedPCs(id: number) {
    return this.http.get<PC[]>(this.baseUrl + 'mobo/get/related_pc/' + id).pipe(
      map(response => {
        return response;
      })
    );
  }

  // Filter Options
  getFilterBrand(): Observable<string[]> {
    return this.http.get<string[]>(this.baseUrl + 'mobo/filter/brand').pipe(
      map(response => {
        return response;
      })
    );
  }
  getFilterModel(): Observable<string[]> {
    return this.http.get<string[]>(this.baseUrl + 'mobo/filter/model').pipe(
      map(response => {
        return response;
      })
    );
  }
}