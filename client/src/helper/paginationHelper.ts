import { HttpClient, HttpParams } from "@angular/common/http";
import { map } from "rxjs/operators";
import { PaginatedResult } from "src/models/pagination";

export function getPaginationHttpParams(pageSize: number, currentPage: number) {
  let params = new HttpParams();

  params = params.append('pageSize', pageSize)
                 .append('pageNumber', currentPage);

  return params;
}

export function getPaginatedResult<T>(url: string, params: HttpParams, http: HttpClient) {
  return http.get<T>(url, { observe: 'response', params }).pipe(
    map(response => {
      let paginatedResult: PaginatedResult<T> = new PaginatedResult<T>();
      paginatedResult.result = response.body;
      if (response.headers.get('Pagination') !== null) {
        paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
      }

      return paginatedResult;
    })
  );
}