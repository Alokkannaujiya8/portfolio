import { TestBed } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { DashboardComponent } from './dashboard.component';
import { DataService } from '../../../core/services/data.service';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { ReactiveFormsModule } from '@angular/forms';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { MatCardModule } from '@angular/material/card';
import { MatTabsModule } from '@angular/material/tabs';
import { MatListModule } from '@angular/material/list';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { of } from 'rxjs';

describe('DashboardComponent', () => {
  let dataServiceMock: any;

  beforeEach(async () => {
    dataServiceMock = {
      getProjects: () => of([]),
      getSkills: () => of([]),
      getAllBlogsAdmin: () => of([]),
      getHero: () => of({}),
      getAbout: () => of({}),
      getMessages: () => of([]),
      updateHero: () => of({}),
      updateAbout: () => of({}),
      markMessageRead: () => of({})
    };

    await TestBed.configureTestingModule({
      imports: [
        RouterTestingModule,
        MatSnackBarModule,
        ReactiveFormsModule,
        HttpClientTestingModule,
        MatCardModule,
        MatTabsModule,
        MatListModule,
        MatIconModule,
        MatFormFieldModule,
        MatInputModule,
        BrowserAnimationsModule
      ],
      declarations: [
        DashboardComponent
      ],
      providers: [
        { provide: DataService, useValue: dataServiceMock }
      ]
    }).compileComponents();
  });

  it('should create the component', () => {
    const fixture = TestBed.createComponent(DashboardComponent);
    const component = fixture.componentInstance;
    expect(component).toBeTruthy();
  });
});
