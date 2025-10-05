import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ApiService } from '../../services/api';

@Component({
  selector: 'app-book-appointment',
  standalone:false,
  templateUrl: './book-appointment.html',
  styleUrls: ['./book-appointment.css']
})
export class BookAppointment implements OnInit {
  appointmentForm: FormGroup;
  doctors: any[] = [];
  isLoading: boolean = false;

  constructor(private fb: FormBuilder, private api: ApiService) {
    this.appointmentForm = this.fb.group({
      DoctorId: ['', Validators.required],
      AppointmentDate: ['', Validators.required]
    });
  }

  ngOnInit() {
    this.isLoading = true;
    this.api.getDoctors().subscribe({
      next: (data) => {
        this.doctors = data;
        this.isLoading = false;
      },
      error: () => {
        alert('Error fetching doctors');
        this.isLoading = false;
      }
    });
  }

  onSubmit() {
    if (this.appointmentForm.valid) {
      this.isLoading = true;
      this.api.bookAppointment(this.appointmentForm.value).subscribe({
        next: () => {
          alert('Appointment booked successfully');
          this.appointmentForm.reset();
          this.isLoading = false;
        },
        error: (error) => {
          alert('Error booking appointment: ' + (error.error?.Message || 'Unknown error'));
          this.isLoading = false;
        }
      });
    }
  }
}