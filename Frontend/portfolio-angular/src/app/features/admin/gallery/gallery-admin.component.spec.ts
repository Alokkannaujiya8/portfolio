import { TestBed } from '@angular/core/testing';
import { GalleryAdminComponent } from './gallery-admin.component';
import { MatCardModule } from '@angular/material/card';

describe('GalleryAdminComponent', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MatCardModule],
      declarations: [GalleryAdminComponent]
    }).compileComponents();
  });

  it('should create the component', () => {
    const fixture = TestBed.createComponent(GalleryAdminComponent);
    const component = fixture.componentInstance;
    expect(component).toBeTruthy();
  });
});
