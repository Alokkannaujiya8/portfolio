import { Injectable, signal, inject, effect } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AuthService } from './auth.service';
import { ToastrService } from 'ngx-toastr';
import * as signalR from '@microsoft/signalr';

export interface NotificationDto {
  id: string;
  userId?: string;
  title: string;
  message: string;
  type: string; // Success, Error, Warning, Info
  isRead: boolean;
  createdAt: string;
  readAt?: string;
  redirectUrl?: string;
  icon?: string;
  createdBy?: string;
}

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  private http = inject(HttpClient);
  private authService = inject(AuthService);
  private toastr = inject(ToastrService);
  
  private apiUrl = `${environment.apiUrl}/notifications`;
  private hubUrl = environment.apiUrl.replace('/api/v1', '/hubs/notifications');
  
  private hubConnection: signalR.HubConnection | null = null;
  
  // State Signals
  unreadCount = signal<number>(0);
  latestNotifications = signal<NotificationDto[]>([]);
  
  constructor() {
    // Start SignalR connection when the user is logged in, and stop it when logged out
    effect(() => {
      if (this.authService.isAuthenticated()) {
        this.loadInitialData();
        this.startSignalRConnection();
      } else {
        this.stopSignalRConnection();
        this.unreadCount.set(0);
        this.latestNotifications.set([]);
      }
    });
  }
  
  public loadInitialData(): void {
    this.fetchUnreadCount().subscribe();
    this.fetchLatest(5).subscribe();
  }
  
  private startSignalRConnection(): void {
    if (this.hubConnection) return;
    
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(this.hubUrl, {
        accessTokenFactory: () => this.authService.getAccessToken() || ''
      })
      .withAutomaticReconnect()
      .build();
      
    this.hubConnection.on('ReceiveNotification', (notification: NotificationDto) => {
      this.showToast(notification);
      // Auto-refresh states after SignalR event
      this.loadInitialData();
    });
    
    this.hubConnection.start()
      .then(() => console.log('SignalR connection established successfully.'))
      .catch(err => console.error('Error starting SignalR connection:', err));
  }
  
  private stopSignalRConnection(): void {
    if (this.hubConnection) {
      this.hubConnection.stop()
        .then(() => {
          this.hubConnection = null;
          console.log('SignalR connection stopped.');
        })
        .catch(err => console.error('Error stopping SignalR connection:', err));
    }
  }
  
  private showToast(notification: NotificationDto): void {
    const title = notification.title;
    const msg = notification.message;
    switch (notification.type) {
      case 'Success':
        this.toastr.success(msg, title);
        break;
      case 'Error':
        this.toastr.error(msg, title);
        break;
      case 'Warning':
        this.toastr.warning(msg, title);
        break;
      default:
        this.toastr.info(msg, title);
        break;
    }
  }
  
  // REST API Methods
  getNotifications(page: number, size: number, type?: string, isRead?: boolean): Observable<NotificationDto[]> {
    let params = new HttpParams()
      .set('pageNumber', page.toString())
      .set('pageSize', size.toString());
      
    if (type) {
      params = params.set('type', type);
    }
    if (isRead !== undefined) {
      params = params.set('isRead', isRead.toString());
    }
    
    return this.http.get<NotificationDto[]>(this.apiUrl, { params });
  }
  
  fetchUnreadCount(): Observable<{ count: number }> {
    return this.http.get<{ count: number }>(`${this.apiUrl}/unread-count`).pipe(
      tap(res => this.unreadCount.set(res.count))
    );
  }
  
  fetchLatest(count: number = 5): Observable<NotificationDto[]> {
    return this.http.get<NotificationDto[]>(`${this.apiUrl}/latest?count=${count}`).pipe(
      tap(res => this.latestNotifications.set(res))
    );
  }
  
  markAsRead(id: string): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}/mark-read`, {}).pipe(
      tap(() => this.loadInitialData())
    );
  }
  
  markAllRead(): Observable<any> {
    return this.http.put(`${this.apiUrl}/mark-all-read`, {}).pipe(
      tap(() => this.loadInitialData())
    );
  }
  
  deleteNotification(id: string): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`).pipe(
      tap(() => this.loadInitialData())
    );
  }
}
