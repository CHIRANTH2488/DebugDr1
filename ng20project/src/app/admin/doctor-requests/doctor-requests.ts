import { Component, OnInit } from '@angular/core';
import { ApiService } from '../../services/api';

@Component({
  selector: 'app-doctor-requests',
  standalone:false,
  templateUrl: './doctor-requests.html',
  styleUrls: ['./doctor-requests.css']
})
export class DoctorRequests implements OnInit {
  pendingDoctors: any[] = [];

  constructor(private api: ApiService) {}

  ngOnInit() {
    this.api.getPendingDoctors().subscribe(
      data => this.pendingDoctors = data,
      error => alert('Error fetching pending doctors')
    );
  }

  approve(id: number, approve: boolean) {
    this.api.approve({ Id: id, Role: 'Doctor', IsApproved: approve }).subscribe(
      () => {
        alert(approve ? 'Doctor Approved' : 'Doctor Declined');
        this.ngOnInit();
      },
      error => alert('Error processing approval')
    );
  }
}