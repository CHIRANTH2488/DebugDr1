import { Component, OnInit } from '@angular/core';
import { ApiService } from '../../services/api';

@Component({
  selector: 'app-doctor-list',
  standalone:false,
  templateUrl: './doctor-list.html',
  styleUrls: ['./doctor-list.css']
})
export class DoctorList implements OnInit {
  doctors: any[] = [];

  constructor(private api: ApiService) {}

  ngOnInit() {
    this.api.getDoctors().subscribe(data => this.doctors = data);
  }
}