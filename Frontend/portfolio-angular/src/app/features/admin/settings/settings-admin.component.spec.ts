import { TestBed } from '@angular/core/testing';
import { SettingsAdminComponent } from './settings-admin.component';
import { MatCardModule } from '@angular/material/card';

describe('SettingsAdminComponent', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MatCardModule],
      declarations: [SettingsAdminComponent]
    }).compileComponents();
  });

  it('should create the component', () => {
    const fixture = TestBed.createComponent(SettingsAdminComponent);
    const component = fixture.componentInstance;
    expect(component).toBeTruthy();
  });
});
