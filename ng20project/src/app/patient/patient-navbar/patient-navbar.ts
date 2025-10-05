import { Component, EventEmitter, Output } from '@angular/core';

@Component({
  selector: 'app-patient-navbar',
  standalone:false,
  templateUrl: './patient-navbar.html',
  styleUrls: ['./patient-navbar.css']
})
export class PatientNavbar {
  @Output() viewChange = new EventEmitter<string>();
}