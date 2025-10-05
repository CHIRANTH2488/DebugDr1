import { Component, OnInit } from '@angular/core';
import { ApiService } from '../../services/api';
import { AuthService } from '../../services/auth';

@Component({
  selector: 'app-patient-details',
  standalone:false,
  templateUrl: './patient-details.html',
  styleUrls: ['./patient-details.css']
})
export class PatientDetails implements OnInit {
  patient: any = null;

  constructor(private api: ApiService, private authService: AuthService) {}

  ngOnInit() {
    const userId = this.authService.getRole() === 'Patient' ? parseInt(this.authService.jwtHelper.decodeToken(localStorage.getItem('token')!)['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier']) : 0;
    this.api.getPatients().subscribe(data => {
      this.patient = data.find((p: any) => p.userId === userId);
    });
  }
}