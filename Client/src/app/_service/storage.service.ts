import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { getPaginatedResult, getPaginationHeaders } from '../_helper/paginationHelper';
import { map, Observable, of } from 'rxjs';
import { PC } from '../_model/pc';
import { StorageDevice } from '../_model/parts';
import { StorageParams } from '../_model/params';

@Injectable({
  providedIn: 'root'
})
export class StorageService {
  baseUrl = environment.apiUrl;
  storages: StorageDevice[] = [];
  storageCache = new Map();
  storageParams: StorageParams = new StorageParams();

  constructor(private http: HttpClient) {}

  getStorageParams() {
    return this.storageParams;
  }
  setStorageParams(storageParams: StorageParams) {
    this.storageParams = storageParams;
  }
  resetStorageParams() {
    this.storageParams = new StorageParams();
    return this.storageParams;
  }
  getStorage(id: number) {
    return this.http.get<StorageDevice>(this.baseUrl + 'storage/get/' + id).pipe(
      map(response => {
        return response;
      })
    );
  }
  getPagedStorages(storageParams: StorageParams) {
    const response = this.storageCache.get(Object.values(storageParams).join('-'));
    if (response) return of(response);
    let params = getPaginationHeaders(storageParams.pageNumber, storageParams.pageSize);
    params = params.append('barcode', storageParams.barcode);
    params = params.append('brand', storageParams.brand);
    params = params.append('model', storageParams.model);
    params = params.append('orderBy', storageParams.orderBy);

    return getPaginatedResult<StorageDevice[]>(this.baseUrl + 'storage/get/paged', params, this.http).pipe(
      map(response => {
        this.storageCache.set(Object.values(storageParams).join('-'), response);
        return response;
      })
    );
  }
  getStorages() {
    return this.http.get<StorageDevice[]>(this.baseUrl + 'storage/get/all').pipe(
      map(response => {
        return response;
      })
    );
  }
  addStorage(storage: StorageDevice): Observable<StorageDevice> {
    return this.http.post<StorageDevice>(this.baseUrl + 'storage/add', storage).pipe(
      map(newstorage => {
        this.storageCache.clear();
        return newstorage;
      })
    );
  }
  updateStorage(storage: StorageDevice): Observable<StorageDevice> {
    return this.http.put<StorageDevice>(this.baseUrl + 'storage/update/' + storage.id, storage).pipe(
      map(updatedstorage => {
        this.storageCache.clear();
        return updatedstorage;
      })
    );
  }
  deleteStorage(id: number): Observable<void> {
    return this.http.delete<void>(this.baseUrl + 'storage/delete/' + id).pipe(
      map(() => {
        this.storageCache.clear();
      })
    );
  }
  getRelatedPCs(id: number) {
    return this.http.get<PC[]>(this.baseUrl + 'storage/get/related_pc/' + id).pipe(
      map(response => {
        return response;
      })
    );
  }

  // Filter Options
  getFilterBrand(): Observable<string[]> {
    return this.http.get<string[]>(this.baseUrl + 'storage/filter/brand').pipe(
      map(response => {
        return response;
      })
    );
  }
  getFilterModel(): Observable<string[]> {
    return this.http.get<string[]>(this.baseUrl + 'storage/filter/model').pipe(
      map(response => {
        return response;
      })
    );
  }
  getFilterType(): Observable<string[]> {
    return this.http.get<string[]>(this.baseUrl + 'storage/filter/type').pipe(
      map(response => {
        return response;
      })
    );
  }
  getFilterInterface(): Observable<string[]> {
    return this.http.get<string[]>(this.baseUrl + 'storage/filter/interface').pipe(
      map(response => {
        return response;
      })
    );
  }
}