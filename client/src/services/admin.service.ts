import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { UserRoles } from 'src/models/userRoles';

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  getUsersWithRoles() {
    return this.http.get<UserRoles[]>(this.baseUrl + 'admin/users-with-roles');
  }

  updateRoles(username: string, roles: string[]) {
    let params = new HttpParams().append('roles', roles.join(','));
    return this.http.post<string[]>(this.baseUrl + 'admin/edit-roles/' + username, {}, { params });
  }
}
