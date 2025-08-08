import { HTTP_INTERCEPTORS, provideHttpClient, withInterceptorsFromDi } from "@angular/common/http";
import { ApplicationConfig, importProvidersFrom } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { BrowserModule } from "@angular/platform-browser";
import { BrowserAnimationsModule, provideAnimations } from "@angular/platform-browser/animations";
import { provideRouter } from "@angular/router";
import { ModalModule } from "ngx-bootstrap/modal";
import { NgxSpinnerModule } from "ngx-spinner";
import { routes } from "./app.routes";
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { JwtInterceptor } from "./_interceptor/jwt.interceptor";
import { LoadingInterceptor } from "./_interceptor/loading.interceptor";
import { ErrorInterceptor } from "./_interceptor/error.interceptor";

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes),
    importProvidersFrom(
      BrowserModule,
      FormsModule,
      ReactiveFormsModule,
      BrowserAnimationsModule,
      NgxSpinnerModule.forRoot({ type: 'line-spin-clockwise-fade' }),
      ModalModule.forRoot()
    ),
    { provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: LoadingInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true },
    provideHttpClient(withInterceptorsFromDi()),
    provideAnimations(),
    provideAnimationsAsync()
  ]
};
