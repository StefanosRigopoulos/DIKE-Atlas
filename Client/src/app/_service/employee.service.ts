import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { map, Observable, of, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { getPaginatedResult, getPaginationHeaders } from '../_helper/paginationHelper';
import { Employee, EmployeeOnly } from '../_model/employee';
import { EmployeeParams } from '../_model/params';
import { HttpClient } from '@angular/common/http';
import { PC } from '../_model/pc';

@Injectable({
  providedIn: 'root'
})
export class EmployeeService {
  baseUrl = environment.apiUrl;
  employees: EmployeeOnly[] = [];
  employeeCache = new Map();
  employeeParams: EmployeeParams = new EmployeeParams();

  constructor(private http: HttpClient) {}

  getEmployeeParams() {
    return this.employeeParams;
  }
  setEmployeeParams(employeeParams: EmployeeParams) {
    this.employeeParams = employeeParams;
  }
  resetEmployeeParams() {
    this.employeeParams = new EmployeeParams();
    return this.employeeParams;
  }
  getEmployee(id: number): Observable<Employee> {
    return this.http.get<Employee>(this.baseUrl + 'employee/get/' + id).pipe(
      map(response => {
        return response;
      })
    );
  }
  getEmployees() {
    return this.http.get<Employee[]>(this.baseUrl + 'employee/get/all').pipe(
      map(response => {
        return response;
      })
    );
  }
  getProfilePhoto(id: number): Observable<Blob> {
    return this.http.get(this.baseUrl + 'employee/get/' + id + '/photo', { responseType: 'blob' });
  }
  getPagedEmployees(employeeParams: EmployeeParams) {
    const response = this.employeeCache.get(Object.values(employeeParams).join('-'));
    if (response) return of(response);
    let params = getPaginationHeaders(employeeParams.pageNumber, employeeParams.pageSize);
    params = params.append('rank', employeeParams.rank);
    params = params.append('speciality', employeeParams.speciality);
    params = params.append('firstname', employeeParams.firstName);
    params = params.append('lastname', employeeParams.lastName);
    params = params.append('unit', employeeParams.unit);
    params = params.append('office', employeeParams.office);
    params = params.append('orderBy', employeeParams.orderBy);
    return getPaginatedResult<EmployeeOnly[]>(this.baseUrl + 'employee/get/all/paged', params, this.http).pipe(
      map(response => {
        this.employeeCache.set(Object.values(employeeParams).join('-'), response);
        return response;
      })
    );
  }
  addEmployee(employee: Employee, file: File | null): Observable<Employee> {
    const formData = new FormData();
    if (employee.am) formData.append('am', employee.am);
    if (employee.rank) formData.append('rank', employee.rank);
    if (employee.speciality) formData.append('speciality', employee.speciality);
    if (employee.firstName) formData.append('firstName', employee.firstName);
    if (employee.lastName) formData.append('lastName', employee.lastName);
    if (employee.unit) formData.append('unit', employee.unit);
    if (employee.office) formData.append('office', employee.office);
    if (employee.position) formData.append('position', employee.position);
    if (employee.pcUsername) formData.append('pcUsername', employee.pcUsername);
    if (employee.shdaedUsername) formData.append('shdaedUsername', employee.shdaedUsername);
    if (employee.phone) formData.append('phone', employee.phone);
    if (employee.mobile) formData.append('mobile', employee.mobile);
    if (employee.email) formData.append('email', employee.email);
    if (file) formData.append('file', file);
    return this.http.post<Employee>(this.baseUrl + 'employee/add', formData).pipe(
      map(newEmployee => {
        this.employeeCache.clear();
        return newEmployee;
      })
    );
  }
  updateEmployee(employee: Employee, file: File | null): Observable<Employee> {
    const formData = new FormData();
    if (employee.am) formData.append('am', employee.am);
    if (employee.rank) formData.append('rank', employee.rank);
    if (employee.speciality) formData.append('speciality', employee.speciality);
    if (employee.firstName) formData.append('firstName', employee.firstName);
    if (employee.lastName) formData.append('lastName', employee.lastName);
    if (employee.unit) formData.append('unit', employee.unit);
    if (employee.office) formData.append('office', employee.office);
    if (employee.position) formData.append('position', employee.position);
    if (employee.pcUsername) formData.append('pcUsername', employee.pcUsername);
    if (employee.shdaedUsername) formData.append('shdaedUsername', employee.shdaedUsername);
    if (employee.phone) formData.append('phone', employee.phone);
    if (employee.mobile) formData.append('mobile', employee.mobile);
    if (employee.email) formData.append('email', employee.email);
    if (employee.photoPath) formData.append('photoPath', employee.photoPath);
    if (file) formData.append('file', file);
    return this.http.put<Employee>(this.baseUrl + 'employee/update/' + employee.id, formData).pipe(
      map(updatedemployee => {
        this.employeeCache.clear();
        return updatedemployee;
      })
    );
  }
  deleteEmployee(id: number): Observable<void> {
    return this.http.delete<void>(this.baseUrl + 'employee/delete/' + id).pipe(
      map(() => {
        this.employeeCache.clear();
      })
    );
  }

  // PC Relationship
  assignPC(employee_id: number, pc_id: number): Observable<PC> {
    return this.http.put<PC>(`${this.baseUrl}employee/pc_assign/${employee_id}/${pc_id}`, null).pipe(
      map(response => {
        return response;
      })
    );
  }
  getRelatedPCs(employee_id: number): Observable<PC[]> {
    return this.http.get<PC[]>(this.baseUrl + 'employee/pc_get/' + employee_id).pipe(
      map(response => {
        return response;
      })
    );
  }
  removePC(employee_id: number, pc_id: number) {
    return this.http.delete(`${this.baseUrl}employee/pc_remove/${employee_id}/${pc_id}`);
  }

  // Filter Options
  getFilterUnit(): Observable<string[]> {
    return this.http.get<string[]>(this.baseUrl + 'employee/filter/unit').pipe(
      map(response => {
        return response;
      })
    );
  }
  getFilterOffice(): Observable<string[]> {
    return this.http.get<string[]>(this.baseUrl + 'employee/filter/office').pipe(
      map(response => {
        return response;
      })
    );
  }
  getFilterRank(): Observable<string[]> {
    return this.http.get<string[]>(this.baseUrl + 'employee/filter/rank').pipe(
      map(response => {
        return response;
      })
    );
  }
  getFilterSpeciality(): Observable<string[]> {
    return this.http.get<string[]>(this.baseUrl + 'employee/filter/speciality').pipe(
      map(response => {
        return response;
      })
    );
  }
}
