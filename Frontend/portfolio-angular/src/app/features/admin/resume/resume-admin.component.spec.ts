import { TestBed } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { ResumeAdminComponent } from './resume-admin.component';
import { DataService } from '../../../core/services/data.service';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatIconModule } from '@angular/material/icon';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { of } from 'rxjs';

describe('ResumeAdminComponent', () => {
  let dataServiceMock: any;

  beforeEach(async () => {
    dataServiceMock = {
      getResumes: () => of([]),
      uploadResume: () => of({}),
      activateResume: () => of({}),
      deleteResume: () => of({})
    };

    await TestBed.configureTestingModule({
      imports: [
        RouterTestingModule,
        MatSnackBarModule,
        HttpClientTestingModule,
        MatCardModule,
        MatTableModule,
        MatIconModule,
        BrowserAnimationsModule
      ],
      declarations: [
        ResumeAdminComponent
      ],
      providers: [
        { provide: DataService, useValue: dataServiceMock }
      ]
    }).compileComponents();
  });

  it('should create the component', () => {
    const fixture = TestBed.createComponent(ResumeAdminComponent);
    const component = fixture.componentInstance;
    expect(component).toBeTruthy();
  });
});
