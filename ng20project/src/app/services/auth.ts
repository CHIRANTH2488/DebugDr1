import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { JwtHelperService } from '@auth0/angular-jwt';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private baseUrl = 'https://localhost:7274/api/auth';

  constructor(private http: HttpClient, public jwtHelper: JwtHelperService) {}

  login(credentials: { Email: string; PswdHash: string }): Observable<any> {
    return this.http.post(`${this.baseUrl}/login`, credentials).pipe(
      tap((response: any) => {
        localStorage.setItem('token', response.Token);
      })
    );
  }

  register(dto: any): Observable<any> {
    return this.http.post(`${this.baseUrl}/register`, dto);
  }

  isLoggedIn(): boolean {
    const token = localStorage.getItem('token');
    return !!token && !this.jwtHelper.isTokenExpired(token);
  }

  getRole(): string {
    const token = localStorage.getItem('token');
    if (token) {
      const decoded = this.jwtHelper.decodeToken(token);
      return decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
    }
    return '';
  }

  logout() {
    localStorage.removeItem('token');
  }
}