import { Component, inject, OnInit, signal, HostListener } from '@angular/core';
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
  
  isScrolled = signal<boolean>(false);
  activeSection = signal<string>('hero');

  private sections = ['hero', 'about', 'skills', 'projects', 'experience', 'blogs', 'gallery', 'contact'];

  ngOnInit(): void {
    this.dataService.getAbout().subscribe({
      next: (res) => this.about.set(res),
      error: () => {}
    });
    this.dataService.getHero().subscribe({
      next: (res) => this.hero.set(res),
      error: () => {}
    });
    
    this.onWindowScroll();
  }

  @HostListener('window:scroll')
  onWindowScroll(): void {
    const scrollPosition = window.scrollY || document.documentElement.scrollTop || 0;
    this.isScrolled.set(scrollPosition > 20);
    
    if (this.router.url === '/' || this.router.url === '') {
      this.updateActiveSection(scrollPosition);
    }
  }

  private updateActiveSection(scrollPosition: number): void {
    const offset = 140;
    for (let i = this.sections.length - 1; i >= 0; i--) {
      const sectionId = this.sections[i];
      const el = document.getElementById(sectionId);
      if (el) {
        const top = el.offsetTop - offset;
        if (scrollPosition >= top) {
          this.activeSection.set(sectionId);
          break;
        }
      }
    }
  }

  scrollTo(sectionId: string): void {
    this.activeSection.set(sectionId);
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
