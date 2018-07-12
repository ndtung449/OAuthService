import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ClientService } from '../shared/client.service';
import { Location } from '@angular/common';
import { Client } from '../shared/client.model';
import { ClientFormGroup } from '../shared/form.model';

const GRANT_TYPES = [
    'password',
    'implicit',
    'hybrid',
    'authorization_code',
    'client_credentials'
];

@Component({
    selector: 'app-client-details',
    templateUrl: './client-detail.component.html'
})
export class ClientDetailComponent implements OnInit {

    public grantTypes = GRANT_TYPES;
    public client: Client;
    public isCreated = true;
    public form: ClientFormGroup = new ClientFormGroup();
    public submitting = false;
    public newGrantType: string;
    public newRedirectUri: string;
    public newPostLogoutRedirectUri: string;
    public newScope: string;

    constructor(
        private route: ActivatedRoute,
        private clientService: ClientService,
        private location: Location
    ) { }

    ngOnInit(): void {
        const id = this.route.snapshot.paramMap.get('id');
        if (id) {
            this.isCreated = false;
            this.getClient(id);
        } else {
            this.client = new Client();
        }
    }

    private getClient(id: string): void {
        this.clientService.getById(id)
            .subscribe(client => this.client = client);
    }

    addGrantType(): void {
        this.client.grantTypes.push(this.newGrantType);
        this.form.controls['grantType'].reset();
    }

    removeGrantType(grantType: string): void {
        const index = this.client.grantTypes.findIndex(t => t === grantType);
        if (index >= 0) {
            this.client.grantTypes.splice(index, 1);
        }
    }

    addRedirectUri(): void {
        const index = this.client.redirectUris.findIndex(u => u === this.newRedirectUri);
        if (index === -1) {
            this.client.redirectUris.push(this.newRedirectUri);
            this.form.controls['redirectUri'].reset();
        }
    }

    removeRedirectUri(uri: string): void {
        const index = this.client.redirectUris.findIndex(u => u === uri);
        if (index >= 0) {
            this.client.redirectUris.splice(index, 1);
        }
    }

    addPostLogoutRedirectUri(): void {
        const index = this.client.postLogoutRedirectUris.findIndex(u => u === this.newPostLogoutRedirectUri);
        if (index === -1) {
            this.client.postLogoutRedirectUris.push(this.newPostLogoutRedirectUri);
            this.form.controls['postLogoutRedirectUri'].reset();
        }
    }

    removePostLogoutRedirectUri(uri: string): void {
        const index = this.client.postLogoutRedirectUris.findIndex(u => u === uri);
        if (index >= 0) {
            this.client.postLogoutRedirectUris.splice(index, 1);
        }
    }

    addScope(): void {
        const index = this.client.scopes.findIndex(s => s === this.newScope);
        if (index === -1) {
            this.client.scopes.push(this.newScope);
            this.form.controls['scope'].reset();
        }
    }

    removeScope(scope: string): void {
        const index = this.client.scopes.findIndex(s => s === scope);
        if (index > -1) {
            this.client.scopes.splice(index, 1);
        }
    }

    save(): void {
        this.submitting = true;
        if (this.isCreated) {
            if (this.client.grantTypes.length === 0) {
                this.client.grantTypes.push('password');
            }
            this.clientService.create(this.client).subscribe(result => {
                this.submitting = false;
                this.goBack();
            }, error => {
                console.log(error);
                this.submitting = false;
            });
        } else {
            this.clientService.update(this.client).subscribe(result => {
                this.submitting = false;
            }, error => {
                console.log(error);
                this.submitting = false;
            });
        }
    }

    goBack(): void {
        this.location.back();
    }
}
