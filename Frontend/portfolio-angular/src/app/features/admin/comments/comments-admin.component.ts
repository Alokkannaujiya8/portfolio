import { Component, OnInit, signal, computed, inject, ViewChild, TemplateRef } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatDialog } from '@angular/material/dialog';

interface BlogComment {
  id: string;
  authorName: string;
  authorEmail: string;
  content: string;
  postTitle: string;
  date: string;
  status: 'Pending' | 'Approved';
}

@Component({
  selector: 'app-comments-admin',
  standalone: false,
  templateUrl: './comments-admin.component.html',
  styleUrl: './comments-admin.component.scss'
})
export class CommentsAdminComponent implements OnInit {
  private snackBar = inject(MatSnackBar);
  private dialog = inject(MatDialog);

  @ViewChild('replyDialog') replyDialogTemplate!: TemplateRef<any>;
  @ViewChild('detailsDialog') detailsDialogTemplate!: TemplateRef<any>;

  comments = signal<BlogComment[]>([]);
  activeTab = signal<'all' | 'pending' | 'approved'>('all');
  selectedComment = signal<BlogComment | null>(null);

  replyName = 'Admin';
  replyEmail = 'admin@portfolio.com';
  replyText = '';

  ngOnInit(): void {
    this.loadComments();
  }

  loadComments(): void {
    const saved = localStorage.getItem('blog_comments');
    if (saved) {
      this.comments.set(JSON.parse(saved));
    } else {
      const initial: BlogComment[] = [
        {
          id: '1',
          authorName: 'Admin',
          authorEmail: 'admin@portfolio.com',
          content: 'Ok thanks',
          postTitle: 'My Journey as a Full Stack and AI/ML Developer',
          date: '2026-05-15 06:33',
          status: 'Approved'
        },
        {
          id: '2',
          authorName: 'good',
          authorEmail: 'good@gmail.com',
          content: 'ajfdklf',
          postTitle: 'My Journey as a Full Stack and AI/ML Developer',
          date: '2026-03-01 13:15',
          status: 'Approved'
        },
        {
          id: '3',
          authorName: 'a',
          authorEmail: 'asd@gmail.com',
          content: 'fyhbhjlo',
          postTitle: 'My Journey as a Full Stack and AI/ML Developer',
          date: '2026-03-01 12:10',
          status: 'Approved'
        }
      ];
      this.comments.set(initial);
      localStorage.setItem('blog_comments', JSON.stringify(initial));
    }
  }

  pendingCount = computed(() => this.comments().filter(c => c.status === 'Pending').length);
  approvedCount = computed(() => this.comments().filter(c => c.status === 'Approved').length);

  filteredComments = computed(() => {
    const tab = this.activeTab();
    if (tab === 'pending') {
      return this.comments().filter(c => c.status === 'Pending');
    } else if (tab === 'approved') {
      return this.comments().filter(c => c.status === 'Approved');
    }
    return this.comments();
  });

  setTab(tab: 'all' | 'pending' | 'approved'): void {
    this.activeTab.set(tab);
  }

  getAvatarLetter(name: string): string {
    return name ? name.charAt(0).toUpperCase() : 'C';
  }

  openReply(comment: BlogComment): void {
    this.selectedComment.set(comment);
    this.replyText = '';
    this.dialog.open(this.replyDialogTemplate, {
      width: '460px',
      panelClass: 'custom-dialog-panel'
    });
  }

  sendReply(): void {
    if (!this.replyText.trim()) return;
    this.dialog.closeAll();
    this.snackBar.open(`Reply sent successfully to ${this.selectedComment()?.authorName}!`, 'Close', { duration: 3000 });
  }

  viewComment(comment: BlogComment): void {
    this.selectedComment.set(comment);
    this.dialog.open(this.detailsDialogTemplate, {
      width: '600px',
      panelClass: 'custom-dialog-panel'
    });
  }

  deleteComment(comment: BlogComment): void {
    if (confirm('Are you sure you want to delete this comment?')) {
      const updated = this.comments().filter(c => c.id !== comment.id);
      this.comments.set(updated);
      localStorage.setItem('blog_comments', JSON.stringify(updated));
      this.snackBar.open('Comment deleted successfully', 'Close', { duration: 2500 });
    }
  }
}
