import { Component, OnInit } from '@angular/core';
import { ApiService } from '../../services/api';

@Component({
  selector: 'app-appointment-history',
  standalone:false,
  templateUrl: './appointment-history.html',
  styleUrls: ['./appointment-history.css']
})
export class AppointmentHistory implements OnInit {
  appointments: any[] = [];

  constructor(private api: ApiService) {}

  ngOnInit() {
    this.api.getAppointments().subscribe(data => {
      this.appointments = data.filter((a: any) => a.appointmentStatus === 'Finished' || a.appointmentStatus === 'Cancelled');
    });
  }
}