import { Component, OnInit } from '@angular/core';
import { ApiService } from '../../services/api';

@Component({
  selector: 'app-appointment-finished',
  standalone:false,
  templateUrl: './appointment-finished.html',
  styleUrls: ['./appointment-finished.css']
})
export class AppointmentFinished implements OnInit {
  appointments: any[] = [];

  constructor(private api: ApiService) {}

  ngOnInit() {
    this.api.getAppointments().subscribe(data => {
      this.appointments = data.filter((a: any) => a.appointmentStatus === 'Finished');
    });
  }
}