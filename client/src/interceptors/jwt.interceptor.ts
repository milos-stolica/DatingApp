import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpHeaders
} from '@angular/common/http';
import { Observable } from 'rxjs';
import { AccountService } from 'src/services/account.service';
import { take } from 'rxjs/operators';

@Injectable()
export class JwtInterceptor implements HttpInterceptor {

  constructor(private accountService: AccountService) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    let jwt: string = '';

    this.accountService.currentUser$.pipe(take(1))
    .subscribe(user => {
      if(user) {
        jwt = user.token;
      }
    })

    if(jwt) {
      request = request.clone({
        headers: new HttpHeaders({
          Authorization: `Bearer ${jwt}`
        })
      })
    }

    return next.handle(request);
  }
}
