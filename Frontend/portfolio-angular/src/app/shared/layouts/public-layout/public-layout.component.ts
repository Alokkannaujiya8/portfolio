import { Component, inject, OnInit, signal } from '@angular/core';
import { Router } from '@angular/router';
import { ThemeService } from '../../../core/services/theme.service';
import { DataService } from '../../../core/services/data.service';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-public-layout',
  standalone: false,
  templateUrl: './public-layout.component.html',
  styleUrl: './public-layout.component.scss'
})
export class PublicLayoutComponent implements OnInit {
  themeService = inject(ThemeService);
  authService = inject(AuthService);
  private router = inject(Router);
  private dataService = inject(DataService);
  
  currentYear = new Date().getFullYear();
  about = signal<any>(null);
  hero = signal<any>(null);

  ngOnInit(): void {
    this.dataService.getAbout().subscribe({
      next: (res) => this.about.set(res),
      error: () => {}
    });
    this.dataService.getHero().subscribe({
      next: (res) => this.hero.set(res),
      error: () => {}
    });
  }

  scrollTo(sectionId: string): void {
    if (this.router.url !== '/') {
      this.router.navigate(['/']).then(() => {
        setTimeout(() => this.scrollElement(sectionId), 200);
      });
    } else {
      this.scrollElement(sectionId);
    }
  }

  private scrollElement(id: string): void {
    const el = document.getElementById(id);
    if (el) {
      el.scrollIntoView({ behavior: 'smooth', block: 'start' });
    }
  }

  getLogoName(): string {
    const title = this.hero()?.title || "Hi, I'm Alok";
    const match = title.match(/^(.*i'm\s+)(.*)$/i) || title.match(/^(.*i\s+am\s+)(.*)$/i);
    if (match) {
      return match[2];
    }
    return 'ALOK';
  }
}
