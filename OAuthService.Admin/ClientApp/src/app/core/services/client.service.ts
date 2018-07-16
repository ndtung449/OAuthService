import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { map } from 'rxjs/operators';
import { ReplaySubject } from 'rxjs/ReplaySubject';
import { PageResult } from '../../shared/pageResult.model';
import { ClientModel } from '../models/index';

const httpOptions = {
    headers: new HttpHeaders({
        'Content-Type': 'application/json',
    })
};

@Injectable()
export class ClientService {

    private clientsSubject = new ReplaySubject<Array<ClientModel>>(1);
    private clients$ = this.clientsSubject.asObservable();
    private clientUrl = 'api/clients';

    constructor(private httpClient: HttpClient) {

    }

    get(): Observable<Array<ClientModel>> {
        return this.httpClient.get<PageResult<ClientModel>>(this.clientUrl).pipe(
            map(result => result.items)
        );
    }

    getById(id: string): Observable<ClientModel> {
        return this.get().pipe(
            map(clients => {
                return clients.find(c => c.clientId === id);
            }));
    }

    create(client: ClientModel): Observable<ClientModel> {
        return this.httpClient.post<ClientModel>(`${this.clientUrl}/ResourceOwner`, client, httpOptions);
    }

    update(client: ClientModel): Observable<ClientModel> {
        return this.httpClient.put<ClientModel>(this.clientUrl, client, httpOptions);
    }

    delete(id: string): Observable<ClientModel> {
        const url = `${this.clientUrl}/${id}`;

        return this.httpClient.delete<ClientModel>(url, httpOptions);
    }
}
