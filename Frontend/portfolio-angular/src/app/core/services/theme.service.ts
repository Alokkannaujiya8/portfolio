import { Injectable, signal, effect } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ThemeService {
  isDarkMode = signal<boolean>(this.getInitialTheme());

  constructor() {
    effect(() => {
      const mode = this.isDarkMode();
      localStorage.setItem('theme', mode ? 'dark' : 'light');
      if (mode) {
        document.body.classList.add('dark-theme');
      } else {
        document.body.classList.remove('dark-theme');
      }
    });
  }

  toggleTheme() {
    this.isDarkMode.update(mode => !mode);
  }

  private getInitialTheme(): boolean {
    const saved = localStorage.getItem('theme');
    if (saved) return saved === 'dark';
    return typeof window !== 'undefined' && window.matchMedia('(prefers-color-scheme: dark)').matches;
  }
}
