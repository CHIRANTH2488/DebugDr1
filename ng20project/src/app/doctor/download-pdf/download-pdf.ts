import { Component, OnInit } from '@angular/core';
import { ApiService } from '../../services/api';
import jsPDF from 'jspdf';

@Component({
  selector: 'app-download-pdf',
  standalone:false,
  templateUrl: './download-pdf.html',
  styleUrls: ['./download-pdf.css']
})
export class DownloadPdf implements OnInit {
  appointments: any[] = [];

  constructor(private api: ApiService) {}

  ngOnInit() {
    this.api.getAppointments().subscribe(data => {
      this.appointments = data.filter((a: any) => a.appointmentStatus === 'Finished' && (a.prescription || a.invoice));
    });
  }

  downloadPrescription(appointmentId: number) {
    this.api.getPatientDataForAppointment(appointmentId).subscribe(data => {
      const doc = new jsPDF();
      const { prescription } = data;
      doc.text('Debugging Doctors Hospital', 105, 10, { align: 'center' });
      doc.text(`Prescription for Appointment #${appointmentId}`, 10, 20);
      doc.text(`Chief Complaints: ${prescription.chiefComplaints}`, 10, 30);
      doc.text(`Past History: ${prescription.pastHistory || 'None'}`, 10, 40);
      doc.text(`Examination: ${prescription.examination || 'None'}`, 10, 50);
      doc.text('Medications:', 10, 60);
      prescription.medications.forEach((med: any, index: number) => {
        doc.text(`${index + 1}. ${med.name} - ${med.timing} (${med.beforeAfterFood})`, 20, 70 + index * 10);
      });
      doc.text(`Advice: ${prescription.advice || 'None'}`, 10, 70 + prescription.medications.length * 10);
      doc.text(`Signature: ${prescription.signature}`, 10, 80 + prescription.medications.length * 10);
      doc.save(`prescription_${appointmentId}.pdf`);
    });
  }

  downloadInvoice(appointmentId: number) {
    this.api.getPatientDataForAppointment(appointmentId).subscribe(data => {
      const doc = new jsPDF();
      const { invoice } = data;
      doc.text('Debugging Doctors Hospital', 105, 10, { align: 'center' });
      doc.text(`Invoice for Appointment #${appointmentId}`, 10, 20);
      doc.text(`Amount: $${invoice.amount}`, 10, 30);
      doc.text(`Description: ${invoice.description}`, 10, 40);
      doc.text(`Issue Date: ${invoice.issueDate}`, 10, 50);
      doc.save(`invoice_${appointmentId}.pdf`);
    });
  }
}