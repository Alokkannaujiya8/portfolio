import { TestBed } from '@angular/core/testing';
import { BlogsAdminComponent } from './blogs-admin.component';
import { MatCardModule } from '@angular/material/card';

describe('BlogsAdminComponent', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MatCardModule],
      declarations: [BlogsAdminComponent]
    }).compileComponents();
  });

  it('should create the component', () => {
    const fixture = TestBed.createComponent(BlogsAdminComponent);
    const component = fixture.componentInstance;
    expect(component).toBeTruthy();
  });
});
