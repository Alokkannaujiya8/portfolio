import { TestBed } from '@angular/core/testing';
import { MessagesAdminComponent } from './messages-admin.component';
import { MatCardModule } from '@angular/material/card';

describe('MessagesAdminComponent', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MatCardModule],
      declarations: [MessagesAdminComponent]
    }).compileComponents();
  });

  it('should create the component', () => {
    const fixture = TestBed.createComponent(MessagesAdminComponent);
    const component = fixture.componentInstance;
    expect(component).toBeTruthy();
  });
});
