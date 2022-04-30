import { Component, OnInit } from '@angular/core';
import { LikeMember } from 'src/models/likeMember';
import { LikeParams } from 'src/models/likeParams';
import { MembersService } from 'src/services/members.service';

@Component({
  selector: 'app-lists',
  templateUrl: './lists.component.html',
  styleUrls: ['./lists.component.css']
})
export class ListsComponent implements OnInit {
  likes: LikeMember[] = [];
  likeParams: LikeParams;
  totalCount: number;
  maxSize: number = 10;


  constructor(private membersService: MembersService) { 
    this.likeParams = new LikeParams();
  }

  ngOnInit(): void {
    this.loadLikes();
  }

  loadLikes() {
    this.membersService.getLikes(this.likeParams)
      .subscribe(response => {
        this.likes = response.result;
        this.totalCount = response.pagination.totalCount;
      })
  }

  pageChanged(event: any) {
    this.likeParams.currentPage = event.page;
    this.loadLikes();
  }

}
