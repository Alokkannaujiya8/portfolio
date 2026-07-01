import { TestBed } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { AdminLayoutComponent } from './admin-layout.component';
import { AuthService } from '../../../core/services/auth.service';
import { ThemeService } from '../../../core/services/theme.service';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatListModule } from '@angular/material/list';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HttpClientTestingModule } from '@angular/common/http/testing';

describe('AdminLayoutComponent', () => {
  let authServiceMock: any;
  let themeServiceMock: any;

  beforeEach(async () => {
    authServiceMock = {
      currentUserEmail: () => 'admin@portfolio.com',
      logout: () => {}
    };

    themeServiceMock = {
      isDarkMode: () => false,
      toggleTheme: () => {}
    };

    await TestBed.configureTestingModule({
      imports: [
        RouterTestingModule,
        MatToolbarModule,
        MatSidenavModule,
        MatListModule,
        BrowserAnimationsModule,
        HttpClientTestingModule
      ],
      declarations: [
        AdminLayoutComponent
      ],
      providers: [
        { provide: AuthService, useValue: authServiceMock },
        { provide: ThemeService, useValue: themeServiceMock }
      ]
    }).compileComponents();
  });

  it('should create the component', () => {
    const fixture = TestBed.createComponent(AdminLayoutComponent);
    const component = fixture.componentInstance;
    expect(component).toBeTruthy();
  });
});
