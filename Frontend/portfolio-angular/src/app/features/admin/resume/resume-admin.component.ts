import { Component, inject, OnInit, signal } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { DataService } from '../../../core/services/data.service';

@Component({
  selector: 'app-resume-admin',
  standalone: false,
  templateUrl: './resume-admin.component.html',
  styleUrl: './resume-admin.component.scss'
})
export class ResumeAdminComponent implements OnInit {
  private dataService = inject(DataService);
  private snackBar = inject(MatSnackBar);

  resumes = signal<any[]>([]);
  selectedFile = signal<File | null>(null);
  isUploading = signal<boolean>(false);

  displayedColumns: string[] = ['fileName', 'createdAt', 'isActive', 'actions'];

  ngOnInit() {
    this.loadResumes();
  }

  loadResumes() {
    this.dataService.getResumes().subscribe(res => this.resumes.set(res));
  }

  onFileSelected(event: any) {
    const file = event.target.files[0];
    if (file) {
      if (file.type !== 'application/pdf') {
        this.snackBar.open('Only PDF files are allowed.', 'Dismiss', { duration: 3000 });
        return;
      }
      this.selectedFile.set(file);
    }
  }

  onClearFile() {
    this.selectedFile.set(null);
  }

  onUpload() {
    const file = this.selectedFile();
    if (!file) return;

    this.isUploading.set(true);
    this.dataService.uploadResume(file).subscribe({
      next: () => {
        this.isUploading.set(false);
        this.selectedFile.set(null);
        this.snackBar.open('Resume uploaded and activated successfully.', 'Close', { duration: 3000 });
        this.loadResumes();
      },
      error: (err) => {
        this.isUploading.set(false);
        this.snackBar.open(err.error || 'Upload failed.', 'Close', { duration: 3000 });
      }
    });
  }

  onActivate(id: string) {
    this.dataService.activateResume(id).subscribe({
      next: () => {
        this.snackBar.open('Resume activated successfully.', 'Close', { duration: 3000 });
        this.loadResumes();
      },
      error: () => this.snackBar.open('Failed to activate resume.', 'Close', { duration: 3000 })
    });
  }

  onDelete(id: string) {
    if (confirm('Are you sure you want to delete this resume?')) {
      this.dataService.deleteResume(id).subscribe({
        next: () => {
          this.snackBar.open('Resume deleted successfully.', 'Close', { duration: 3000 });
          this.loadResumes();
        },
        error: () => this.snackBar.open('Failed to delete resume.', 'Close', { duration: 3000 })
      });
    }
  }
}
