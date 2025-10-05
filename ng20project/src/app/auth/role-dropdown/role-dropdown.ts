import { Component, EventEmitter, Output } from '@angular/core';

@Component({
  selector: 'app-role-dropdown',
  standalone:false,
  templateUrl: './role-dropdown.html',
  styleUrls: ['./role-dropdown.css']
})
export class RoleDropdown {
  @Output() roleSelected = new EventEmitter<string>();

  onRoleChange(event: Event) {
    const selectElement = event.target as HTMLSelectElement;
    if (selectElement) {
      this.roleSelected.emit(selectElement.value);
    }
  }
}