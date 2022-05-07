import { AfterViewInit, Component, ElementRef, Input, OnInit, QueryList, ViewChild, ViewChildren } from '@angular/core';
import { NgForm } from '@angular/forms';
import { CreateMessage } from 'src/models/createMessage';
import { Message } from 'src/models/message';
import { ChatService } from 'src/services/chat.service';

@Component({
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css']
})
export class MemberMessagesComponent implements OnInit, AfterViewInit {
  @ViewChild('sendMessageForm') sendMessageForm: NgForm;
  @ViewChild('messagesDiv', {static: true}) messagesDiv: ElementRef;
  @ViewChildren('messageItem') messageElements: QueryList<any>;
  @Input() username: string;
  messageContent: string;

  constructor(public chat: ChatService) { }
  
  ngOnInit(): void {
  }

  ngAfterViewInit(): void {
    this.messageElements.changes
      .subscribe(() => this.scrollMessagesToEnd());
  }

  sendMessage() {
    let message = new CreateMessage(this.username, this.messageContent);
    this.chat.sendMessage(message)
      .then(() => this.sendMessageForm.resetForm());
  }

  scrollMessagesToEnd() {
    this.messagesDiv.nativeElement.scroll({
      top: this.messagesDiv.nativeElement.scrollHeight,
      left: 0,
      behavior: 'smooth'
    });
  }

}
