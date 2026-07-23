import { Component, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { DataService } from '../../../core/services/data.service';

@Component({
  selector: 'app-profile-admin',
  standalone: false,
  templateUrl: './profile-admin.component.html',
  styleUrl: './profile-admin.component.scss'
})
export class ProfileAdminComponent implements OnInit {
  private dataService = inject(DataService);
  private fb = inject(FormBuilder);
  private snackBar = inject(MatSnackBar);

  heroId: string = '';
  aboutId: string = '';
  isLoading = signal<boolean>(false);

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

  ngOnInit(): void {
    this.loadHeroAndAbout();
  }

  loadHeroAndAbout(): void {
    this.isLoading.set(true);
    this.dataService.getHero().subscribe({
      next: (res) => {
        if (res) {
          this.heroId = res.id;
          this.heroForm.patchValue(res);
        }
      }
    });
    this.dataService.getAbout().subscribe({
      next: (res) => {
        if (res) {
          this.aboutId = res.id;
          this.aboutForm.patchValue(res);
        }
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  onSaveHero(): void {
    if (this.heroForm.invalid) return;
    this.isLoading.set(true);
    const model = { id: this.heroId, ...this.heroForm.value };
    this.dataService.updateHero(model).subscribe({
      next: () => {
        this.isLoading.set(false);
        this.snackBar.open('Hero section updated successfully!', 'Close', { duration: 3000 });
        this.loadHeroAndAbout();
      },
      error: () => {
        this.isLoading.set(false);
        this.snackBar.open('Failed to update Hero section.', 'Close', { duration: 3000 });
      }
    });
  }

  onSaveAbout(): void {
    if (this.aboutForm.invalid) return;
    this.isLoading.set(true);
    const model = { id: this.aboutId, ...this.aboutForm.value };
    this.dataService.updateAbout(model).subscribe({
      next: () => {
        this.isLoading.set(false);
        this.snackBar.open('About section updated successfully!', 'Close', { duration: 3000 });
        this.loadHeroAndAbout();
      },
      error: () => {
        this.isLoading.set(false);
        this.snackBar.open('Failed to update About section.', 'Close', { duration: 3000 });
      }
    });
  }

  getFullUrl(relativePath: string | null | undefined): string {
    if (!relativePath) return 'profile.jpg';
    if (relativePath.startsWith('http://') || relativePath.startsWith('https://')) return relativePath;
    const cleanPath = relativePath.startsWith('/') ? relativePath.substring(1) : relativePath;
    return `/${cleanPath}`;
  }
}
