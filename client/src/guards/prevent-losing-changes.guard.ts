import { Injectable } from '@angular/core';
import { CanDeactivate } from '@angular/router';
import { MemberEditComponent } from 'src/app/member-edit/member-edit.component';

@Injectable({
  providedIn: 'root'
})
export class PreventLosingChangesGuard implements CanDeactivate<unknown> {
  canDeactivate(component: MemberEditComponent): boolean {
    if(component.editForm.dirty) {
      return confirm('Are you sure you want to leave page? Any unsaved changes will be lost.');
    }

    return true;
  }
}
