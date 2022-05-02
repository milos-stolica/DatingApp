import { PaginationParams } from "./paginationParams";

export class MessageParams extends PaginationParams {
  container: string = 'Unread';

  constructor() {
    super(5, 1);
  }
}