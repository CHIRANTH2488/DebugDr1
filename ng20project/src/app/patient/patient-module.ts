import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { PatientDashboard } from './patient-dashboard/patient-dashboard';
import { PatientDetails } from './patient-details/patient-details';
import { PatientNavbar } from './patient-navbar/patient-navbar';
import { BookAppointment } from './book-appointment/book-appointment';
import { CancelAppointment } from './cancel-appointment/cancel-appointment';
import { AppointmentUpcoming } from './appointment-upcoming/appointment-upcoming';
import { AppointmentOngoing } from './appointment-ongoing/appointment-ongoing';
import { AppointmentHistory } from './appointment-history/appointment-history';

@NgModule({
  declarations: [
    PatientDashboard,
    PatientDetails,
    PatientNavbar,
    BookAppointment,
    CancelAppointment,
    AppointmentUpcoming,
    AppointmentOngoing,
    AppointmentHistory
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule
  ],
  exports: [
    PatientDashboard,
    PatientDetails,
    PatientNavbar,
    BookAppointment,
    CancelAppointment,
    AppointmentUpcoming,
    AppointmentOngoing,
    AppointmentHistory
  ]
})
export class PatientModule { }