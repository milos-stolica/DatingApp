import { PaginationParams } from "./paginationParams";
import { User } from "./user";

export class UserParams extends PaginationParams {
  gender: string;
  minAge: number = 18;
  maxAge: number = 99;
  
  orderBy: string = 'lastActive';

  constructor(user: User) {
    super(5, 1);
    this.gender = user.gender === 'female' ? 'male' : 'female';
  }
}