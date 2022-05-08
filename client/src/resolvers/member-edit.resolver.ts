import { Injectable } from '@angular/core';
import {
  Router, Resolve,
  RouterStateSnapshot,
  ActivatedRouteSnapshot
} from '@angular/router';
import { Observable, of } from 'rxjs';
import { concatAll, map, take } from 'rxjs/operators';
import { Member } from 'src/models/member';
import { AccountService } from 'src/services/account.service';
import { MembersService } from 'src/services/members.service';

@Injectable({
  providedIn: 'root'
})
export class MemberEditResolver implements Resolve<Member> {

  constructor(private accountService: AccountService, private memberService: MembersService) {}

  resolve(): Observable<Member> {
    return this.accountService.currentUser$
      .pipe(take(1))
      .pipe(map(user => this.memberService.getMember(user.username)), concatAll());
  }
}
