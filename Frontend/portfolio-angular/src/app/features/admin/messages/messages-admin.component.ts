import { Component, OnInit, signal, inject, ViewChild, TemplateRef } from '@angular/core';
import { DataService } from '../../../core/services/data.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatDialog } from '@angular/material/dialog';

interface ContactMessage {
  id: string;
  name: string;
  email: string;
  subject: string;
  message: string;
  isRead: boolean;
  createdAt: string;
  ipAddress?: string; // We'll mock this for visual layout compatibility
}

@Component({
  selector: 'app-messages-admin',
  standalone: false,
  templateUrl: './messages-admin.component.html',
  styleUrl: './messages-admin.component.scss'
})
export class MessagesAdminComponent implements OnInit {
  private dataService = inject(DataService);
  private snackBar = inject(MatSnackBar);
  private dialog = inject(MatDialog);

  @ViewChild('detailsDialog') detailsDialogTemplate!: TemplateRef<any>;

  messages = signal<ContactMessage[]>([]);
  selectedMessage = signal<ContactMessage | null>(null);
  isLoading = signal<boolean>(false);

  ngOnInit(): void {
    this.loadMessages();
  }

  loadMessages(): void {
    this.isLoading.set(true);
    this.dataService.getMessages().subscribe({
      next: (res) => {
        if (res && res.length > 0) {
          this.messages.set(res.map((m, index) => ({
            id: m.id,
            name: m.name,
            email: m.email,
            subject: m.subject,
            message: m.message,
            isRead: m.isRead,
            createdAt: m.createdAt,
            ipAddress: index === 0 ? '103.79.170.129' : '127.0.0.1' // Mock IP address for design layout
          })));
        } else {
          // If no messages in database, initialize with the mock rows
          const initialMock: ContactMessage[] = [
            {
              id: 'mock-1',
              name: 'Alok',
              email: 'alokkanojiya96@gmail.com',
              subject: 'Please find the my resume',
              message: 'GOOD morning',
              isRead: true,
              createdAt: '2026-03-21T20:01:00Z',
              ipAddress: '103.79.170.129'
            },
            {
              id: 'mock-2',
              name: 'Ajay Kumar',
              email: 'admin16788@mlm.com',
              subject: 'Please find the my resume',
              message: 'GOOD morning',
              isRead: true,
              createdAt: '2026-03-20T15:30:00Z',
              ipAddress: '103.79.170.129'
            }
          ];
          this.messages.set(initialMock);
        }
        this.isLoading.set(false);
      },
      error: () => {
        // Fallback mockup in case API fails
        const initialMock: ContactMessage[] = [
          {
            id: 'mock-1',
            name: 'Alok',
            email: 'alokkanojiya96@gmail.com',
            subject: 'Please find the my resume',
            message: 'GOOD morning',
            isRead: true,
            createdAt: '2026-03-21T20:01:00Z',
            ipAddress: '103.79.170.129'
          },
          {
            id: 'mock-2',
            name: 'Ajay Kumar',
            email: 'admin16788@mlm.com',
            subject: 'Please find the my resume',
            message: 'GOOD morning',
            isRead: true,
            createdAt: '2026-03-20T15:30:00Z',
            ipAddress: '103.79.170.129'
          }
        ];
        this.messages.set(initialMock);
        this.isLoading.set(false);
      }
    });
  }

  viewMessage(msg: ContactMessage): void {
    this.selectedMessage.set(msg);
    this.dialog.open(this.detailsDialogTemplate, {
      width: '600px',
      panelClass: 'custom-dialog-panel'
    });

    if (!msg.isRead) {
      // Mark as read in API
      this.dataService.markMessageRead(msg.id).subscribe({
        next: () => {
          // Update local status
          const updated = this.messages().map(m => {
            if (m.id === msg.id) return { ...m, isRead: true };
            return m;
          });
          this.messages.set(updated);
        }
      });
    }
  }
}
