import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { getPaginatedResult, getPaginationHttpParams } from 'src/helper/paginationHelper';
import { CreateMessage } from 'src/models/createMessage';
import { Message } from 'src/models/message';
import { MessageParams } from 'src/models/messageParams';

@Injectable({
  providedIn: 'root'
})
export class MessagesService {
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  getMessages(messageParams: MessageParams) {
    let params = getPaginationHttpParams(messageParams.pageSize, messageParams.currentPage) 
                 .append('Container', messageParams.container);

    return getPaginatedResult<Message[]>(this.baseUrl + 'messages', params, this.http);
  }

  getMessageThread(username: string) {
    return this.http.get<Message[]>(this.baseUrl + 'messages/thread/' + username);
  }

  sendMessage(message: CreateMessage) {
    return this.http.post<Message>(this.baseUrl + 'messages', message);
  }

  deleteMessage(id: number) {
    return this.http.delete(this.baseUrl + 'messages/' + id);
  }
}
