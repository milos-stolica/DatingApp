import { Injectable } from '@angular/core';
import { take } from 'rxjs/operators';
import { User } from 'src/models/user';
import { UserParams } from 'src/models/userParams';
import { AccountService } from './account.service';

@Injectable({
  providedIn: 'root'
})
export class UserParamsService {
  public userParams: UserParams;
  private user: User;

  constructor(private accountService: AccountService) {
    this.accountService.currentUser$
      .pipe(take(1))
      .subscribe(user => {
        this.user = user;
        this.userParams = new UserParams(user);
      })
  }

  resetUserParams() {
    this.userParams = new UserParams(this.user);
  }
}
