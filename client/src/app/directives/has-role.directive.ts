import { Directive, Input, OnInit, TemplateRef, ViewContainerRef } from '@angular/core';
import { take } from 'rxjs/operators';
import { User } from 'src/models/user';
import { AccountService } from 'src/services/account.service';

@Directive({
  selector: '[appHasRole]'
})
export class HasRoleDirective implements OnInit {
  @Input() appHasRole: string[];
  user: User;

  constructor(private viewContainerRef: ViewContainerRef,
              private templateRef: TemplateRef<any>,
              private accountService: AccountService) {

    this.accountService.currentUser$
       //.pipe(take(1)) - with this part this also works but accidentally (it wouldnt without ngIf and checking for user in template)
       .subscribe(user => {
          this.user = user;
          this.checkForUserRoles();
       });
  }

  ngOnInit(): void {
    this.checkForUserRoles();
  }

  private checkForUserRoles() {
    if(this.user === null || this.user.roles === null) {
      this.viewContainerRef.clear();
      return;
    }

    if(!this.user?.roles.some(role => this.appHasRole?.includes(role))) {
      this.viewContainerRef.clear();
      return;
    }

    this.viewContainerRef.createEmbeddedView(this.templateRef);
  }

}
