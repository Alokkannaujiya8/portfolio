import { TestBed } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { EducationAdminComponent } from './education-admin.component';
import { DataService } from '../../../core/services/data.service';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { ReactiveFormsModule } from '@angular/forms';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { of } from 'rxjs';

describe('EducationAdminComponent', () => {
  let dataServiceMock: any;

  beforeEach(async () => {
    dataServiceMock = {
      getEducations: () => of([]),
      createEducation: () => of({}),
      updateEducation: () => of({}),
      deleteEducation: () => of({})
    };

    await TestBed.configureTestingModule({
      imports: [
        RouterTestingModule,
        MatSnackBarModule,
        ReactiveFormsModule,
        HttpClientTestingModule,
        MatCardModule,
        MatTableModule,
        MatCheckboxModule,
        MatIconModule,
        MatFormFieldModule,
        MatInputModule,
        BrowserAnimationsModule
      ],
      declarations: [
        EducationAdminComponent
      ],
      providers: [
        { provide: DataService, useValue: dataServiceMock }
      ]
    }).compileComponents();
  });

  it('should create the component', () => {
    const fixture = TestBed.createComponent(EducationAdminComponent);
    const component = fixture.componentInstance;
    expect(component).toBeTruthy();
  });
});
