import { Component, OnInit } from '@angular/core';
import { Message } from 'src/models/message';
import { MessageParams } from 'src/models/messageParams';
import { MessagesService } from 'src/services/messages.service';

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.css']
})
export class MessagesComponent implements OnInit {

  constructor(private messagesService: MessagesService) { }
  messages: Message[];
  messageParams: MessageParams;
  totalCount: number;
  maxSize: number = 10;
  loading: boolean = false;

  ngOnInit(): void {
    this.initMessageParams();
    this.loadMessages();
  }

  loadMessages() {
    this.loading = true;
    this.messagesService.getMessages(this.messageParams)
      .subscribe(response => 
      {
        this.messages = response.result;
        this.totalCount = response.pagination.totalCount
        this.loading = false;
      });
  }

  pageChanged(event) {
    if(this.messageParams.currentPage !== event.page) {
      this.messageParams.currentPage = event.page;
      this.loadMessages();
    }
  }

  initMessageParams() {
    this.messageParams = new MessageParams();
    this.messageParams.container = 'Unread';
  }

  deleteMessage(id: number) {
    this.messagesService.deleteMessage(id)
      .subscribe(() => {
        this.messages.splice(this.messages.findIndex(message => message.id === id), 1);
      })
  }

}
