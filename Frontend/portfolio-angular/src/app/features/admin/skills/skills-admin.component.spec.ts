import { TestBed } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { SkillsAdminComponent } from './skills-admin.component';
import { DataService } from '../../../core/services/data.service';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { ReactiveFormsModule } from '@angular/forms';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatSelectModule } from '@angular/material/select';
import { MatSliderModule } from '@angular/material/slider';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { of } from 'rxjs';

describe('SkillsAdminComponent', () => {
  let dataServiceMock: any;

  beforeEach(async () => {
    dataServiceMock = {
      getSkills: () => of([]),
      createSkill: () => of({}),
      updateSkill: () => of({}),
      deleteSkill: () => of({})
    };

    await TestBed.configureTestingModule({
      imports: [
        RouterTestingModule,
        MatSnackBarModule,
        ReactiveFormsModule,
        HttpClientTestingModule,
        MatCardModule,
        MatTableModule,
        MatSelectModule,
        MatSliderModule,
        MatIconModule,
        MatFormFieldModule,
        MatInputModule,
        BrowserAnimationsModule
      ],
      declarations: [
        SkillsAdminComponent
      ],
      providers: [
        { provide: DataService, useValue: dataServiceMock }
      ]
    }).compileComponents();
  });

  it('should create the component', () => {
    const fixture = TestBed.createComponent(SkillsAdminComponent);
    const component = fixture.componentInstance;
    expect(component).toBeTruthy();
  });
});
