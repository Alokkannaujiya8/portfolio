import { Component, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { DataService } from '../../../core/services/data.service';

@Component({
  selector: 'app-skills-admin',
  standalone: false,
  templateUrl: './skills-admin.component.html',
  styleUrl: './skills-admin.component.scss'
})
export class SkillsAdminComponent implements OnInit {
  private dataService = inject(DataService);
  private fb = inject(FormBuilder);
  private snackBar = inject(MatSnackBar);

  skills = signal<any[]>([]);
  isFormVisible = signal<boolean>(false);
  editId = signal<string | null>(null);

  displayedColumns: string[] = ['name', 'category', 'proficiency', 'actions'];

  skillForm = this.fb.group({
    name: ['', Validators.required],
    category: ['Frontend', Validators.required],
    proficiency: [80, [Validators.required, Validators.min(0), Validators.max(100)]],
    displayOrder: [0]
  });

  ngOnInit() {
    this.loadSkills();
  }

  loadSkills() {
    this.dataService.getSkills().subscribe(res => this.skills.set(res));
  }

  onNewSkill() {
    this.editId.set(null);
    this.skillForm.reset({ category: 'Frontend', proficiency: 80, displayOrder: 0 });
    this.isFormVisible.set(true);
  }

  onEdit(skill: any) {
    this.editId.set(skill.id);
    this.skillForm.patchValue(skill);
    this.isFormVisible.set(true);
  }

  onCancel() {
    this.isFormVisible.set(false);
  }

  onSubmit() {
    if (this.skillForm.invalid) return;

    const payload = this.skillForm.value;
    const id = this.editId();

    if (id) {
      const updatePayload = { id, ...payload };
      this.dataService.updateSkill(id, updatePayload).subscribe({
        next: () => {
          this.snackBar.open('Skill updated successfully.', 'Close', { duration: 3000 });
          this.isFormVisible.set(false);
          this.loadSkills();
        },
        error: () => this.snackBar.open('Failed to update skill.', 'Close', { duration: 3000 })
      });
    } else {
      this.dataService.createSkill(payload).subscribe({
        next: () => {
          this.snackBar.open('Skill created successfully.', 'Close', { duration: 3000 });
          this.isFormVisible.set(false);
          this.loadSkills();
        },
        error: () => this.snackBar.open('Failed to create skill.', 'Close', { duration: 3000 })
      });
    }
  }

  onDelete(id: string) {
    if (confirm('Are you sure you want to delete this skill?')) {
      this.dataService.deleteSkill(id).subscribe({
        next: () => {
          this.snackBar.open('Skill deleted successfully.', 'Close', { duration: 3000 });
          this.loadSkills();
        },
        error: () => this.snackBar.open('Failed to delete skill.', 'Close', { duration: 3000 })
      });
    }
  }
}
