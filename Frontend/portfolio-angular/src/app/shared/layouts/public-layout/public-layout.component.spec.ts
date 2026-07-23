import { TestBed } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { PublicLayoutComponent } from './public-layout.component';
import { ThemeService } from '../../../core/services/theme.service';
import { AuthService } from '../../../core/services/auth.service';
import { DataService } from '../../../core/services/data.service';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatListModule } from '@angular/material/list';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { of } from 'rxjs';
import { NO_ERRORS_SCHEMA } from '@angular/core';

describe('PublicLayoutComponent', () => {
  let themeServiceMock: any;
  let authServiceMock: any;
  let dataServiceMock: any;

  beforeEach(async () => {
    themeServiceMock = {
      isDarkMode: () => false,
      toggleTheme: () => {}
    };

    authServiceMock = {
      isAuthenticated: () => false,
      logout: () => {}
    };

    dataServiceMock = {
      getAbout: () => of({}),
      getHero: () => of({})
    };

    await TestBed.configureTestingModule({
      imports: [
        RouterTestingModule,
        MatToolbarModule,
        MatSidenavModule,
        MatListModule,
        BrowserAnimationsModule
      ],
      declarations: [
        PublicLayoutComponent
      ],
      providers: [
        { provide: ThemeService, useValue: themeServiceMock },
        { provide: AuthService, useValue: authServiceMock },
        { provide: DataService, useValue: dataServiceMock }
      ],
      schemas: [NO_ERRORS_SCHEMA]
    }).compileComponents();
  });

  it('should create the component', () => {
    const fixture = TestBed.createComponent(PublicLayoutComponent);
    const component = fixture.componentInstance;
    expect(component).toBeTruthy();
  });
});
