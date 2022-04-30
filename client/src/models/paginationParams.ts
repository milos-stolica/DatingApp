export class PaginationParams {
  currentPage: number = 1;
  pageSize: number = 5;

  constructor(pageSize: number, currentPage: number) {
    this.pageSize = pageSize;
    this.currentPage = currentPage;
  }
}