import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { ToastrService } from 'ngx-toastr';
import { BehaviorSubject } from 'rxjs';
import { take } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { User } from 'src/models/user';

@Injectable({
  providedIn: 'root'
})
export class PresenceService {
  private hubUrl: string = environment.hubUrl;
  private hubConnection: HubConnection;
  private onlineUsersSource = new BehaviorSubject<string[]>([]);
  onlineUsers$ = this.onlineUsersSource.asObservable();

  constructor(private toastr: ToastrService, private router: Router) { }

  startConnection(user: User) {
    this.createConnection(user);
    this.tryStartConnection();
    this.listenToUserConnected();
    this.listenToUserDisconnected();
    this.listenToCallerConnected();
    this.listenToMessageNotification();
  }

  stopConnection() {
    if(this.hubConnection) {
      this.hubConnection
        .stop()
        .catch(error => console.log(error));
    }
  }

  private listenToUserConnected() {
    this.hubConnection.on("UserIsOnline", username => {
      //this.toastr.success(`User ${username} is online.`)
      this.onlineUsers$.pipe(take(1))
        .subscribe(onlineUsers => {
          this.onlineUsersSource.next([...onlineUsers, username]);
        });

    })
  }

  private listenToUserDisconnected() {
    this.hubConnection.on("UserIsOffline", username => {
      //this.toastr.success(`User ${username} is offline.`)
      this.onlineUsers$.pipe(take(1))
        .subscribe(onlineUsers => {
          this.onlineUsersSource.next([...onlineUsers.filter(userName => userName != username)]);
        });
    })
  }

  private listenToCallerConnected() {
    this.hubConnection.on("SendOnlineUsers", onlineUsers => {
      this.onlineUsersSource.next(onlineUsers);
    })
  }

  private createConnection(user: User) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + 'presence', {
        accessTokenFactory: () => user.token
      })
      .withAutomaticReconnect()
      .build();
  }

  private tryStartConnection() {
    this.hubConnection
      .start()
      .catch(error => console.log(error));
  }

  private listenToMessageNotification() {
    this.hubConnection.on("MessageNotification", ({ username, knownAs }) => {
      this.toastr.info(`User ${knownAs} sent you message. Click to see it`)
      .onTap
      .pipe(take(1))
      .subscribe(() => this.router.navigateByUrl("/members/" + username + "?tab=3"));
    });
  }
}
