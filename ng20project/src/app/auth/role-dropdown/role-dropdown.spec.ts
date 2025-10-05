import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RoleDropdown } from './role-dropdown';

describe('RoleDropdown', () => {
  let component: RoleDropdown;
  let fixture: ComponentFixture<RoleDropdown>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [RoleDropdown]
    })
    .compileComponents();

    fixture = TestBed.createComponent(RoleDropdown);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
