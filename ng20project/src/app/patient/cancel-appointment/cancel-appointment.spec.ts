import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CancelAppointment } from './cancel-appointment';

describe('CancelAppointment', () => {
  let component: CancelAppointment;
  let fixture: ComponentFixture<CancelAppointment>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [CancelAppointment]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CancelAppointment);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
