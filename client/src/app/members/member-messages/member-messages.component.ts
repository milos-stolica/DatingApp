import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { CreateMessage } from 'src/models/createMessage';
import { Message } from 'src/models/message';
import { MessagesService } from 'src/services/messages.service';

@Component({
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css']
})
export class MemberMessagesComponent implements OnInit {
  @ViewChild('sendMessageForm') sendMessageForm: NgForm;
  @Input() username: string;
  @Input() messages: Message[];
  messageContent: string;

  constructor(private messagesService: MessagesService) { }

  ngOnInit(): void {
    
  }

  sendMessage() {
    let message = new CreateMessage(this.username, this.messageContent);

    this.messagesService.sendMessage(message)
      .subscribe(message => {
        this.messages.push(message);
        this.sendMessageForm.resetForm();
      })
  }

}
