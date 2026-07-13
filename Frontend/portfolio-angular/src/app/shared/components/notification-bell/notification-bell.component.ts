import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { NotificationService, NotificationDto } from '../../../core/services/notification.service';

@Component({
  selector: 'app-notification-bell',
  standalone: false,
  templateUrl: './notification-bell.component.html',
  styleUrls: ['./notification-bell.component.scss']
})
export class NotificationBellComponent {
  public notificationService = inject(NotificationService);
  private router = inject(Router);

  get unreadCount() {
    return this.notificationService.unreadCount();
  }

  get latestNotifications() {
    return this.notificationService.latestNotifications();
  }

  getIcon(type: string): string {
    switch (type) {
      case 'Success': return 'check_circle';
      case 'Error': return 'error';
      case 'Warning': return 'warning';
      default: return 'info';
    }
  }

  getIconColor(type: string): string {
    switch (type) {
      case 'Success': return '#10b981'; // Green
      case 'Error': return '#ef4444'; // Red
      case 'Warning': return '#f59e0b'; // Amber
      default: return '#3b82f6'; // Blue
    }
  }

  onNotificationClick(notification: NotificationDto): void {
    if (!notification.isRead) {
      this.notificationService.markAsRead(notification.id).subscribe();
    }
    if (notification.redirectUrl) {
      this.router.navigateByUrl(notification.redirectUrl);
    }
  }

  markAsRead(event: Event, id: string): void {
    event.stopPropagation(); // Avoid closing the dropdown menu
    this.notificationService.markAsRead(id).subscribe();
  }

  markAllAsRead(event: Event): void {
    event.stopPropagation();
    this.notificationService.markAllRead().subscribe();
  }

  deleteNotification(event: Event, id: string): void {
    event.stopPropagation();
    this.notificationService.deleteNotification(id).subscribe();
  }

  viewAll(): void {
    this.router.navigate(['/admin/notifications']);
  }

  getTimeAgo(dateString: string): string {
    const date = new Date(dateString);
    const now = new Date();
    const seconds = Math.floor((now.getTime() - date.getTime()) / 1000);

    if (seconds < 60) return 'Just now';
    const minutes = Math.floor(seconds / 60);
    if (minutes < 60) return `${minutes}m ago`;
    const hours = Math.floor(minutes / 60);
    if (hours < 24) return `${hours}h ago`;
    const days = Math.floor(hours / 24);
    if (days === 1) return 'Yesterday';
    return `${days}d ago`;
  }
}
