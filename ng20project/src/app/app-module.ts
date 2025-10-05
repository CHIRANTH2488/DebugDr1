import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { JwtModule } from '@auth0/angular-jwt';
import { App } from './app';
import { AuthModule } from './auth/auth-module';
import { AdminModule } from './admin/admin-module';
import { DoctorModule } from './doctor/doctor-module';
import { PatientModule } from './patient/patient-module';
import { PublicModule } from './public/public-module';
import { AuthService } from './services/auth';
import { ApiService } from './services/api';

@NgModule({
  declarations: [
    App
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    NgbModule,
    JwtModule.forRoot({
      config: {
        tokenGetter: () => localStorage.getItem('token')
      }
    }),
    AuthModule,
    AdminModule,
    DoctorModule,
    PatientModule,
    PublicModule
  ],
  providers: [AuthService, ApiService],
  bootstrap: [App]
})
export class AppModule { }