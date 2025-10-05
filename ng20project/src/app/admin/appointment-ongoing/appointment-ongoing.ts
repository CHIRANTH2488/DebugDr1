import { Component, OnInit } from '@angular/core';
import { ApiService } from '../../services/api';

@Component({
  selector: 'app-appointment-ongoing',
  standalone:false,
  templateUrl: './appointment-ongoing.html',
  styleUrls: ['./appointment-ongoing.css']
})
export class AppointmentOngoing implements OnInit {
  appointments: any[] = [];

  constructor(private api: ApiService) {}

  ngOnInit() {
    this.api.getAppointments().subscribe(data => {
      this.appointments = data.filter((a: any) => a.isApproved && a.appointmentStatus === 'Ongoing');
    });
  }
}