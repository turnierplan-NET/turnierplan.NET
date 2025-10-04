import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ApplicationChangeLogComponent } from './application-change-log.component';

describe('ApplicationChangeLogComponent', () => {
  let component: ApplicationChangeLogComponent;
  let fixture: ComponentFixture<ApplicationChangeLogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ApplicationChangeLogComponent]
    }).compileComponents();

    fixture = TestBed.createComponent(ApplicationChangeLogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
