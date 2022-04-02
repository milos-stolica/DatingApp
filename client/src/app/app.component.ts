import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {

  title = 'The Dating App';
  users: any;

  constructor(private http : HttpClient){}

  ngOnInit(): void {
    this.getUsers();
  }

  private getUsers() : void {
    this.http.get('https://localhost:5001/api/users').subscribe(this.getUsersSuccess.bind(this), this.getUsersError.bind(this))
  }

  private getUsersSuccess(response: any) : void {
    this.users = response;
    for(let i = 0; i < this.users.length; i++)
    {
      console.log(this.users[i].userName);
    }
  }

  private getUsersError(error: any) : void {
    console.log(error);
  }
}
