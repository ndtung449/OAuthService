import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ClientService } from '../shared/client.service';
import { Location } from '@angular/common';
import { Client } from '../shared/client.model';

@Component({
    selector: 'app-client-details',
    templateUrl: './client-detail.component.html'
})
export class ClientDetailComponent implements OnInit {

    public client: Client;
    public newGrantType: string;

    constructor(
        private route: ActivatedRoute,
        private heroService: ClientService,
        private location: Location
    ) { }

    ngOnInit(): void {
        this.getClient();
    }

    getClient(): void {
        const id = this.route.snapshot.paramMap.get('id');
        this.heroService.getById(id)
            .subscribe(client => this.client = client);
    }

    addGrantType(): void {
        this.client.grantTypes.push(this.newGrantType);
        this.newGrantType = '';
    }

    removeGrantType(grantType: string): void {
        const index = this.client.grantTypes.findIndex(t => t === grantType);
        if (index >= 0) {
            this.client.grantTypes.splice(index, 1);
        }
    }

    goBack(): void {
        this.location.back();
    }
}
