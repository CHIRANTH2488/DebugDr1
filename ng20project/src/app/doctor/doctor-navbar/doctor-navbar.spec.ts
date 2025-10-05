import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DoctorNavbar } from './doctor-navbar';

describe('DoctorNavbar', () => {
  let component: DoctorNavbar;
  let fixture: ComponentFixture<DoctorNavbar>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [DoctorNavbar]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DoctorNavbar);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
