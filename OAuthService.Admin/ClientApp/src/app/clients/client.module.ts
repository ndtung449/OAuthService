import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { ClientsComponent } from './client-list/clients.component';
import { ClientDetailComponent } from './client-detail/client-detail.component';

@NgModule({
  declarations: [
    ClientsComponent,
    ClientDetailComponent,
  ],
  imports: [
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule.forRoot([
      { path: '', component: ClientsComponent, pathMatch: 'full' },
      { path: 'client', component: ClientsComponent },
      { path: 'client/detail/:id', component: ClientDetailComponent },
      { path: 'client/detail', component: ClientDetailComponent },
    ])
  ],
  providers: [
  ]
})
export class ClientModule { }
