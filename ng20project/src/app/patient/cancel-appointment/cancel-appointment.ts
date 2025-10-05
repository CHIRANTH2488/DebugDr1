import { Component, OnInit } from '@angular/core';
import { ApiService } from '../../services/api';

@Component({
  selector: 'app-cancel-appointment',
  standalone:false,
  templateUrl: './cancel-appointment.html',
  styleUrls: ['./cancel-appointment.css']
})
export class CancelAppointment implements OnInit {
  appointments: any[] = [];

  constructor(private api: ApiService) {}

  ngOnInit() {
    this.api.getPatientAppointments().subscribe(data => {
      this.appointments = data.filter((a: any) => a.isApproved && a.appointmentStatus === 'Scheduled');
    });
  }

  cancel(appointmentId: number) {
    this.api.cancelAppointment(appointmentId).subscribe(
      () => {
        alert('Appointment cancelled successfully');
        this.ngOnInit();
      },
      error => alert('Error cancelling appointment: ' + error.error.Message)
    );
  }
}