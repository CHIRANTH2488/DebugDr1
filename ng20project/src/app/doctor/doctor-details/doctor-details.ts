import { Component, OnInit } from '@angular/core';
import { ApiService } from '../../services/api';
import { AuthService } from '../../services/auth';

@Component({
  selector: 'app-doctor-details',
  standalone:false,
  templateUrl: './doctor-details.html',
  styleUrls: ['./doctor-details.css']
})
export class DoctorDetails implements OnInit {
  doctor: any = null;

  constructor(private api: ApiService, private authService: AuthService) {}

  ngOnInit() {
    const userId = this.authService.getRole() === 'Doctor' ? parseInt(this.authService.jwtHelper.decodeToken(localStorage.getItem('token')!)['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier']) : 0;
    this.api.getDoctors().subscribe(data => {
      this.doctor = data.find((d: any) => d.userId === userId);
    });
  }
}