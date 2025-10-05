import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AdminDashboard } from './admin-dashboard/admin-dashboard';
import { DoctorRequests } from './doctor-requests/doctor-requests';
import { PatientRequests } from './patient-requests/patient-requests';
import { DoctorList } from './doctor-list/doctor-list';
import { PatientList } from './patient-list/patient-list';
import { AppointmentOngoing } from './appointment-ongoing/appointment-ongoing';
import { AppointmentUpcoming } from './appointment-upcoming/appointment-upcoming';
import { AppointmentFinished } from './appointment-finished/appointment-finished';
import { AdminNavbar } from './admin-navbar/admin-navbar';

@NgModule({
  declarations: [
    AdminDashboard,
    DoctorRequests,
    PatientRequests,
    DoctorList,
    PatientList,
    AppointmentOngoing,
    AppointmentUpcoming,
    AppointmentFinished,
    AdminNavbar
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule
  ],
  exports: [
    AdminDashboard,
    DoctorRequests,
    PatientRequests,
    DoctorList,
    PatientList,
    AppointmentOngoing,
    AppointmentUpcoming,
    AppointmentFinished,
    AdminNavbar
  ]
})
export class AdminModule { }