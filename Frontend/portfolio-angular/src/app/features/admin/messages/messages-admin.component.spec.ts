import { TestBed } from '@angular/core/testing';
import { MessagesAdminComponent } from './messages-admin.component';
import { MatCardModule } from '@angular/material/card';
import { DataService } from '../../../core/services/data.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { of } from 'rxjs';
import { NO_ERRORS_SCHEMA } from '@angular/core';

describe('MessagesAdminComponent', () => {
  let dataServiceMock: any;
  let snackBarMock: any;

  beforeEach(async () => {
    dataServiceMock = {
      getMessages: () => of([]),
      deleteMessage: () => of({})
    };
    snackBarMock = {
      open: () => {}
    };

    await TestBed.configureTestingModule({
      imports: [MatCardModule],
      declarations: [MessagesAdminComponent],
      providers: [
        { provide: DataService, useValue: dataServiceMock },
        { provide: MatSnackBar, useValue: snackBarMock }
      ],
      schemas: [NO_ERRORS_SCHEMA]
    }).compileComponents();
  });

  it('should create the component', () => {
    const fixture = TestBed.createComponent(MessagesAdminComponent);
    const component = fixture.componentInstance;
    expect(component).toBeTruthy();
  });
});
