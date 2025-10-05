import { Component } from '@angular/core';
import { AuthService } from './services/auth';

@Component({
  selector: 'app-root',
  standalone: false,
  templateUrl: './app.html',
  styleUrls: ['./app.css']
})
export class App {
  currentView: string = 'home';

  constructor(public authService: AuthService) {}

  logout() {
    this.authService.logout();
    this.currentView = 'home';
  }

  setView(view: string) {
    this.currentView = view;
  }
}