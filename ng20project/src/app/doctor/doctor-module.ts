import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { DoctorDashboard } from './doctor-dashboard/doctor-dashboard';
import { DoctorDetails } from './doctor-details/doctor-details';
import { DoctorNavbar } from './doctor-navbar/doctor-navbar';
import { AppointmentUpcoming } from './appointment-upcoming/appointment-upcoming';
import { AppointmentOngoing } from './appointment-ongoing/appointment-ongoing';
import { AppointmentHistory } from './appointment-history/appointment-history';
import { PrescriptionForm } from './prescription-form/prescription-form';
import { InvoiceForm } from './invoice-form/invoice-form';
import { DownloadPdf } from './download-pdf/download-pdf';

@NgModule({
  declarations: [
    DoctorDashboard,
    DoctorDetails,
    DoctorNavbar,
    AppointmentUpcoming,
    AppointmentOngoing,
    AppointmentHistory,
    PrescriptionForm,
    InvoiceForm,
    DownloadPdf
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule
  ],
  exports: [
    DoctorDashboard,
    DoctorDetails,
    DoctorNavbar,
    AppointmentUpcoming,
    AppointmentOngoing,
    AppointmentHistory,
    PrescriptionForm,
    InvoiceForm,
    DownloadPdf
  ]
})
export class DoctorModule { }