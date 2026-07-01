import { Component, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { DataService } from '../../../core/services/data.service';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-dashboard',
  standalone: false,
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent implements OnInit {
  private dataService = inject(DataService);
  private fb = inject(FormBuilder);
  private snackBar = inject(MatSnackBar);
  authService = inject(AuthService);

  currentLoginTime = new Date();
  projectsCount = signal<number>(0);
  skillsCount = signal<number>(0);
  unreadMessagesCount = signal<number>(0);
  blogsCount = signal<number>(0);

  heroId: string = '';
  aboutId: string = '';
  messages = signal<any[]>([]);

  heroForm = this.fb.group({
    title: ['', Validators.required],
    subtitle: ['', Validators.required],
    imageUrl: [''],
    resumeUrl: [''],
    primaryButtonText: [''],
    secondaryButtonText: ['']
  });

  aboutForm = this.fb.group({
    title: ['', Validators.required],
    subtitle: [''],
    description: ['', Validators.required],
    imageUrl: [''],
    location: [''],
    email: ['', [Validators.required, Validators.email]],
    phone: [''],
    experienceYears: [0, Validators.min(0)],
    projectsCompleted: [0, Validators.min(0)]
  });

  ngOnInit() {
    this.loadStats();
    this.loadHeroAndAbout();
    this.loadMessages();
  }

  loadStats() {
    this.dataService.getProjects().subscribe(res => this.projectsCount.set(res.length));
    this.dataService.getSkills().subscribe(res => this.skillsCount.set(res.length));
    this.dataService.getAllBlogsAdmin().subscribe(res => this.blogsCount.set(res.length));
  }

  loadHeroAndAbout() {
    this.dataService.getHero().subscribe(res => {
      if (res) {
        this.heroId = res.id;
        this.heroForm.patchValue(res);
      }
    });
    this.dataService.getAbout().subscribe(res => {
      if (res) {
        this.aboutId = res.id;
        this.aboutForm.patchValue(res);
      }
    });
  }

  loadMessages() {
    this.dataService.getMessages().subscribe(res => {
      this.messages.set(res);
      this.unreadMessagesCount.set(res.filter(m => !m.isRead).length);
    });
  }

  onSaveHero() {
    if (this.heroForm.invalid) return;
    const model = { id: this.heroId, ...this.heroForm.value };
    this.dataService.updateHero(model).subscribe({
      next: () => {
        this.snackBar.open('Hero section updated successfully!', 'Close', { duration: 3000 });
        this.loadHeroAndAbout();
      },
      error: () => this.snackBar.open('Failed to update Hero section.', 'Close', { duration: 3000 })
    });
  }

  onSaveAbout() {
    if (this.aboutForm.invalid) return;
    const model = { id: this.aboutId, ...this.aboutForm.value };
    this.dataService.updateAbout(model).subscribe({
      next: () => {
        this.snackBar.open('About section updated successfully!', 'Close', { duration: 3000 });
        this.loadHeroAndAbout();
      },
      error: () => this.snackBar.open('Failed to update About section.', 'Close', { duration: 3000 })
    });
  }

  markAsRead(id: string) {
    this.dataService.markMessageRead(id).subscribe({
      next: () => {
        this.snackBar.open('Message marked as read.', 'Dismiss', { duration: 2000 });
        this.loadMessages();
      }
    });
  }

  logout(): void {
    this.authService.logout();
  }
}
