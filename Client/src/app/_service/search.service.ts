import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { SearchResult } from '../_model/search';

@Injectable({
  providedIn: 'root'
})
export class SearchService {
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  search(searchTerm: string): Observable<SearchResult[]> {
    return this.http.get<SearchResult[]>(this.baseUrl + "search/?searchTerm=" + searchTerm, {
      headers: new HttpHeaders({ 'X-Skip-Error-Modal': 'true' })
    });
  }
}