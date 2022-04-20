import { Injectable } from '@angular/core';
import { NgxSpinnerService } from 'ngx-spinner';

@Injectable({
  providedIn: 'root'
})
export class BusyService {
  requestsInProgressCount = 0;

  constructor(private spinnerService: NgxSpinnerService) { }

  busy() {
    this.requestsInProgressCount++;
    this.spinnerService.show();
  }

  idle() {
    this.requestsInProgressCount--;
    if(this.requestsInProgressCount <= 0) {
      this.requestsInProgressCount = 0;
      this.spinnerService.hide();
    }
  }

}
