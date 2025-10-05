import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DoctorRequests } from './doctor-requests';

describe('DoctorRequests', () => {
  let component: DoctorRequests;
  let fixture: ComponentFixture<DoctorRequests>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [DoctorRequests]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DoctorRequests);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
