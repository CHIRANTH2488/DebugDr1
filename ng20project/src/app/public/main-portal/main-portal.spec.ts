import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MainPortal } from './main-portal';

describe('MainPortal', () => {
  let component: MainPortal;
  let fixture: ComponentFixture<MainPortal>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [MainPortal]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MainPortal);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
