import { Component, OnInit } from '@angular/core';
import { take } from 'rxjs/operators';
import { Member } from 'src/models/member';
import { Pagination } from 'src/models/pagination';
import { MembersService } from 'src/services/members.service';
import { UserParamsService } from 'src/services/user-params.service';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {
  members: Member[];
  pagination: Pagination;
  maxSize: number = 10;

  genderList = [{value: 'male', display: 'Males'},
                {value: 'female', display: 'Females'}];

  constructor(private membersService: MembersService,
              public userParamsService: UserParamsService) { }

  ngOnInit(): void {
    this.loadMembers();
  }

  loadMembers() {
    this.membersService.getMembers(this.userParamsService.userParams)
      .pipe(take(1))
      .subscribe(response => {
        this.members = response.result;
        this.pagination = response.pagination;
      });
  }

  pageChanged(event: any) {
    this.userParamsService.userParams.currentPage = event.page;
    this.loadMembers();
  }

  resetFilters() {
    this.userParamsService.resetUserParams();
    this.loadMembers();
  }
}
