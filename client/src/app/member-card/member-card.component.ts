import { Component, Input, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Member } from 'src/models/member';
import { MembersService } from 'src/services/members.service';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css']
})
export class MemberCardComponent implements OnInit {
  @Input() member: Partial<Member>;

  constructor(private membersService: MembersService,
              private toastr: ToastrService) { }

  ngOnInit(): void {
  }

  addLike(member: Partial<Member>) {
    this.membersService.addLike(member.username)
        .subscribe(() => {
          this.toastr.success("Successfully liked " + member.knownAs)
        })
  }
}
