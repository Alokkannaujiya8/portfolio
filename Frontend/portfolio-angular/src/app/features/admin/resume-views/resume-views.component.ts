import { Component, OnInit, signal } from '@angular/core';

interface ResumeDownload {
  id: string;
  visitorName: string;
  email: string;
  company: string;
  designation: string;
  location: string;
  date: string;
  status: string;
}

@Component({
  selector: 'app-resume-views',
  standalone: false,
  templateUrl: './resume-views.component.html',
  styleUrl: './resume-views.component.scss'
})
export class ResumeViewsComponent implements OnInit {
  downloads = signal<ResumeDownload[]>([]);

  ngOnInit(): void {
    this.loadDownloads();
  }

  loadDownloads(): void {
    const saved = localStorage.getItem('resume_downloads');
    if (saved) {
      this.downloads.set(JSON.parse(saved));
    } else {
      // Mock data matching the screenshot exactly!
      const initial: ResumeDownload[] = [
        {
          id: '1',
          visitorName: 'Harshita Rathore',
          email: 'officialharshitarathore15@gmail.com',
          company: '',
          designation: '',
          location: 'GurugramIndia',
          date: '2026-06-29T06:18:00Z',
          status: 'Downloaded'
        },
        {
          id: '2',
          visitorName: 'ajay',
          email: 'ajaykumar@gmail.com',
          company: '',
          designation: '',
          location: 'DevelopmentLocalhost',
          date: '2026-06-28T11:43:00Z',
          status: 'Downloaded'
        },
        {
          id: '3',
          visitorName: 'ajay',
          email: 'ajaykumar@gmail.com',
          company: '',
          designation: '',
          location: 'DevelopmentLocalhost',
          date: '2026-06-28T01:11:00Z',
          status: 'Downloaded'
        },
        {
          id: '4',
          visitorName: 'ajay',
          email: 'ajaykumar@gmail.com',
          company: '',
          designation: '',
          location: 'DevelopmentLocalhost',
          date: '2026-06-28T01:08:00Z',
          status: 'Downloaded'
        },
        {
          id: '5',
          visitorName: 'Ajay Kumar',
          email: 'ajaykumar737905@gmail.com',
          company: 'Jupiter Orison',
          designation: '',
          location: 'Unknown',
          date: '2026-06-06T12:07:00Z',
          status: 'Downloaded'
        },
        {
          id: '6',
          visitorName: 'Ajay Kumar',
          email: 'ajaykumar737905@gmail.com',
          company: '',
          designation: '',
          location: 'Unknown',
          date: '2026-03-23T07:04:00Z',
          status: 'Downloaded'
        },
        {
          id: '7',
          visitorName: 'Ajay Kumar',
          email: 'ak74579747@gmail.com',
          company: 'Jupeak',
          designation: 'AI&ML DEVELOPER',
          location: 'Unknown',
          date: '2026-03-21T18:47:00Z',
          status: 'Downloaded'
        }
      ];
      this.downloads.set(initial);
      localStorage.setItem('resume_downloads', JSON.stringify(initial));
    }
  }
}
