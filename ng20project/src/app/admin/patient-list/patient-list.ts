import { Component, OnInit } from '@angular/core';
import { ApiService } from '../../services/api';

@Component({
  selector: 'app-patient-list',
  standalone:false,
  templateUrl: './patient-list.html',
  styleUrls: ['./patient-list.css']
})
export class PatientList implements OnInit {
  patients: any[] = [];

  constructor(private api: ApiService) {}

  ngOnInit() {
    this.api.getPatients().subscribe(data => this.patients = data);
  }
}