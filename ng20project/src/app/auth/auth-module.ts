import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { Login } from './login/login';
import { Register } from './register/register';
import { RoleDropdown } from './role-dropdown/role-dropdown';

@NgModule({
  declarations: [
    Login,
    Register,
    RoleDropdown
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule
  ],
  exports: [
    Login,
    Register,
    RoleDropdown
  ]
})
export class AuthModule { }