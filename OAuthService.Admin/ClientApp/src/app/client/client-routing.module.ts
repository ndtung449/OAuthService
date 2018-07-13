import { Routes, RouterModule } from '@angular/router';
import { ClientsComponent } from './client-list/clients.component';
import { ClientDetailComponent } from './client-detail/client-detail.component';
import { NgModule } from '@angular/core';

const routes: Routes = [
    { path: '', component: ClientsComponent },
    { path: 'detail/:id', component: ClientDetailComponent },
    { path: 'detail', component: ClientDetailComponent },
];

@NgModule({
    imports: [
        RouterModule.forChild(routes),
    ],
    exports: [RouterModule]
})
export class ClientRoutingModule { }
