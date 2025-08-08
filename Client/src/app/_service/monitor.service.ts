import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { getPaginatedResult, getPaginationHeaders } from '../_helper/paginationHelper';
import { map, Observable, of } from 'rxjs';
import { PC } from '../_model/pc';
import { Monitor } from '../_model/parts';
import { MonitorParams } from '../_model/params';

@Injectable({
  providedIn: 'root'
})
export class MonitorService {
  baseUrl = environment.apiUrl;
  monitors: Monitor[] = [];
  monitorCache = new Map();
  monitorParams: MonitorParams = new MonitorParams();

  constructor(private http: HttpClient) {}

  getMonitorParams() {
    return this.monitorParams;
  }
  setMonitorParams(monitorParams: MonitorParams) {
    this.monitorParams = monitorParams;
  }
  resetMonitorParams() {
    this.monitorParams = new MonitorParams();
    return this.monitorParams;
  }
  getMonitor(id: number) {
    return this.http.get<Monitor>(this.baseUrl + 'monitor/get/' + id).pipe(
      map(response => {
        return response;
      })
    );
  }
  getPagedMonitors(monitorParams: MonitorParams) {
    const response = this.monitorCache.get(Object.values(monitorParams).join('-'));
    if (response) return of(response);

    let params = getPaginationHeaders(monitorParams.pageNumber, monitorParams.pageSize);

    params = params.append('barcode', monitorParams.barcode);
    params = params.append('brand', monitorParams.brand);
    params = params.append('model', monitorParams.model);
    params = params.append('orderBy', monitorParams.orderBy);

    return getPaginatedResult<Monitor[]>(this.baseUrl + 'monitor/get/paged', params, this.http).pipe(
      map(response => {
        this.monitorCache.set(Object.values(monitorParams).join('-'), response);
        return response;
      })
    );
  }
  getMonitors() {
    return this.http.get<Monitor[]>(this.baseUrl + 'monitor/get/all').pipe(
      map(response => {
        return response;
      })
    );
  }
  addMonitor(monitor: Monitor): Observable<Monitor> {
    return this.http.post<Monitor>(this.baseUrl + 'monitor/add', monitor).pipe(
      map(newmonitor => {
        this.monitorCache.clear();
        return newmonitor;
      })
    );
  }
  updateMonitor(monitor: Monitor): Observable<Monitor> {
    return this.http.put<Monitor>(this.baseUrl + 'monitor/update/' + monitor.id, monitor).pipe(
      map(updatedmonitor => {
        this.monitorCache.clear();
        return updatedmonitor;
      })
    );
  }
  deleteMonitor(id: number): Observable<void> {
    return this.http.delete<void>(this.baseUrl + 'monitor/delete/' + id).pipe(
      map(() => {
        this.monitorCache.clear();
      })
    );
  }
  getRelatedPCs(id: number) {
    return this.http.get<PC[]>(this.baseUrl + 'monitor/get/related_pc/' + id).pipe(
      map(response => {
        return response;
      })
    );
  }

  // Filter Options
  getFilterBrand(): Observable<string[]> {
    return this.http.get<string[]>(this.baseUrl + 'monitor/filter/brand').pipe(
      map(response => {
        return response;
      })
    );
  }
  getFilterModel(): Observable<string[]> {
    return this.http.get<string[]>(this.baseUrl + 'monitor/filter/model').pipe(
      map(response => {
        return response;
      })
    );
  }
}