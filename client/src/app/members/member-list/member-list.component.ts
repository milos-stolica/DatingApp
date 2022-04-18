import { Component, OnInit } from '@angular/core';
import { Member } from 'src/models/member';
import { MembersService } from 'src/services/members.service';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {
  members: Member[] = [];

  constructor(private membersService: MembersService) { }

  ngOnInit(): void {
    this.loadUsers();
  }

  loadUsers() {
    this.membersService.getMembers()
      .subscribe(members => this.members = members);
  }
}
