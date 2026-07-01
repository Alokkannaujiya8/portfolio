import { Injectable, inject } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent, HttpErrorResponse } from '@angular/common/http';
import { Observable, catchError, switchMap, throwError } from 'rxjs';
import { AuthService } from '../services/auth.service';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
  private authService = inject(AuthService);

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(req).pipe(
      catchError((error: any) => {
        if (req.url.includes('/auth/refresh') || req.url.includes('/auth/login')) {
          return throwError(() => error);
        }

        if (error instanceof HttpErrorResponse && error.status === 401) {
          return this.authService.refreshToken().pipe(
            switchMap((res) => {
              const retryReq = req.clone({
                setHeaders: {
                  Authorization: `Bearer ${res.accessToken}`
                }
              });
              return next.handle(retryReq);
            }),
            catchError((refreshError) => {
              this.authService.clearSession();
              return throwError(() => refreshError);
            })
          );
        }

        return throwError(() => error);
      })
    );
  }
}
