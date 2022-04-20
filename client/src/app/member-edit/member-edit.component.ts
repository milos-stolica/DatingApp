import { Component, HostListener, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { take } from 'rxjs/operators';
import { Member } from 'src/models/member';
import { AccountService } from 'src/services/account.service';
import { MembersService } from 'src/services/members.service';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css']
})
export class MemberEditComponent implements OnInit {
  @ViewChild('editForm') editForm: NgForm;

  @HostListener('window:beforeunload', ['$event']) unloadNotification($event: any) {
    if(this.editForm.dirty) {
      $event.returnValue = true;
    }
  }

  member: Member;

  constructor(private accountService: AccountService,
              private memberService: MembersService,
              private toastr: ToastrService) { }

  ngOnInit(): void {
    this.setLoggedInMember();
  }

  setLoggedInMember() {
    this.accountService.currentUser$
      .pipe(take(1))
      .subscribe(user => {
        this.setMember(user.username);
      });
  }

  setMember(username: string) {
    this.memberService.getMember(username)
      .subscribe(member => this.member = member)
  }

  updateMember() {
    this.memberService.updateMember(this.member)
      .subscribe(() => {
        this.toastr.success('Updated successfully');
        this.editForm.reset(this.member);
      });
  }

}
