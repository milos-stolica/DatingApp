import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { of } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Member } from 'src/models/member';
import { PaginatedResult } from 'src/models/pagination';
import { UserParams } from 'src/models/userParams';

@Injectable({
  providedIn: 'root'
})
export class MembersService {
  private baseUrl = environment.apiUrl;
  private membersCache: Map<string, Member> = new Map();
  private pgResultsCache: Map<string, PaginatedResult<Member[]>> = new Map();

  constructor(private http: HttpClient) { }

  getMembers(userParams: UserParams) {
    let cachedKey = Object.values(userParams).join('-');
    let cachedValue = this.pgResultsCache.get(cachedKey);
    if(cachedValue) {
      return of(cachedValue);
    }

    let params = this.getPaginationHttpParams(userParams)
                     .append('minAge', userParams.minAge)
                     .append('maxAge', userParams.maxAge)
                     .append('gender', userParams.gender)
                     .append('orderBy', userParams.orderBy);

    return this.getPaginatedResult<Member[]>(this.baseUrl + 'users', params)
                .pipe(map(result => {
                  this.populateCache(cachedKey, result);
                  return result;
                }));
  }

  getMember(username: string) {
    if(this.membersCache.get(username)) {
      return of(this.membersCache.get(username));
    }

    return this.http.get<Member>(this.baseUrl + 'users/' + username);
  }

  updateMember(member: Member) {
    return this.http.put<void>(this.baseUrl + 'users/', member).pipe(
      map(() => {
        //todo with this implementation that is not necessary
      })
    );
  }

  setMainPhoto(photoId: number) {
    return this.http.put(this.baseUrl + 'users/set-main-photo/' + photoId, {});
  }

  deletePhoto(photoId : number) {
    return this.http.delete(this.baseUrl + 'users/delete-photo/' + photoId);
  }

  private getPaginationHttpParams(userParams: UserParams) {
    let params = new HttpParams();

    params = params.append('pageSize', userParams.pageSize)
                   .append('pageNumber', userParams.currentPage);

    return params;
  }

  private getPaginatedResult<T>(url: string, params: HttpParams) {
    return this.http.get<T>(url, { observe: 'response', params }).pipe(
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

  private populateMembersCache(result: PaginatedResult<Member[]>) {
    result.result.forEach(member => {
      this.membersCache.set(member.username, member);
    });
  }

  private populateCache(cachedKey: string, result: PaginatedResult<Member[]>) {
    this.pgResultsCache.set(cachedKey, result);
    this.populateMembersCache(result);
  }

}
