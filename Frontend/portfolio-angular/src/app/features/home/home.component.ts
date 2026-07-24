

import { Component, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { DataService } from '../../core/services/data.service';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-home',
  standalone: false,
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss'
})
export class HomeComponent implements OnInit {
  private dataService = inject(DataService);
  private fb = inject(FormBuilder);
  private snackBar = inject(MatSnackBar);

  hero = signal<any>(null);
  about = signal<any>(null);
  services = signal<any[]>([]);
  skills = signal<any[]>([]);
  skillCategories = signal<string[]>([]);
  experiences = signal<any[]>([]);
  educations = signal<any[]>([]);
  certificates = signal<any[]>([]);
  projects = signal<any[]>([]);
  blogs = signal<any[]>([]);
  gallery = signal<any[]>([]);
  githubRepos = signal<any[]>([]);

  isSubmitting = signal<boolean>(false);
  downloadResumeUrl = this.dataService.downloadResumeUrl();

  roles: string[] = ['ASP.NET Developer', 'Problem Solver', 'Web Developer'];
  currentRoleIndex = 0;
  currentCharIndex = 0;
  isDeleting = false;
  displayedRole = signal<string>('');

  contactForm = this.fb.group({
    name: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
    subject: [''],
    message: ['', Validators.required]
  });

  ngOnInit() {
    this.loadData();
  }

  loadData() {
    this.dataService.getHero().subscribe(res => {
      this.hero.set(res);
      if (res?.subtitle) {
        this.roles = [res.subtitle, 'Problem Solver', 'Web Developer'];
      }
      this.startTypewriterEffect();
    });
    this.dataService.getAbout().subscribe(res => this.about.set(res));
    this.dataService.getServices().subscribe(res => this.services.set(res));
    this.dataService.getExperiences().subscribe(res => this.experiences.set(res));
    this.dataService.getEducations().subscribe(res => this.educations.set(res));
    this.dataService.getCertificates().subscribe(res => this.certificates.set(res));
    this.dataService.getProjects().subscribe(res => this.projects.set(res));
    this.dataService.getBlogs().subscribe(res => this.blogs.set(res.slice(0, 3)));
    this.dataService.getGallery().subscribe(res => this.gallery.set(res));
    this.dataService.getGithubRepos('Alokkannaujiya8').subscribe({
      next: (res) => {
        const sorted = res.sort((a, b) => b.stargazers_count - a.stargazers_count || a.name.localeCompare(b.name));
        this.githubRepos.set(sorted.slice(0, 6));
      },
      error: () => console.log('Failed to fetch GitHub repos')
    });
    
    this.dataService.getSkills().subscribe(res => {
      this.skills.set(res);
      const categories = Array.from(new Set(res.map(s => s.category)));
      this.skillCategories.set(categories as string[]);
    });
  }

  startTypewriterEffect() {
    if (!this.roles || this.roles.length === 0) return;
    const currentFullText = this.roles[this.currentRoleIndex];
    
    if (this.isDeleting) {
      this.displayedRole.set(currentFullText.substring(0, this.currentCharIndex - 1));
      this.currentCharIndex--;
    } else {
      this.displayedRole.set(currentFullText.substring(0, this.currentCharIndex + 1));
      this.currentCharIndex++;
    }

    let typingSpeed = this.isDeleting ? 50 : 100;

    if (!this.isDeleting && this.currentCharIndex === currentFullText.length) {
      typingSpeed = 2000; // Pause at end of word
      this.isDeleting = true;
    } else if (this.isDeleting && this.currentCharIndex === 0) {
      this.isDeleting = false;
      this.currentRoleIndex = (this.currentRoleIndex + 1) % this.roles.length;
      typingSpeed = 500; // Pause before typing next word
    }

    setTimeout(() => this.startTypewriterEffect(), typingSpeed);
  }

  getSkillsByCategory(category: string): any[] {
    return this.skills().filter(s => s.category === category);
  }

  getTechArray(techStr: string): string[] {
    if (!techStr) return [];
    return techStr.split(',').map(s => s.trim());
  }

  getFullUrl(relativePath: string): string {
    if (!relativePath) return 'profile.jpg';
    if (relativePath.startsWith('http://') || relativePath.startsWith('https://')) return relativePath;
    const cleanPath = relativePath.startsWith('/') ? relativePath.substring(1) : relativePath;
    const baseUrl = environment.apiUrl.replace(/\/api\/v1\/?$/i, '').replace(/\/+$/, '');
    return `${baseUrl}/${cleanPath}`;
  }

  viewBlog(slug: string) {
    this.snackBar.open(`Navigating to blog details for slug: ${slug}`, 'Dismiss', { duration: 3000 });
  }

  onContactSubmit() {
    if (this.contactForm.invalid) return;

    this.isSubmitting.set(true);
    const msg = {
      name: this.contactForm.value.name!,
      email: this.contactForm.value.email!,
      subject: this.contactForm.value.subject || 'No Subject',
      message: this.contactForm.value.message!
    };

    this.dataService.submitContact(msg).subscribe({
      next: () => {
        this.isSubmitting.set(false);
        this.snackBar.open('Message sent successfully! Thank you for reaching out.', 'Close', { duration: 5000 });
        this.contactForm.reset();
      },
      error: () => {
        this.isSubmitting.set(false);
        this.snackBar.open('Failed to send message. Please try again later.', 'Close', { duration: 5000 });
      }
    });
  }

  scrollTo(sectionId: string): void {
    const el = document.getElementById(sectionId);
    if (el) {
      el.scrollIntoView({ behavior: 'smooth', block: 'start' });
    }
  }

  getFormattedTitle(): { prefix: string; name: string } {
    const title = this.hero()?.title || "Hi, I'm Alok";
    const match = title.match(/^(.*i'm\s+)(.*)$/i) || title.match(/^(.*i\s+am\s+)(.*)$/i);
    if (match) {
      return { prefix: match[1], name: match[2] };
    }
    const lastSpace = title.lastIndexOf(' ');
    if (lastSpace !== -1) {
      return { prefix: title.substring(0, lastSpace + 1), name: title.substring(lastSpace + 1) };
    }
    return { prefix: title, name: '' };
  }

  getFormattedSubtitle(): { prefix: string; highlight: string } {
    const subtitle = this.hero()?.subtitle || "ASP.NET Developer";
    const regex = /^(.*i'm\s+a\s+|.*i\s+am\s+a\s+|.*i'm\s+an\s+|.*i\s+am\s+an\s+|.*i'm\s+|.*i\s+am\s+)(.*)$/i;
    const match = subtitle.match(regex);
    if (match) {
      return { prefix: match[1], highlight: match[2] };
    }
    const lastSpace = subtitle.lastIndexOf(' ');
    if (lastSpace !== -1) {
      return { prefix: subtitle.substring(0, lastSpace + 1), highlight: subtitle.substring(lastSpace + 1) };
    }
    return { prefix: '', highlight: subtitle };
  }

  getExperienceStartDate(exp: any): string {
    if (exp?.isCurrent || exp?.company?.includes('Jogaz')) {
      return 'Sep 2025';
    }
    return new Date(exp.startDate).toLocaleDateString('en-US', { month: 'short', year: 'numeric' });
  }

  getExperienceDuration(startDate: string, endDate?: string, isCurrent?: boolean): string {
    if (isCurrent) return '11 months';
    const start = new Date(startDate);
    const end = isCurrent || !endDate ? new Date() : new Date(endDate);
    let months = (end.getFullYear() - start.getFullYear()) * 12 + (end.getMonth() - start.getMonth());
    if (months <= 0) months = 11;
    
    if (months >= 12) {
      const years = Math.floor(months / 12);
      const remainingMonths = months % 12;
      return remainingMonths > 0 
        ? `${years} yr${years > 1 ? 's' : ''} ${remainingMonths} mo${remainingMonths > 1 ? 's' : ''}`
        : `${years} yr${years > 1 ? 's' : ''}`;
    }
    return `${months} month${months > 1 ? 's' : ''}`;
  }

  getTotalExperienceString(): string {
    let totalMonths = 0;
    for (const exp of this.experiences()) {
      const start = new Date(exp.startDate);
      const end = exp.isCurrent || !exp.endDate ? new Date() : new Date(exp.endDate);
      const months = (end.getFullYear() - start.getFullYear()) * 12 + (end.getMonth() - start.getMonth());
      totalMonths += Math.max(0, months);
    }
    if (totalMonths <= 0) return '11 months';

    const years = Math.floor(totalMonths / 12);
    const remainingMonths = totalMonths % 12;

    let result = '';
    if (years > 0) {
      result += `${years} year${years > 1 ? 's' : ''}`;
    }
    if (remainingMonths > 0) {
      if (result) result += ' ';
      result += `${remainingMonths} month${remainingMonths > 1 ? 's' : ''}`;
    }
    return result || '11 months';
  }

  getAboutDescription(): string {
    const desc = this.about()?.description || 'Motivated ASP.NET Developer with 11 months of hands-on industry experience building scalable web applications using ASP.NET Core, RESTful APIs, and Angular. Passionate about clean architecture and contributing to high-impact software projects. Seeking a challenging role to grow technically and deliver production-grade solutions.';
    return desc.replace(/8\s*months/gi, '11 months');
  }

  getTotalExperienceYears(): number {
    let totalMonths = 0;
    for (const exp of this.experiences()) {
      const start = new Date(exp.startDate);
      const end = exp.isCurrent || !exp.endDate ? new Date() : new Date(exp.endDate);
      const months = (end.getFullYear() - start.getFullYear()) * 12 + (end.getMonth() - start.getMonth());
      totalMonths += Math.max(0, months);
    }
    const years = Math.floor(totalMonths / 12);
    return years > 0 ? years : 1; // Default to 1+ if there is experience but under a year
  }

  getSortedSkills() {
    return this.skills().slice().sort((a, b) => a.name.localeCompare(b.name));
  }

  isDownloadModalVisible = signal<boolean>(false);

  downloadForm = this.fb.group({
    name: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
    company: [''],
    designation: ['']
  });

  openDownloadModal() {
    this.downloadForm.reset();
    this.isDownloadModalVisible.set(true);
  }

  closeDownloadModal() {
    this.isDownloadModalVisible.set(false);
  }

  onSubmitDownload() {
    if (this.downloadForm.invalid) return;
    
    const val = this.downloadForm.value;
    const messageSubject = `Resume Download: ${val.name}`;
    const messageContent = `Name: ${val.name}\nEmail: ${val.email}\nCompany: ${val.company || 'N/A'}\nDesignation: ${val.designation || 'N/A'}`;
    
    // Save to local storage list for Admin Panel Resume Views tracking
    const newDownload = {
      id: 'dl-' + Date.now(),
      visitorName: val.name,
      email: val.email,
      company: val.company || '',
      designation: val.designation || '',
      location: window.location.hostname === 'localhost' || window.location.hostname === '127.0.0.1' ? 'DevelopmentLocalhost' : 'GurugramIndia',
      date: new Date().toISOString(),
      status: 'Downloaded'
    };

    try {
      const saved = localStorage.getItem('resume_downloads');
      let downloadsList = saved ? JSON.parse(saved) : [];
      downloadsList.unshift(newDownload);
      localStorage.setItem('resume_downloads', JSON.stringify(downloadsList));
    } catch (e) {
      console.error('Failed to save resume download log:', e);
    }

    this.dataService.submitContact({
      name: val.name!,
      email: val.email!,
      subject: messageSubject,
      message: messageContent
    }).subscribe({
      next: () => {
        this.snackBar.open('Thank you! Starting your download.', 'Close', { duration: 3000 });
        this.closeDownloadModal();
        
        const url = this.hero()?.resumeUrl || this.downloadResumeUrl;
        if (url) {
          window.open(url, '_blank');
        }
      },
      error: () => {
        this.snackBar.open('Starting download.', 'Close', { duration: 3000 });
        this.closeDownloadModal();
        
        const url = this.hero()?.resumeUrl || this.downloadResumeUrl;
        if (url) {
          window.open(url, '_blank');
        }
      }
    });
  }
}
