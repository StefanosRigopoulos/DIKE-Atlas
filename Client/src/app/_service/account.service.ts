import { computed, effect, Injectable, OnInit, signal } from '@angular/core';
import { User } from '../_model/user';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { map } from 'rxjs';

@Injectable({
  providedIn: 'root'
})

export class AccountService{
  baseUrl = environment.apiUrl;
  currentUser = signal<User | null>(null);
  role = computed(() => {
    const user = this.currentUser();
    try {
      if (user?.token) {
        const payload = JSON.parse(atob(user.token.split('.')[1]));
        return payload?.role ?? null;
      }
    } catch (e) {
      console.error('Failed to parse role from token', e);
    }
    return null;
  });

  constructor(private http: HttpClient) {}

  login(model: any) {
    return this.http.post<User>(this.baseUrl + 'account/login', model, {
      headers: new HttpHeaders({ 'X-Skip-Error-Modal': 'true' })
    }).pipe(
      map((response: User) => {
        const user = response;
        if (user) {
          this.setCurrentUser(user);
        }
      })
    )
  }
  setCurrentUser(user: User) {
    localStorage.setItem('user', JSON.stringify(user));
    this.currentUser.set(user);
  }
  logout() {
    localStorage.removeItem('user');
    this.currentUser.set(null);
  }
}