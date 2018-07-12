import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpErrorResponse } from '@angular/common/http';
import { Client } from './client.model';
import { catchError, map } from 'rxjs/operators';
import { of } from 'rxjs/observable/of';
import { Observable } from 'rxjs/Observable';
import { ReplaySubject } from 'rxjs/ReplaySubject';
import { PageResult } from '../../shared/pageResult.model';

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
        return this.httpClient.post<Client>(this.clientUrl, client, httpOptions)
            .pipe(
                catchError(this.handleError<Client>('create'))
            );
    }

    update(client: Client): Observable<Client> {
        return this.httpClient.put<Client>(this.clientUrl, client, httpOptions)
            .pipe(
                catchError(this.handleError<Client>('update'))
            );
    }

    delete(id: string): Observable<Client> {
        const url = `${this.clientUrl}/${id}`;

        return this.httpClient.delete<Client>(url, httpOptions).pipe(
            catchError(this.handleError<Client>('delete'))
        );
    }

    private handleError<T>(operation = 'operation', result?: T) {
        return (error: any): Observable<T> => {

            // TODO: send the error to remote logging infrastructure
            console.error(error); // log to console instead

            // Let the app keep running by returning an empty result.
            return of(result as T);
        };
    }
}
