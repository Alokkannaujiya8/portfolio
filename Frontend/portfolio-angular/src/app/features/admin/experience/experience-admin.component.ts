import { Component, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { DataService } from '../../../core/services/data.service';

@Component({
  selector: 'app-experience-admin',
  standalone: false,
  templateUrl: './experience-admin.component.html',
  styleUrl: './experience-admin.component.scss'
})
export class ExperienceAdminComponent implements OnInit {
  private dataService = inject(DataService);
  private fb = inject(FormBuilder);
  private snackBar = inject(MatSnackBar);

  experiences = signal<any[]>([]);
  isFormVisible = signal<boolean>(false);
  editId = signal<string | null>(null);

  displayedColumns: string[] = ['company', 'role', 'dates', 'actions'];

  experienceForm = this.fb.group({
    company: ['', Validators.required],
    role: ['', Validators.required],
    location: [''],
    description: ['', Validators.required],
    startDate: ['', Validators.required],
    endDate: [''],
    isCurrent: [false],
    displayOrder: [0]
  });

  ngOnInit() {
    this.loadExperiences();
  }

  loadExperiences() {
    this.dataService.getExperiences().subscribe(res => this.experiences.set(res));
  }

  onNewExperience() {
    this.editId.set(null);
    this.experienceForm.reset({ isCurrent: false, displayOrder: 0 });
    this.isFormVisible.set(true);
  }

  onEdit(exp: any) {
    this.editId.set(exp.id);
    
    // Format dates to YYYY-MM-DD for standard html date input
    const formattedExp = { ...exp };
    if (exp.startDate) {
      formattedExp.startDate = exp.startDate.split('T')[0];
    }
    if (exp.endDate) {
      formattedExp.endDate = exp.endDate.split('T')[0];
    }
    
    this.experienceForm.patchValue(formattedExp);
    this.isFormVisible.set(true);
  }

  onCancel() {
    this.isFormVisible.set(false);
  }

  onSubmit() {
    if (this.experienceForm.invalid) return;

    const payload = this.experienceForm.value;
    const id = this.editId();

    if (id) {
      const updatePayload = { id, ...payload };
      this.dataService.updateExperience(id, updatePayload).subscribe({
        next: () => {
          this.snackBar.open('Experience updated successfully.', 'Close', { duration: 3000 });
          this.isFormVisible.set(false);
          this.loadExperiences();
        },
        error: () => this.snackBar.open('Failed to update experience.', 'Close', { duration: 3000 })
      });
    } else {
      this.dataService.createExperience(payload).subscribe({
        next: () => {
          this.snackBar.open('Experience created successfully.', 'Close', { duration: 3000 });
          this.isFormVisible.set(false);
          this.loadExperiences();
        },
        error: () => this.snackBar.open('Failed to create experience.', 'Close', { duration: 3000 })
      });
    }
  }

  onDelete(id: string) {
    if (confirm('Are you sure you want to delete this experience entry?')) {
      this.dataService.deleteExperience(id).subscribe({
        next: () => {
          this.snackBar.open('Experience entry deleted successfully.', 'Close', { duration: 3000 });
          this.loadExperiences();
        },
        error: () => this.snackBar.open('Failed to delete experience entry.', 'Close', { duration: 3000 })
      });
    }
  }
}
