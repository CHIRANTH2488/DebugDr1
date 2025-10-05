import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private baseUrl = 'https://localhost:7274/api';

  constructor(private http: HttpClient) {}

  private getHeaders(): HttpHeaders {
    const token = localStorage.getItem('token');
    return new HttpHeaders({
      Authorization: `Bearer ${token}`
    });
  }

  getDoctors(): Observable<any> {
    return this.http.get(`${this.baseUrl}/doctors`, { headers: this.getHeaders() });
  }

  getPendingDoctors(): Observable<any> {
    return this.http.get(`${this.baseUrl}/users/PendingDoctors`, { headers: this.getHeaders() });
  }

  getPendingPatients(): Observable<any> {
    return this.http.get(`${this.baseUrl}/users/PendingPatients`, { headers: this.getHeaders() });
  }

  approve(dto: any): Observable<any> {
    return this.http.put(`${this.baseUrl}/users/Approve`, dto, { headers: this.getHeaders() });
  }

  getPatients(): Observable<any> {
    return this.http.get(`${this.baseUrl}/patients`, { headers: this.getHeaders() });
  }

  bookAppointment(appointment: any): Observable<any> {
    return this.http.post(`${this.baseUrl}/appointments/Book`, appointment, { headers: this.getHeaders() });
  }

  cancelAppointment(id: number): Observable<any> {
    return this.http.put(`${this.baseUrl}/appointments/Cancel/${id}`, {}, { headers: this.getHeaders() });
  }

  submitPrescription(id: number, prescription: any): Observable<any> {
    return this.http.post(`${this.baseUrl}/appointments/Prescription/${id}`, prescription, { headers: this.getHeaders() });
  }

  submitInvoice(id: number, invoice: any): Observable<any> {
    return this.http.post(`${this.baseUrl}/appointments/Invoice/${id}`, invoice, { headers: this.getHeaders() });
  }

  getAppointments(): Observable<any> {
    return this.http.get(`${this.baseUrl}/appointments`, { headers: this.getHeaders() });
  }

  getPatientAppointments(): Observable<any> {
    return this.http.get(`${this.baseUrl}/patients/Appointments`, { headers: this.getHeaders() });
  }

  getPatientDataForAppointment(id: number): Observable<any> {
    return this.http.get(`${this.baseUrl}/appointments/${id}/PatientData`, { headers: this.getHeaders() });
  }
}