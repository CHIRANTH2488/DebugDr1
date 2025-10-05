import { Component, OnInit } from '@angular/core';
import { ApiService } from '../../services/api';

@Component({
  selector: 'app-our-doctors',
  standalone:false,
  templateUrl: './our-doctors.html',
  styleUrls: ['./our-doctors.css']
})
export class OurDoctors implements OnInit {
  doctors: any[] = [];

  constructor(private api: ApiService) {}

  ngOnInit() {
    this.api.getDoctors().subscribe(data => this.doctors = data);
  }
}