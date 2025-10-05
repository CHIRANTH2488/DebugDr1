import { Component, EventEmitter, Output } from '@angular/core';

@Component({
  selector: 'app-main-portal',
  standalone:false,
  templateUrl: './main-portal.html',
  styleUrls: ['./main-portal.css']
})
export class MainPortal {
  @Output() viewChange = new EventEmitter<string>();
}