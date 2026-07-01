import { Component, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { DataService } from '../../../core/services/data.service';

@Component({
  selector: 'app-education-admin',
  standalone: false,
  templateUrl: './education-admin.component.html',
  styleUrl: './education-admin.component.scss'
})
export class EducationAdminComponent implements OnInit {
  private dataService = inject(DataService);
  private fb = inject(FormBuilder);
  private snackBar = inject(MatSnackBar);

  educations = signal<any[]>([]);
  isFormVisible = signal<boolean>(false);
  editId = signal<string | null>(null);

  displayedColumns: string[] = ['institution', 'degree', 'dates', 'actions'];

  educationForm = this.fb.group({
    institution: ['', Validators.required],
    degree: ['', Validators.required],
    fieldOfStudy: ['', Validators.required],
    startDate: ['', Validators.required],
    endDate: [''],
    isCurrent: [false],
    grade: [''],
    displayOrder: [0]
  });

  ngOnInit() {
    this.loadEducations();
  }

  loadEducations() {
    this.dataService.getEducations().subscribe(res => this.educations.set(res));
  }

  onNewEducation() {
    this.editId.set(null);
    this.educationForm.reset({ isCurrent: false, displayOrder: 0 });
    this.isFormVisible.set(true);
  }

  onEdit(edu: any) {
    this.editId.set(edu.id);
    
    // Format dates to YYYY-MM-DD for standard html date input
    const formattedEdu = { ...edu };
    if (edu.startDate) {
      formattedEdu.startDate = edu.startDate.split('T')[0];
    }
    if (edu.endDate) {
      formattedEdu.endDate = edu.endDate.split('T')[0];
    }
    
    this.educationForm.patchValue(formattedEdu);
    this.isFormVisible.set(true);
  }

  onCancel() {
    this.isFormVisible.set(false);
  }

  onSubmit() {
    if (this.educationForm.invalid) return;

    const payload = this.educationForm.value;
    const id = this.editId();

    if (id) {
      const updatePayload = { id, ...payload };
      this.dataService.updateEducation(id, updatePayload).subscribe({
        next: () => {
          this.snackBar.open('Education record updated successfully.', 'Close', { duration: 3000 });
          this.isFormVisible.set(false);
          this.loadEducations();
        },
        error: () => this.snackBar.open('Failed to update education record.', 'Close', { duration: 3000 })
      });
    } else {
      this.dataService.createEducation(payload).subscribe({
        next: () => {
          this.snackBar.open('Education record created successfully.', 'Close', { duration: 3000 });
          this.isFormVisible.set(false);
          this.loadEducations();
        },
        error: () => this.snackBar.open('Failed to create education record.', 'Close', { duration: 3000 })
      });
    }
  }

  onDelete(id: string) {
    if (confirm('Are you sure you want to delete this education entry?')) {
      this.dataService.deleteEducation(id).subscribe({
        next: () => {
          this.snackBar.open('Education entry deleted successfully.', 'Close', { duration: 3000 });
          this.loadEducations();
        },
        error: () => this.snackBar.open('Failed to delete education entry.', 'Close', { duration: 3000 })
      });
    }
  }
}
