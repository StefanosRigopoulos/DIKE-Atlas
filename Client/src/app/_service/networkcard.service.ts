import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { getPaginatedResult, getPaginationHeaders } from '../_helper/paginationHelper';
import { map, Observable, of } from 'rxjs';
import { PC } from '../_model/pc';
import { NetworkCard } from '../_model/parts';
import { NetworkCardParams } from '../_model/params';

@Injectable({
  providedIn: 'root'
})
export class NetworkCardService {
  baseUrl = environment.apiUrl;
  networkcards: NetworkCard[] = [];
  networkcardCache = new Map();
  networkcardParams: NetworkCardParams = new NetworkCardParams();

  constructor(private http: HttpClient) {}

  getNetworkCardParams() {
    return this.networkcardParams;
  }
  setNetworkCardParams(networkcardParams: NetworkCardParams) {
    this.networkcardParams = networkcardParams;
  }
  resetNetworkCardParams() {
    this.networkcardParams = new NetworkCardParams();
    return this.networkcardParams;
  }
  getNetworkCard(id: number) {
    return this.http.get<NetworkCard>(this.baseUrl + 'networkcard/get/' + id).pipe(
      map(response => {
        return response;
      })
    );
  }
  getPagedNetworkCards(networkcardParams: NetworkCardParams) {
    const response = this.networkcardCache.get(Object.values(networkcardParams).join('-'));
    if (response) return of(response);
    let params = getPaginationHeaders(networkcardParams.pageNumber, networkcardParams.pageSize);
    params = params.append('barcode', networkcardParams.barcode);
    params = params.append('brand', networkcardParams.brand);
    params = params.append('model', networkcardParams.model);
    params = params.append('orderBy', networkcardParams.orderBy);
    return getPaginatedResult<NetworkCard[]>(this.baseUrl + 'networkcard/get/paged', params, this.http).pipe(
      map(response => {
        this.networkcardCache.set(Object.values(networkcardParams).join('-'), response);
        return response;
      })
    );
  }
  getNetworkCards() {
    return this.http.get<NetworkCard[]>(this.baseUrl + 'networkcard/get/all').pipe(
      map(response => {
        return response;
      })
    );
  }
  addNetworkCard(networkcard: NetworkCard): Observable<NetworkCard> {
    return this.http.post<NetworkCard>(this.baseUrl + 'networkcard/add', networkcard).pipe(
      map(newnetworkcard => {
        this.networkcardCache.clear();
        return newnetworkcard;
      })
    );
  }
  updateNetworkCard(networkcard: NetworkCard): Observable<NetworkCard> {
    return this.http.put<NetworkCard>(this.baseUrl + 'networkcard/update/' + networkcard.id, networkcard).pipe(
      map(updatednetworkcard => {
        this.networkcardCache.clear();
        return updatednetworkcard;
      })
    );
  }
  deleteNetworkCard(id: number): Observable<void> {
    return this.http.delete<void>(this.baseUrl + 'networkcard/delete/' + id).pipe(
      map(() => {
        this.networkcardCache.clear();
      })
    );
  }
  getRelatedPCs(id: number) {
    return this.http.get<PC[]>(this.baseUrl + 'networkcard/get/related_pc/' + id).pipe(
      map(response => {
        return response;
      })
    );
  }

  // Filter Options
  getFilterBrand(): Observable<string[]> {
    return this.http.get<string[]>(this.baseUrl + 'networkcard/filter/brand').pipe(
      map(response => {
        return response;
      })
    );
  }
  getFilterModel(): Observable<string[]> {
    return this.http.get<string[]>(this.baseUrl + 'networkcard/filter/model').pipe(
      map(response => {
        return response;
      })
    );
  }
}