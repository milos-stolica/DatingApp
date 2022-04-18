import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NgxGalleryAnimation, NgxGalleryImage, NgxGalleryOptions } from '@kolkov/ngx-gallery';
import { take } from 'rxjs/operators';
import { Member } from 'src/models/member';
import { MembersService } from 'src/services/members.service';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit {
  member: Member;
  galleryOptions: NgxGalleryOptions[] = [];
  galleryImages: NgxGalleryImage[] = [];

  constructor(private membersService: MembersService,
              private route: ActivatedRoute) { }

  ngOnInit(): void {
    this.loadMember();
    this.setPhotoGalleryOptions();
  }

  loadMember() {
    const username = this.route.snapshot.paramMap.get('username');

    this.membersService.getMember(username)
      .pipe(take(1))
      .subscribe(member => {
        this.member = member;
        this.setPhotoGalleryImages();
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
}
