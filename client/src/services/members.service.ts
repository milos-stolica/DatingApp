import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { of } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { getPaginatedResult, getPaginationHttpParams } from 'src/helper/paginationHelper';
import { LikeMember } from 'src/models/likeMember';
import { LikeParams } from 'src/models/likeParams';
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

    let params = getPaginationHttpParams(userParams.pageSize, userParams.currentPage)
                     .append('minAge', userParams.minAge)
                     .append('maxAge', userParams.maxAge)
                     .append('gender', userParams.gender)
                     .append('orderBy', userParams.orderBy);

    return getPaginatedResult<Member[]>(this.baseUrl + 'users', params, this.http)
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

  addLike(username: string) {
    return this.http.post(this.baseUrl + `likes/${username}`, {});
  }

  getLikes(likeParams: LikeParams) {
    let params = getPaginationHttpParams(likeParams.pageSize, likeParams.currentPage)
                     .append('predicate', likeParams.predicate);

    return getPaginatedResult<LikeMember[]>(this.baseUrl + 'likes', params, this.http);
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
