import { Component, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-settings-admin',
  standalone: false,
  templateUrl: './settings-admin.component.html',
  styleUrl: './settings-admin.component.scss'
})
export class SettingsAdminComponent implements OnInit {
  authService = inject(AuthService);
  private fb = inject(FormBuilder);
  private snackBar = inject(MatSnackBar);

  settingsForm!: FormGroup;
  isLoading = signal<boolean>(false);
  currentLoginTime = new Date();

  // Visibility toggles
  hideCurrentPassword = signal<boolean>(true);
  hideNewPassword = signal<boolean>(true);
  hideConfirmPassword = signal<boolean>(true);

  ngOnInit(): void {
    const currentEmail = this.authService.currentUserEmail() || 'admin@portfolio.com';
    
    this.settingsForm = this.fb.group({
      currentUsername: [{ value: currentEmail, disabled: true }],
      currentPassword: ['', Validators.required],
      newUsername: [currentEmail, [Validators.required, Validators.email]],
      newPassword: ['', [Validators.minLength(8)]],
      confirmNewPassword: ['']
    }, { validators: this.passwordMatchValidator });
  }

  private passwordMatchValidator(g: FormGroup) {
    const newPass = g.get('newPassword')?.value;
    const confirmPass = g.get('confirmNewPassword')?.value;
    if (newPass && newPass !== confirmPass) {
      g.get('confirmNewPassword')?.setErrors({ mismatch: true });
    } else {
      g.get('confirmNewPassword')?.setErrors(null);
    }
    return null;
  }

  onSubmit(): void {
    if (this.settingsForm.invalid) return;

    this.isLoading.set(true);
    const formValue = this.settingsForm.value;
    const model = {
      currentPassword: formValue.currentPassword,
      newUsername: formValue.newUsername,
      newPassword: formValue.newPassword || null
    };

    this.authService.changeCredentials(model).subscribe({
      next: (res) => {
        this.isLoading.set(false);
        this.snackBar.open(res.message || 'Credentials updated successfully!', 'Close', { duration: 3000 });
        // Update user email state if username changed
        if (model.newUsername && model.newUsername !== this.authService.currentUserEmail()) {
          this.authService.currentUserEmail.set(model.newUsername);
        }
        // Reset password fields
        this.settingsForm.patchValue({
          currentPassword: '',
          newPassword: '',
          confirmNewPassword: ''
        });
        this.settingsForm.markAsPristine();
        this.settingsForm.markAsUntouched();
      },
      error: (err) => {
        this.isLoading.set(false);
        const errMsg = err.error?.message || 'Failed to update credentials. Please check your inputs.';
        this.snackBar.open(errMsg, 'Close', { duration: 4000 });
      }
    });
  }

  onCancel(): void {
    const currentEmail = this.authService.currentUserEmail() || 'admin@portfolio.com';
    this.settingsForm.reset({
      currentUsername: currentEmail,
      currentPassword: '',
      newUsername: currentEmail,
      newPassword: '',
      confirmNewPassword: ''
    });
    this.snackBar.open('Changes cancelled.', 'Close', { duration: 2000 });
  }
}
