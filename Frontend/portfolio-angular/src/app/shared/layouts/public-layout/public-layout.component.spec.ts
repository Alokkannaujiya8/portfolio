import { TestBed } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { PublicLayoutComponent } from './public-layout.component';
import { ThemeService } from '../../../core/services/theme.service';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatListModule } from '@angular/material/list';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

describe('PublicLayoutComponent', () => {
  let themeServiceMock: any;

  beforeEach(async () => {
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
        BrowserAnimationsModule
      ],
      declarations: [
        PublicLayoutComponent
      ],
      providers: [
        { provide: ThemeService, useValue: themeServiceMock }
      ]
    }).compileComponents();
  });

  it('should create the component', () => {
    const fixture = TestBed.createComponent(PublicLayoutComponent);
    const component = fixture.componentInstance;
    expect(component).toBeTruthy();
  });
});
