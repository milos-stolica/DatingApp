import { Component, Input, OnInit } from '@angular/core';
import { Member } from 'src/models/member';
import { FileUploader, FileItem } from 'ng2-file-upload';
import { environment } from 'src/environments/environment';
import { AccountService } from 'src/services/account.service';
import { take } from 'rxjs/operators';
import { Photo } from 'src/models/photo';
import { MembersService } from 'src/services/members.service';
import { User } from 'src/models/user';

@Component({
  selector: 'app-photo-editor',
  templateUrl: './photo-editor.component.html',
  styleUrls: ['./photo-editor.component.css']
})
export class PhotoEditorComponent implements OnInit {
  @Input('member') member: Member;
  user: User;

  uploader: FileUploader;
  hasBaseDropZoneOver = false;
  hasAnotherDropZoneOver = false;
  response:string;
  baseUrl = environment.apiUrl;

  constructor (private accountService: AccountService,
               private memberService: MembersService) {

    accountService.currentUser$
      .pipe(take(1))
      .subscribe(user => this.user = user);

    this.initializePhotoUploader();

    this.response = '';

    this.uploader.response.subscribe( res => this.response = res );
  }

  ngOnInit(): void {
  }

  private initializePhotoUploader() {
    this.uploader = new FileUploader({
      url: this.baseUrl + 'users/add-photo',
      authToken: `Bearer ${this.user.token}`,
      isHTML5: true,
      allowedFileType: ['image'],
      autoUpload: false,
      removeAfterUpload: true,
      maxFileSize: 10 * 1024 * 1024,
    });

    this.uploader.onAfterAddingFile = (file: FileItem) => {
      file.withCredentials = false;
    };

    this.uploader.onSuccessItem = (item, response, status, headers) => {
      if(response) {
        this.member.photos.push(JSON.parse(response));
      }
    };
  }

  public fileOverBase(e:any):void {
    this.hasBaseDropZoneOver = e;
  }

  public setMain(photo : Photo) {
    this.memberService.setMainPhoto(photo.id)
      .pipe(take(1))
      .subscribe(() => {
        this.user.photoUrl = photo.url;
        this.accountService.setCurrentUser(this.user);
        const previousMain = this.member.photos.find(photo => photo.isMain);
        previousMain.isMain = false;
        photo.isMain = true;
        this.member.photoUrl = photo.url;
      });
  }

  public deletePhoto(photo: Photo) {
    this.memberService.deletePhoto(photo.id)
      .pipe(take(1))
      .subscribe(() => {
        this.member.photos = this.member.photos.filter(p => p.id !== photo.id);
      })
  }

}
