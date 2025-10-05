import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PatientNavbar } from './patient-navbar';

describe('PatientNavbar', () => {
  let component: PatientNavbar;
  let fixture: ComponentFixture<PatientNavbar>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [PatientNavbar]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PatientNavbar);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
