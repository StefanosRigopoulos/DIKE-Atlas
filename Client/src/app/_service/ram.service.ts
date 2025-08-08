import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { getPaginatedResult, getPaginationHeaders } from '../_helper/paginationHelper';
import { map, Observable, of } from 'rxjs';
import { PC } from '../_model/pc';
import { RAM } from '../_model/parts';
import { RAMParams } from '../_model/params';

@Injectable({
  providedIn: 'root'
})
export class RAMService {
  baseUrl = environment.apiUrl;
  rams: RAM[] = [];
  ramCache = new Map();
  ramParams: RAMParams = new RAMParams();

  constructor(private http: HttpClient) {}

  getRAMParams() {
    return this.ramParams;
  }
  setRAMParams(ramParams: RAMParams) {
    this.ramParams = ramParams;
  }
  resetRAMParams() {
    this.ramParams = new RAMParams();
    return this.ramParams;
  }
  getRAM(id: number) {
    return this.http.get<RAM>(this.baseUrl + 'ram/get/' + id).pipe(
      map(response => {
        return response;
      })
    );
  }
  getPagedRAMs(ramParams: RAMParams) {
    const response = this.ramCache.get(Object.values(ramParams).join('-'));
    if (response) return of(response);
    let params = getPaginationHeaders(ramParams.pageNumber, ramParams.pageSize);
    params = params.append('barcode', ramParams.barcode);
    params = params.append('brand', ramParams.brand);
    params = params.append('model', ramParams.model);
    params = params.append('type', ramParams.type);
    params = params.append('size', ramParams.size);
    params = params.append('orderBy', ramParams.orderBy);
    return getPaginatedResult<RAM[]>(this.baseUrl + 'ram/get/paged', params, this.http).pipe(
      map(response => {
        this.ramCache.set(Object.values(ramParams).join('-'), response);
        return response;
      })
    );
  }
  getRAMs() {
    return this.http.get<RAM[]>(this.baseUrl + 'ram/get/all').pipe(
      map(response => {
        return response;
      })
    );
  }
  addRAM(ram: RAM): Observable<RAM> {
    return this.http.post<RAM>(this.baseUrl + 'ram/add', ram).pipe(
      map(newram => {
        this.ramCache.clear();
        return newram;
      })
    );
  }
  updateRAM(ram: RAM): Observable<RAM> {
    return this.http.put<RAM>(this.baseUrl + 'ram/update/' + ram.id, ram).pipe(
      map(updatedram => {
        this.ramCache.clear();
        return updatedram;
      })
    );
  }
  deleteRAM(id: number): Observable<void> {
    return this.http.delete<void>(this.baseUrl + 'ram/delete/' + id).pipe(
      map(() => {
        this.ramCache.clear();
      })
    );
  }
  getRelatedPCs(id: number) {
    return this.http.get<PC[]>(this.baseUrl + 'ram/get/related_pc/' + id).pipe(
      map(response => {
        return response;
      })
    );
  }

  // Filter Options
  getFilterBrand(): Observable<string[]> {
    return this.http.get<string[]>(this.baseUrl + 'ram/filter/brand').pipe(
      map(response => {
        return response;
      })
    );
  }
  getFilterModel(): Observable<string[]> {
    return this.http.get<string[]>(this.baseUrl + 'ram/filter/model').pipe(
      map(response => {
        return response;
      })
    );
  }
}