import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OurDoctors } from './our-doctors';

describe('OurDoctors', () => {
  let component: OurDoctors;
  let fixture: ComponentFixture<OurDoctors>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [OurDoctors]
    })
    .compileComponents();

    fixture = TestBed.createComponent(OurDoctors);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
