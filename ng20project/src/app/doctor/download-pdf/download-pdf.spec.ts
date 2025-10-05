import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DownloadPdf } from './download-pdf';

describe('DownloadPdf', () => {
  let component: DownloadPdf;
  let fixture: ComponentFixture<DownloadPdf>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [DownloadPdf]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DownloadPdf);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
