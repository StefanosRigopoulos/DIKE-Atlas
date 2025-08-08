import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { AccountService } from '../_service/account.service';

@Injectable()
export class JwtInterceptor implements HttpInterceptor {
  private accountService = inject(AccountService);

  constructor() {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    if (this.accountService.currentUser()) {
      request = request.clone({
        setHeaders: {
          Authorization: `Bearer ${this.accountService.currentUser()?.token}`
        }
      })
    }
    return next.handle(request);
  }
}