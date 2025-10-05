import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ApiService } from '../../services/api';
import jsPDF from 'jspdf';

@Component({
  selector: 'app-invoice-form',
  standalone:false,
  templateUrl: './invoice-form.html',
  styleUrls: ['./invoice-form.css']
})
export class InvoiceForm implements OnInit {
  invoiceForm: FormGroup;
  appointments: any[] = [];
  isLoading: boolean = false;

  constructor(private fb: FormBuilder, private api: ApiService) {
    this.invoiceForm = this.fb.group({
      AppointmentId: ['', Validators.required],
      Amount: [0, [Validators.required, Validators.min(0)]],
      Description: ['', Validators.required],
      IssueDate: ['', Validators.required]
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

  onSubmit() {
    if (this.invoiceForm.valid) {
      this.isLoading = true;
      this.api.submitInvoice(this.invoiceForm.value.AppointmentId, this.invoiceForm.value).subscribe({
        next: () => {
          alert('Invoice submitted successfully');
          this.downloadPdf();
          this.isLoading = false;
        },
        error: (error) => {
          alert('Error submitting invoice: ' + (error.error?.Message || 'Unknown error'));
          this.isLoading = false;
        }
      });
    }
  }

  downloadPdf() {
    const doc = new jsPDF();
    const form = this.invoiceForm.value;
    doc.text('Debugging Doctors Hospital', 105, 10, { align: 'center' });
    doc.text(`Invoice for Appointment #${form.AppointmentId}`, 10, 20);
    doc.text(`Amount: $${form.Amount}`, 10, 30);
    doc.text(`Description: ${form.Description}`, 10, 40);
    doc.text(`Issue Date: ${form.IssueDate}`, 10, 50);
    doc.save(`invoice_${form.AppointmentId}.pdf`);
  }
}