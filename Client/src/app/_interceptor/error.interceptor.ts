import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor, HttpErrorResponse } from '@angular/common/http';
import { catchError, Observable, skip } from 'rxjs';
import { MatDialog } from '@angular/material/dialog';
import { ErrorModalComponent } from '../_modals/error-modal/error-modal.component';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {

  constructor(private dialog: MatDialog) {}

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const skipModal = request.headers.has('X-Skip-Error-Modal');
    return next.handle(request).pipe(
      catchError((error: HttpErrorResponse) => {
        if (!skipModal) {
          let errorMessage;
          switch (error.status) {
            case 400:
              errorMessage = 'Bad Request';
              break;
            case 401:
              errorMessage = 'Unauthorised';
              break;
            case 404:
              errorMessage = 'Not Found';
              break;
            case 500:
              errorMessage = 'Internal Server Error';
              break;
            default:
              errorMessage = 'Unknown Error';
              break;
          }
          let errorDetails = 'No further details are provided';
          if (error.error && error.error.details) {
            errorDetails = error.error.details;
          }
          this.openErrorModal(error.status, errorMessage, errorDetails);
        }
        throw error;
      })
    )
  }

  private openErrorModal(status: number, message: string, details: string): void {
    this.dialog.open(ErrorModalComponent, {
      width: '400px',
      data: {
        title: 'Error Notification',
        status,
        message,
        details,
      },
      disableClose: true,
    });
  }
}