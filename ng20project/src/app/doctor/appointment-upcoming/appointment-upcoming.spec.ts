import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AppointmentUpcoming } from './appointment-upcoming';

describe('AppointmentUpcoming', () => {
  let component: AppointmentUpcoming;
  let fixture: ComponentFixture<AppointmentUpcoming>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [AppointmentUpcoming]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AppointmentUpcoming);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
