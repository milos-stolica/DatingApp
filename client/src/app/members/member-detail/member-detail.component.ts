import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NgxGalleryAnimation, NgxGalleryImage, NgxGalleryOptions } from '@kolkov/ngx-gallery';
import { TabDirective, TabsetComponent } from 'ngx-bootstrap/tabs';
import { take } from 'rxjs/operators';
import { Member } from 'src/models/member';
import { Message } from 'src/models/message';
import { MembersService } from 'src/services/members.service';
import { MessagesService } from 'src/services/messages.service';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit {
  @ViewChild('memberTabs', {static: true}) memberTabs: TabsetComponent;
  member: Member;
  messages: Message[] = [];
  galleryOptions: NgxGalleryOptions[] = [];
  galleryImages: NgxGalleryImage[] = [];
  activeTab: TabDirective;

  constructor(private membersService: MembersService,
              private route: ActivatedRoute,
              private messagesService: MessagesService) { }

  ngOnInit(): void {
    this.route.data.subscribe(data => this.member = data?.member);
    this.setPhotoGalleryOptions();
    this.setPhotoGalleryImages();

    this.route.queryParams.subscribe(params => {
      params.tab ? this.selectTab(params.tab) : this.selectTab(0);
    });
  }

  setPhotoGalleryOptions() {
    this.galleryOptions = [
      {
        width: '500px',
        height: '500px',
        thumbnailsColumns: 4,
        imageAnimation: NgxGalleryAnimation.Slide
      },
      // max-width 800
      {
        breakpoint: 800,
        width: '100%',
        height: '600px',
        imagePercent: 80,
        thumbnailsPercent: 20,
        thumbnailsMargin: 20,
        thumbnailMargin: 20
      },
      // max-width 400
      {
        breakpoint: 400,
        preview: false
      }
    ];
  }

  setPhotoGalleryImages() {
    for (const photo of this.member.photos) {
      this.galleryImages.push({
        small: photo.url,
        medium: photo.url,
        big: photo.url
      });
    }
  }

  onTabActivated(data: TabDirective) {
    if(data.heading === 'Messages' && this.messages.length === 0) {
      this.loadMessages();
    }
  }

  selectTab(tabId: number) {
    this.memberTabs.tabs[tabId].active = true;
  }

  loadMessages() {
    this.messagesService.getMessageThread(this.member.username)
      .subscribe(messages => this.messages = messages)
  }
}
