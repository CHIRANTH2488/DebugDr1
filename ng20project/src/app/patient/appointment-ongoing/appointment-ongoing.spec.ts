import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AppointmentOngoing } from './appointment-ongoing';

describe('AppointmentOngoing', () => {
  let component: AppointmentOngoing;
  let fixture: ComponentFixture<AppointmentOngoing>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [AppointmentOngoing]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AppointmentOngoing);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
