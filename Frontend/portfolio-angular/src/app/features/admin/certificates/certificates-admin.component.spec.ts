import { TestBed } from '@angular/core/testing';
import { CertificatesAdminComponent } from './certificates-admin.component';
import { MatCardModule } from '@angular/material/card';

describe('CertificatesAdminComponent', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MatCardModule],
      declarations: [CertificatesAdminComponent]
    }).compileComponents();
  });

  it('should create the component', () => {
    const fixture = TestBed.createComponent(CertificatesAdminComponent);
    const component = fixture.componentInstance;
    expect(component).toBeTruthy();
  });
});
