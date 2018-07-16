export class ClientModel {
    clientId: string;
    clientName: string;
    clientUri: string;
    grantTypes: Array<string>;
    redirectUris: Array<string>;
    postLogoutRedirectUris: Array<string>;
    scopes: Array<string>;

    constructor() {
        this.grantTypes = new Array<string>();
        this.redirectUris = new Array<string>();
        this.postLogoutRedirectUris = new Array<string>();
        this.scopes = new Array<string>();
    }
}
