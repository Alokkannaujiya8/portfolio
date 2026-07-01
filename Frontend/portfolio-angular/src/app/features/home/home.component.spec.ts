import { TestBed } from '@angular/core/testing';
import { HomeComponent } from './home.component';
import { DataService } from '../../core/services/data.service';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { ReactiveFormsModule } from '@angular/forms';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { MatCardModule } from '@angular/material/card';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { of } from 'rxjs';

describe('HomeComponent', () => {
  let dataServiceMock: any;

  beforeEach(async () => {
    dataServiceMock = {
      getHero: () => of({}),
      getAbout: () => of({}),
      getServices: () => of([]),
      getExperiences: () => of([]),
      getEducations: () => of([]),
      getCertificates: () => of([]),
      getProjects: () => of([]),
      getBlogs: () => of([]),
      getGallery: () => of([]),
      getSkills: () => of([]),
      downloadResumeUrl: () => 'http://localhost/resume'
    };

    await TestBed.configureTestingModule({
      imports: [
        MatSnackBarModule,
        ReactiveFormsModule,
        HttpClientTestingModule,
        MatCardModule,
        MatProgressBarModule,
        MatIconModule,
        MatFormFieldModule,
        MatInputModule,
        BrowserAnimationsModule
      ],
      declarations: [
        HomeComponent
      ],
      providers: [
        { provide: DataService, useValue: dataServiceMock }
      ]
    }).compileComponents();
  });

  it('should create the component', () => {
    const fixture = TestBed.createComponent(HomeComponent);
    const component = fixture.componentInstance;
    expect(component).toBeTruthy();
  });
});
