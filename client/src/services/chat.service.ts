import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { BehaviorSubject } from 'rxjs';
import { take } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { CreateMessage } from 'src/models/createMessage';
import { Message } from 'src/models/message';
import { User } from 'src/models/user';
import { AccountService } from './account.service';

@Injectable({
  providedIn: 'root'
})
export class ChatService {
  private hubUrl: string = environment.hubUrl;
  private hubConnection: HubConnection;
  private messagesSource = new BehaviorSubject<Message[]>([]);
  private user: User;

  messages$ = this.messagesSource.asObservable();
  
  
  constructor(private accountService: AccountService) { 
    this.accountService.currentUser$
      .pipe(take(1))
      .subscribe(user => this.user = user)
  }

  startConnection(recipientUsername: string) {
    this.createConnection(this.user, recipientUsername);
    this.tryStartConnection();
    this.listenToCallerConnected();
    this.listenToNewMessages();
    this.listenToParticipantConnected();
    console.log("Chat hub connection created");
  }

  stopConnection() {
    if(this.hubConnection) {
      this.hubConnection
        .stop()
        .then(() => console.log("Chat hub connection closed"))
        .catch(error => console.log(error))
        .finally(() => this.messagesSource.next([]))
        ;
    }
  }

  async sendMessage(message: CreateMessage) {
    if(this.hubConnection) {
      return this.hubConnection.invoke("SendMessage", message)
        .catch(error => console.log(`Fail sending message to ${message.recipientUsername}. Error: ${error}`));
    }

    return Promise.reject("Connection is closed.");
  }

  private createConnection(user: User, recipientUsername: string) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + 'message?user=' + recipientUsername, {
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

  private listenToCallerConnected() {
    this.hubConnection.on("GetMessageThreadOnConnected", (messages: Message[]) => {
      this.messagesSource.next(messages);
    });
  }

  private listenToParticipantConnected() {
    this.hubConnection.on("NewUserInGroup", (username: string) => {
      console.log("New user in group: " + username);
      this.messages$.pipe(take(1))
        .subscribe(messages => {
          messages.forEach(message => {
            if(message.dateRead == null && message.recipientUsername == username) {
              message.dateRead = new Date(Date.now());
            }
          });
          this.messagesSource.next([...messages]);
        })
    });
  }

  private listenToNewMessages() {
    this.hubConnection.on("NewMessage", message => {
      this.messages$
        .pipe(take(1))
        .subscribe(messages => {
          this.messagesSource.next([...messages, message]);
        });
    });

    // this.hubConnection.onclose((error) => {
    //   console.log(`Closing socket...Error: ${error}`);
    // });

    // this.hubConnection.onreconnecting((error) => {
    //   console.log(`Reconnecting socket...Error: ${error}`);
    // });
  }
}
