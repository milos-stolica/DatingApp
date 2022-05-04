import { Component, OnInit } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';
import { take } from 'rxjs/operators';
import { RolesModalComponent } from 'src/app/modals/roles-modal/roles-modal.component';
import { UserRoles } from 'src/models/userRoles';
import { AdminService } from 'src/services/admin.service';

@Component({
  selector: 'app-user-management',
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.css']
})
export class UserManagementComponent implements OnInit {
  users: UserRoles[] = [];
  bsModalRef?: BsModalRef;

  constructor(private adminService: AdminService, 
              private modalService: BsModalService,
              private toastr: ToastrService) { }

  ngOnInit(): void {
    this.getUsersWithRoles();
  }

  private getUsersWithRoles() {
    this.adminService.getUsersWithRoles()
      .pipe(take(1))
      .subscribe(userRoles => this.users = userRoles);
  }

  openRolesModal(user: UserRoles) {
    const config = {
      class: 'modal-dialog-centered',
      initialState: {
        user,
        roles: this.getRolesArray(user)
      }
    }

    this.bsModalRef = this.modalService.show(RolesModalComponent, config);
    this.bsModalRef.content.updateSelectedRoles.subscribe((roles: any[]) => {
      const rolesToUpdate = [...roles.filter(role => role.checked).map(role => role.name)];
      
      //bug when change his role, than it is not changed in accountService and user can open admin page themselves,
      //even if he is not admin anymore
      if(rolesToUpdate.length > 0) {
        this.adminService.updateRoles(user.username, rolesToUpdate)
          .subscribe(newRoles => user.roles = newRoles);
      }
      else {
        this.toastr.error("At least one role should be selected");
      }
    });
  }

  getRolesArray(user: UserRoles) {
    const roles = [];
    const userRoles = user.roles;
    const availableRoles: any[] = [
      {name:'Admin', value: 'Admin'},
      {name:'Moderator', value: 'Moderator'},
      {name:'Member', value: 'Member'}
    ];

    availableRoles.forEach(role => {
      userRoles.includes(role.name) ? role.checked = true : role.checked = false;
      roles.push(role);
    });

    console.log(roles);
    return roles;
  }
}
