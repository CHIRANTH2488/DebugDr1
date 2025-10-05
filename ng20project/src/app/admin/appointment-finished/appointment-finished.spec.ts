import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AppointmentFinished } from './appointment-finished';

describe('AppointmentFinished', () => {
  let component: AppointmentFinished;
  let fixture: ComponentFixture<AppointmentFinished>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [AppointmentFinished]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AppointmentFinished);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
