import { Component, OnInit, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { NotificationService, NotificationDto } from '../../../core/services/notification.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-notifications-list',
  standalone: false,
  templateUrl: './notifications-list.component.html',
  styleUrls: ['./notifications-list.component.scss']
})
export class NotificationsListComponent implements OnInit {
  private notificationService = inject(NotificationService);
  private router = inject(Router);
  private toastr = inject(ToastrService);

  // Table Config
  displayedColumns: string[] = ['type', 'title', 'message', 'createdAt', 'status', 'actions'];
  notifications = signal<NotificationDto[]>([]);
  isLoading = signal<boolean>(false);

  // Pagination State
  pageNumber = 1;
  pageSize = 10;
  totalItems = 100; // Mock total or dynamic

  // Filtering State
  selectedType: string = '';
  selectedReadStatus: string = '';

  types: string[] = ['Success', 'Error', 'Warning', 'Info'];

  ngOnInit(): void {
    this.loadNotifications();
  }

  loadNotifications(): void {
    this.isLoading.set(true);
    let isReadFilter: boolean | undefined = undefined;
    
    if (this.selectedReadStatus === 'Read') {
      isReadFilter = true;
    } else if (this.selectedReadStatus === 'Unread') {
      isReadFilter = false;
    }

    this.notificationService.getNotifications(
      this.pageNumber,
      this.pageSize,
      this.selectedType || undefined,
      isReadFilter
    ).subscribe({
      next: (res) => {
        this.notifications.set(res);
        this.isLoading.set(false);
        // Standard pagination total approximation or length
        if (res.length < this.pageSize && this.pageNumber === 1) {
          this.totalItems = res.length;
        } else {
          // Adjust total dynamically if we had headers, else provide standard pagination
          this.totalItems = this.pageNumber * this.pageSize + (res.length === this.pageSize ? this.pageSize : 0);
        }
      },
      error: () => {
        this.isLoading.set(false);
        this.toastr.error('Failed to load notifications.', 'Error');
      }
    });
  }

  onPageChange(event: any): void {
    this.pageNumber = event.pageIndex + 1;
    this.pageSize = event.pageSize;
    this.loadNotifications();
  }

  onFilterChange(): void {
    this.pageNumber = 1;
    this.loadNotifications();
  }

  markAsRead(notification: NotificationDto): void {
    this.notificationService.markAsRead(notification.id).subscribe({
      next: () => {
        this.toastr.success('Notification marked as read.', 'Success');
        this.loadNotifications();
      }
    });
  }

  markAllAsRead(): void {
    this.notificationService.markAllRead().subscribe({
      next: () => {
        this.toastr.success('All notifications marked as read.', 'Success');
        this.loadNotifications();
      }
    });
  }

  deleteNotification(id: string): void {
    this.notificationService.deleteNotification(id).subscribe({
      next: () => {
        this.toastr.success('Notification deleted.', 'Success');
        this.loadNotifications();
      }
    });
  }

  onRowClick(notification: NotificationDto): void {
    if (!notification.isRead) {
      this.notificationService.markAsRead(notification.id).subscribe();
    }
    if (notification.redirectUrl) {
      this.router.navigateByUrl(notification.redirectUrl);
    }
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
      case 'Success': return '#10b981';
      case 'Error': return '#ef4444';
      case 'Warning': return '#f59e0b';
      default: return '#3b82f6';
    }
  }
}
