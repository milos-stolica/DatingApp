import { PaginationParams } from "./paginationParams";

export class LikeParams extends PaginationParams {
  predicate: string = 'liked';

  constructor() {
    super(5, 1);
  }
}