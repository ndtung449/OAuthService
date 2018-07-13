import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { map } from 'rxjs/operators';
import { ReplaySubject } from 'rxjs/ReplaySubject';
import { PageResult } from '../../shared/pageResult.model';
import { Client } from '../models/index';

const httpOptions = {
    headers: new HttpHeaders({
        'Content-Type': 'application/json',
        'Authorization': 'my-auth-token'
    })
};

@Injectable()
export class ClientService {

    private clientsSubject = new ReplaySubject<Array<Client>>(1);
    private clients$ = this.clientsSubject.asObservable();
    private clientUrl = 'api/clients';

    constructor(private httpClient: HttpClient) {

    }

    get(): Observable<Array<Client>> {
        return this.httpClient.get<PageResult<Client>>(this.clientUrl).pipe(
            map(result => result.items)
        );
    }

    getById(id: string): Observable<Client> {
        return this.get().pipe(
            map(clients => {
                return clients.find(c => c.clientId === id);
            }));
    }

    create(client: Client): Observable<Client> {
        return this.httpClient.post<Client>(`${this.clientUrl}/ResourceOwner`, client, httpOptions);
    }

    update(client: Client): Observable<Client> {
        return this.httpClient.put<Client>(this.clientUrl, client, httpOptions);
    }

    delete(id: string): Observable<Client> {
        const url = `${this.clientUrl}/${id}`;

        return this.httpClient.delete<Client>(url, httpOptions);
    }
}
