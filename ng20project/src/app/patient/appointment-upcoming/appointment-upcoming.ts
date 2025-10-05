import { Component, OnInit } from '@angular/core';
import { ApiService } from '../../services/api';

@Component({
  selector: 'app-appointment-upcoming',
  standalone:false,
  templateUrl: './appointment-upcoming.html',
  styleUrls: ['./appointment-upcoming.css']
})
export class AppointmentUpcoming implements OnInit {
  appointments: any[] = [];

  constructor(private api: ApiService) {}

  ngOnInit() {
    this.api.getPatientAppointments().subscribe(data => {
      this.appointments = data.filter((a: any) => a.isApproved && a.appointmentStatus === 'Scheduled');
    });
  }
}