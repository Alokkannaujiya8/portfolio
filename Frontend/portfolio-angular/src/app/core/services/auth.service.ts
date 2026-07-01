import { Injectable, signal, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, tap, catchError, throwError } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface AuthResponse {
  accessToken: string;
  refreshToken: string;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private http = inject(HttpClient);
  private router = inject(Router);
  private apiUrl = environment.apiUrl;

  isAuthenticated = signal<boolean>(this.hasToken());
  currentUserEmail = signal<string | null>(this.getEmailFromToken());

  private hasToken(): boolean {
    return !!localStorage.getItem('accessToken');
  }

  private getEmailFromToken(): string | null {
    const token = localStorage.getItem('accessToken');
    if (!token) return null;
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      return payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'] || payload.email || null;
    } catch {
      return null;
    }
  }

  getAccessToken(): string | null {
    return localStorage.getItem('accessToken');
  }

  getRefreshToken(): string | null {
    return localStorage.getItem('refreshToken');
  }

  login(credentials: { email: string; password: string }): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/auth/login`, credentials).pipe(
      tap(res => {
        localStorage.setItem('accessToken', res.accessToken);
        localStorage.setItem('refreshToken', res.refreshToken);
        this.isAuthenticated.set(true);
        this.currentUserEmail.set(this.getEmailFromToken());
      })
    );
  }

  changeCredentials(credentials: any): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/auth/change-credentials`, credentials);
  }

  logout(): void {
    const accessToken = this.getAccessToken();
    const refreshToken = this.getRefreshToken();
    if (accessToken && refreshToken) {
      this.http.post(`${this.apiUrl}/auth/logout`, {}).subscribe({
        next: () => this.clearSession(),
        error: () => this.clearSession()
      });
    } else {
      this.clearSession();
    }
  }

  clearSession(): void {
    localStorage.removeItem('accessToken');
    localStorage.removeItem('refreshToken');
    this.isAuthenticated.set(false);
    this.currentUserEmail.set(null);
    this.router.navigate(['/login']);
  }

  refreshToken(): Observable<AuthResponse> {
    const accessToken = this.getAccessToken();
    const refreshToken = this.getRefreshToken();
    return this.http.post<AuthResponse>(`${this.apiUrl}/auth/refresh`, { accessToken, refreshToken }).pipe(
      tap(res => {
        localStorage.setItem('accessToken', res.accessToken);
        localStorage.setItem('refreshToken', res.refreshToken);
        this.isAuthenticated.set(true);
        this.currentUserEmail.set(this.getEmailFromToken());
      }),
      catchError(err => {
        this.clearSession();
        return throwError(() => err);
      })
    );
  }
}
