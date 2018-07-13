import { Component, OnInit } from '@angular/core';
import { ClientService } from '../../core/services/client.service';
import { Client } from '../../core/models/client.model';
import { Router, ActivatedRoute } from '@angular/router';

@Component({
    selector: 'app-clients',
    templateUrl: './clients.component.html',
    styleUrls: ['./clients.component.css'],
})
export class ClientsComponent implements OnInit {

    public clients: Array<Client> = [];

    constructor(
        private clientService: ClientService,
        private router: Router,
        private route: ActivatedRoute,
    ) {
    }

    ngOnInit(): void {
        this.loadClients();
    }

    loadClients(): void {
        this.clientService.get().subscribe(clients => {
            this.clients = clients;
        });
    }

    createClient() {
        this.router.navigate(['detail'], { relativeTo: this.route });
    }
}
