import { Component, OnInit } from '@angular/core';
import { ApiService } from '../../services/api';

@Component({
  selector: 'app-patient-requests',
  standalone:false,
  templateUrl: './patient-requests.html',
  styleUrls: ['./patient-requests.css']
})
export class PatientRequests implements OnInit {
  pendingPatients: any[] = [];

  constructor(private api: ApiService) {}

  ngOnInit() {
    this.api.getPendingPatients().subscribe(
      data => this.pendingPatients = data,
      error => alert('Error fetching pending patients')
    );
  }

  approve(id: number, approve: boolean) {
    this.api.approve({ Id: id, Role: 'Patient', IsApproved: approve }).subscribe(
      () => {
        alert(approve ? 'Patient Approved' : 'Patient Declined');
        this.ngOnInit();
      },
      error => alert('Error processing approval')
    );
  }
}