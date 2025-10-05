import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AuthService } from '../../services/auth';

@Component({
  selector: 'app-register',
  standalone:false,
  templateUrl: './register.html',
  styleUrls: ['./register.css']
})
export class Register {
  registerForm: FormGroup;
  role: string = '';

  constructor(private fb: FormBuilder, private authService: AuthService) {
    this.registerForm = this.fb.group({
      Email: ['', [Validators.required, Validators.email]],
      PswdHash: ['', Validators.required],
      FullName: [''],
      Specialisation: [''],
      Hpid: [''],
      Availability: [''],
      ContactNo: [''],
      Dob: [''],
      Gender: [''],
      Address: [''],
      AadhaarNo: ['']
    });
  }

  onRoleChange(role: string) {
    this.role = role;
    const controls = this.registerForm.controls;
    if (role === 'Doctor') {
      controls['FullName'].setValidators([Validators.required]);
      controls['Specialisation'].setValidators([Validators.required]);
      controls['Hpid'].setValidators([Validators.required]);
      controls['Availability'].setValidators([Validators.required]);
      controls['ContactNo'].setValidators([Validators.required]);
      controls['Dob'].clearValidators();
      controls['Gender'].clearValidators();
      controls['Address'].clearValidators();
      controls['AadhaarNo'].clearValidators();
    } else if (role === 'Patient') {
      controls['FullName'].setValidators([Validators.required]);
      controls['Dob'].setValidators([Validators.required]);
      controls['Gender'].setValidators([Validators.required]);
      controls['ContactNo'].setValidators([Validators.required]);
      controls['Address'].setValidators([Validators.required]);
      controls['AadhaarNo'].setValidators([Validators.required]);
      controls['Specialisation'].clearValidators();
      controls['Hpid'].clearValidators();
      controls['Availability'].clearValidators();
    } else {
      Object.keys(controls).forEach(key => controls[key].clearValidators());
    }
    Object.keys(controls).forEach(key => controls[key].updateValueAndValidity());
  }

  onSubmit() {
    if (this.registerForm.valid && this.role) {
      const dto = { ...this.registerForm.value, Role: this.role };
      this.authService.register(dto).subscribe({
        next: () => alert('Registration successful, awaiting approval'),
        error: (error) => alert('Registration failed: ' + (error.error?.Message || 'Unknown error'))
      });
    } else {
      alert('Please fill out the form correctly and select a role.');
    }
  }
}