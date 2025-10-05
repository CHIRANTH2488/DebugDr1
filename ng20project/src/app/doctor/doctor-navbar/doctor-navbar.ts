import { Component, EventEmitter, Output } from '@angular/core';

@Component({
  selector: 'app-doctor-navbar',
  standalone:false,
  templateUrl: './doctor-navbar.html',
  styleUrls: ['./doctor-navbar.css']
})
export class DoctorNavbar {
  @Output() viewChange = new EventEmitter<string>();
}