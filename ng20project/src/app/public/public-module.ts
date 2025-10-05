import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MainPortal } from './main-portal/main-portal';
import { Home } from './home/home';
import { About } from './about/about';
import { Vision } from './vision/vision';
import { OurDoctors } from './our-doctors/our-doctors';

@NgModule({
  declarations: [
    MainPortal,
    Home,
    About,
    Vision,
    OurDoctors
  ],
  imports: [
    CommonModule
  ],
  exports: [
    MainPortal,
    Home,
    About,
    Vision,
    OurDoctors
  ]
})
export class PublicModule { }