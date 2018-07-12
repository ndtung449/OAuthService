import { Component, OnInit } from '@angular/core';
import { ClientService } from './shared/client.service';
import { Client } from './shared/client.model';
import { Observable } from 'rxjs/Observable';

@Component({
    selector: 'app-clients',
    templateUrl: './clients.component.html',
    styleUrls: ['./clients.component.css'],
})
export class ClientsComponent implements OnInit {

    public clients: Array<Client> = [];

    constructor(private clientService: ClientService) {

    }

    ngOnInit(): void {
        this.loadClients();
    }

    loadClients(): void {
        this.clientService.get().subscribe(clients => {
            this.clients = clients;
        });
    }
}
