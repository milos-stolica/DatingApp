import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AdminGuard } from 'src/guards/admin.guard';
import { AuthGuard } from 'src/guards/auth.guard';
import { PreventLosingChangesGuard } from 'src/guards/prevent-losing-changes.guard';
import { MemberDetailedResolver } from 'src/resolvers/member-detailed.resolver';
import { AdminPanelComponent } from './admin/admin-panel/admin-panel.component';
import { NotFoundComponent } from './errors/not-found/not-found.component';
import { ServerErrorComponent } from './errors/server-error/server-error.component';
import { HomeComponent } from './home/home.component';
import { ListsComponent } from './lists/lists.component';
import { MemberEditComponent } from './member-edit/member-edit.component';
import { MemberDetailComponent } from './members/member-detail/member-detail.component';
import { MemberListComponent } from './members/member-list/member-list.component';
import { MessagesComponent } from './messages/messages.component';
import { TestErrorsComponent } from './test-errors/test-errors.component';

const routes : Routes = [
  {path:'', component: HomeComponent},
  {
    path: '',
    runGuardsAndResolvers:'always',
    canActivate: [AuthGuard],
    children: [
      {path:'members', component: MemberListComponent},
      {path:'members/:username', component: MemberDetailComponent, resolve: {member: MemberDetailedResolver}},
      {path:'member/edit', component: MemberEditComponent, canDeactivate: [PreventLosingChangesGuard]},
      {path:'messages', component: MessagesComponent},
      {path:'lists', component: ListsComponent},
      {path:'admin', component: AdminPanelComponent, canActivate: [AdminGuard]}
    ]
  },
  {path:'errors', component: TestErrorsComponent},
  {path:'not-found', component: NotFoundComponent},
  {path:'server-error', component: ServerErrorComponent},
  {path:'**', component: NotFoundComponent, pathMatch:'full'}
];

@NgModule({
  declarations: [],
  imports: [
    RouterModule.forRoot(routes)
  ],
  exports: [RouterModule]
})
export class AppRoutingModule { }
