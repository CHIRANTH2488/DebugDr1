import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PatientRequests } from './patient-requests';

describe('PatientRequests', () => {
  let component: PatientRequests;
  let fixture: ComponentFixture<PatientRequests>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [PatientRequests]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PatientRequests);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
