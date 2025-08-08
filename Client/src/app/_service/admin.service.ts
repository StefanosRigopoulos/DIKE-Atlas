import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, of, tap } from 'rxjs';
import { User } from '../_model/user';

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  // User Management
  userCache = new Map<number, User>();
  usersSubject = new BehaviorSubject<User[] | null>(null);
  getUser(user_id: number): Observable<User> {
    if (this.userCache.has(user_id)) return of(this.userCache.get(user_id)!);
    return this.http.get<User>(this.baseUrl + 'admin/user/get/' + user_id).pipe(
      tap(user => this.userCache.set(user.id, user))
    );
  }
  getUsers(): Observable<User[]> {
    const cachedUsers = this.usersSubject.getValue();
    if (cachedUsers) return of(cachedUsers);
    return this.http.get<User[]>(this.baseUrl + 'admin/user/get/all').pipe(
      tap(users => {
        this.usersSubject.next(users);
        users.forEach(user => this.userCache.set(user.id, user));
      })
    );
  }
  registerUser(user: User, password: string): Observable<User> {
    return this.http.post<User>(this.baseUrl + 'admin/user/register', { ...user, password }).pipe(
      tap(newUser => {
        this.invalidateCache();
      })
    );
  }
  updateUser(user: User, newPassword?: string): Observable<User> {
    return this.http.put<User>(this.baseUrl + 'admin/user/update', { ...user, newPassword }).pipe(
      tap(updatedUser => {
        this.userCache.set(updatedUser.id, updatedUser);
        this.updateUserInCache(updatedUser);
        this.invalidateCache();
      })
    );
  }
  deleteUser(user_id: number) {
    return this.http.delete<void>(this.baseUrl + 'admin/user/delete/' + user_id).pipe(
      tap(() => {
        this.userCache.delete(user_id);
        this.removeUserFromCache(user_id);
      })
    );
  }
  private invalidateCache() {
    this.usersSubject.next(null);
    this.userCache.clear();
  }
  private updateUserInCache(updatedUser: User) {
    const cachedUsers = this.usersSubject.getValue();
    if (cachedUsers) {
      const updatedUsers = cachedUsers.map(user =>
        user.id === updatedUser.id ? updatedUser : user
      );
      this.usersSubject.next(updatedUsers);
    }
  }
  private removeUserFromCache(user_id: number) {
    const cachedUsers = this.usersSubject.getValue();
    if (cachedUsers) {
      this.usersSubject.next(cachedUsers.filter(user => user.id !== user_id));
    }
  }

  // Database Management
  getDatabase() {
    return this.http.get(this.baseUrl + 'admin/database/export/', { responseType: 'blob' });
  }
  getTable(tableName: string) {
    return this.http.get(this.baseUrl + 'admin/database/export/' + tableName, { responseType: 'blob' });
  }

  // Backup
  exportDatabase() {
    return this.http.get(this.baseUrl + 'admin/backup/export', { responseType: 'blob' });
  }
  importDatabase(file: File) {
    if (!file) return;
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<void>(this.baseUrl + 'admin/backup/import', formData).subscribe({
      next: () => console.log("Data imported successfully!"),
      error: (err) => console.error("Data update failed:", err)
    });
  }

  // Miscellaneous
  uploadBackgroundImage(file: File) {
    if (!file) return;
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<void>(this.baseUrl + 'admin/misc/background', formData).subscribe({
      next: () => console.log("Upload background successfully!"),
      error: (err) => console.error("Upload failed:", err)
    });
  }
}
