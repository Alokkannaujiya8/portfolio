import { Component, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { DataService } from '../../../core/services/data.service';

@Component({
  selector: 'app-projects-admin',
  standalone: false,
  templateUrl: './projects-admin.component.html',
  styleUrl: './projects-admin.component.scss'
})
export class ProjectsAdminComponent implements OnInit {
  private dataService = inject(DataService);
  private fb = inject(FormBuilder);
  private snackBar = inject(MatSnackBar);

  projects = signal<any[]>([]);
  isFormVisible = signal<boolean>(false);
  editId = signal<string | null>(null);

  displayedColumns: string[] = ['title', 'displayOrder', 'isFeatured', 'actions'];

  projectForm = this.fb.group({
    title: ['', Validators.required],
    description: ['', Validators.required],
    imageUrl: [''],
    projectUrl: [''],
    githubUrl: [''],
    technologies: [''],
    displayOrder: [0],
    isFeatured: [false]
  });

  ngOnInit() {
    this.loadProjects();
  }

  loadProjects() {
    this.dataService.getProjects().subscribe(res => this.projects.set(res));
  }

  onNewProject() {
    this.editId.set(null);
    this.projectForm.reset({ displayOrder: 0, isFeatured: false });
    this.isFormVisible.set(true);
  }

  onEdit(project: any) {
    this.editId.set(project.id);
    this.projectForm.patchValue(project);
    this.isFormVisible.set(true);
  }

  onCancel() {
    this.isFormVisible.set(false);
  }

  onSubmit() {
    if (this.projectForm.invalid) return;

    const payload = this.projectForm.value;
    const id = this.editId();

    if (id) {
      const updatePayload = { id, ...payload };
      this.dataService.updateProject(id, updatePayload).subscribe({
        next: () => {
          this.snackBar.open('Project updated successfully.', 'Close', { duration: 3000 });
          this.isFormVisible.set(false);
          this.loadProjects();
        },
        error: () => this.snackBar.open('Failed to update project.', 'Close', { duration: 3000 })
      });
    } else {
      this.dataService.createProject(payload).subscribe({
        next: () => {
          this.snackBar.open('Project created successfully.', 'Close', { duration: 3000 });
          this.isFormVisible.set(false);
          this.loadProjects();
        },
        error: () => this.snackBar.open('Failed to create project.', 'Close', { duration: 3000 })
      });
    }
  }

  onDelete(id: string) {
    if (confirm('Are you sure you want to delete this project?')) {
      this.dataService.deleteProject(id).subscribe({
        next: () => {
          this.snackBar.open('Project deleted successfully.', 'Close', { duration: 3000 });
          this.loadProjects();
        },
        error: () => this.snackBar.open('Failed to delete project.', 'Close', { duration: 3000 })
      });
    }
  }
}
