import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class FileService {
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}
  
  getPCReport(id: number) {
    return this.http.get(this.baseUrl + 'file/get/report/' + id, { responseType: 'blob' });
  }
}
