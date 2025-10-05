import { Component, EventEmitter, Output } from '@angular/core';

@Component({
  selector: 'app-admin-navbar',
  standalone:false,
  templateUrl: './admin-navbar.html',
  styleUrls: ['./admin-navbar.css']
})
export class AdminNavbar {
  @Output() viewChange = new EventEmitter<string>();
}