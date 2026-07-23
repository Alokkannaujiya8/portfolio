import { TestBed } from '@angular/core/testing';
import { SettingsAdminComponent } from './settings-admin.component';
import { MatCardModule } from '@angular/material/card';
import { ReactiveFormsModule } from '@angular/forms';
import { AuthService } from '../../../core/services/auth.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { of } from 'rxjs';
import { NO_ERRORS_SCHEMA } from '@angular/core';

describe('SettingsAdminComponent', () => {
  let authServiceMock: any;
  let snackBarMock: any;

  beforeEach(async () => {
    authServiceMock = {
      changePassword: () => of({})
    };
    snackBarMock = {
      open: () => {}
    };

    await TestBed.configureTestingModule({
      imports: [MatCardModule, ReactiveFormsModule],
      declarations: [SettingsAdminComponent],
      providers: [
        { provide: AuthService, useValue: authServiceMock },
        { provide: MatSnackBar, useValue: snackBarMock }
      ],
      schemas: [NO_ERRORS_SCHEMA]
    }).compileComponents();
  });

  it('should create the component', () => {
    const fixture = TestBed.createComponent(SettingsAdminComponent);
    const component = fixture.componentInstance;
    expect(component).toBeTruthy();
  });
});
