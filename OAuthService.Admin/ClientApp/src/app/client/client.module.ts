import { NgModule } from '@angular/core';
import { ClientsComponent } from './client-list/clients.component';
import { ClientDetailComponent } from './client-detail/client-detail.component';
import { SharedModule } from '../shared/shared.module';
import { ClientRoutingModule } from './client-routing.module';

@NgModule({
  imports: [
    SharedModule,
    ClientRoutingModule,
  ],
  declarations: [
    ClientsComponent,
    ClientDetailComponent,
  ],
  providers: [
  ]
})
export class ClientModule { }
