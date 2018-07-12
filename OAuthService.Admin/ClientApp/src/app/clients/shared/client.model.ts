export interface Client {
    clientId: string;
    clientName: string;
    grantTypes: Array<string>;
    redirectUris: Array<string>;
    postLogoutRedirectUris: Array<string>;
    scope: Array<string>;
}
