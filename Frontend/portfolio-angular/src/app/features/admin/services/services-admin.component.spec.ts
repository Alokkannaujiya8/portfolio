import { TestBed } from '@angular/core/testing';
import { ServicesAdminComponent } from './services-admin.component';
import { MatCardModule } from '@angular/material/card';

describe('ServicesAdminComponent', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MatCardModule],
      declarations: [ServicesAdminComponent]
    }).compileComponents();
  });

  it('should create the component', () => {
    const fixture = TestBed.createComponent(ServicesAdminComponent);
    const component = fixture.componentInstance;
    expect(component).toBeTruthy();
  });
});
