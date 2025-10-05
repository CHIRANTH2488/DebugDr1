import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormArray, Validators } from '@angular/forms';
import { ApiService } from '../../services/api';
import jsPDF from 'jspdf';

@Component({
  selector: 'app-prescription-form',
  standalone:false,
  templateUrl: './prescription-form.html',
  styleUrls: ['./prescription-form.css']
})
export class PrescriptionForm implements OnInit {
  prescriptionForm: FormGroup;
  appointments: any[] = [];
  isLoading: boolean = false;

  constructor(private fb: FormBuilder, private api: ApiService) {
    this.prescriptionForm = this.fb.group({
      AppointmentId: ['', Validators.required],
      ChiefComplaints: ['', Validators.required],
      PastHistory: [''],
      Examination: [''],
      Advice: [''],
      Signature: ['', Validators.required],
      Medications: this.fb.array([])
    });
  }

  ngOnInit() {
    this.isLoading = true;
    this.api.getAppointments().subscribe({
      next: (data) => {
        this.appointments = data.filter((a: any) => a.isApproved && a.appointmentStatus === 'Ongoing');
        this.isLoading = false;
      },
      error: () => {
        alert('Error fetching appointments');
        this.isLoading = false;
      }
    });
  }

  get medications(): FormArray {
    return this.prescriptionForm.get('Medications') as FormArray;
  }

  addMedication() {
    this.medications.push(this.fb.group({
      Name: ['', Validators.required],
      Timing: ['', Validators.required],
      BeforeAfterFood: ['', Validators.required]
    }));
  }

  removeMedication(index: number) {
    this.medications.removeAt(index);
  }

  onSubmit() {
    if (this.prescriptionForm.valid) {
      this.isLoading = true;
      this.api.submitPrescription(this.prescriptionForm.value.AppointmentId, this.prescriptionForm.value).subscribe({
        next: () => {
          alert('Prescription submitted successfully');
          this.downloadPdf();
          this.isLoading = false;
        },
        error: (error) => {
          alert('Error submitting prescription: ' + (error.error?.Message || 'Unknown error'));
          this.isLoading = false;
        }
      });
    }
  }

  downloadPdf() {
    const doc = new jsPDF();
    const form = this.prescriptionForm.value;
    doc.text('Debugging Doctors Hospital', 105, 10, { align: 'center' });
    doc.text(`Prescription for Appointment #${form.AppointmentId}`, 10, 20);
    doc.text(`Chief Complaints: ${form.ChiefComplaints}`, 10, 30);
    doc.text(`Past History: ${form.PastHistory || 'None'}`, 10, 40);
    doc.text(`Examination: ${form.Examination || 'None'}`, 10, 50);
    doc.text('Medications:', 10, 60);
    form.Medications.forEach((med: any, index: number) => {
      doc.text(`${index + 1}. ${med.Name} - ${med.Timing} (${med.BeforeAfterFood})`, 20, 70 + index * 10);
    });
    doc.text(`Advice: ${form.Advice || 'None'}`, 10, 70 + form.Medications.length * 10);
    doc.text(`Signature: ${form.Signature}`, 10, 80 + form.Medications.length * 10);
    doc.save(`prescription_${form.AppointmentId}.pdf`);
  }
}