export class CreateMessage {
  recipientUsername: string;
  content: string;

  constructor(recipientUsername: string, content: string) {
    this.recipientUsername = recipientUsername;
    this.content = content;
  }
}