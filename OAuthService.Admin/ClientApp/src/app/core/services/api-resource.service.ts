import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { map } from 'rxjs/operators';
import { PageResult } from '../../shared/pageResult.model';
import { ApiResourceModel } from '../models/index';

const httpOptions = {
    headers: new HttpHeaders({
        'Content-Type': 'application/json',
    })
};

@Injectable()
export class ApiResourceService {

    private apiResourceUrl = 'api/apiResources';

    constructor(private httpClient: HttpClient) {

    }

    get(): Observable<Array<ApiResourceModel>> {
        return this.httpClient.get<PageResult<ApiResourceModel>>(this.apiResourceUrl).pipe(
            map(result => result.items)
        );
    }

    getByName(name: string): Observable<ApiResourceModel> {
        return this.get().pipe(
            map(apiResources => {
                return apiResources.find(r => r.name === name);
            }));
    }

    create(apiResource: ApiResourceModel): Observable<ApiResourceModel> {
        return this.httpClient.post<ApiResourceModel>(`${this.apiResourceUrl}`, apiResource, httpOptions);
    }

    update(apiResource: ApiResourceModel): Observable<ApiResourceModel> {
        return this.httpClient.put<ApiResourceModel>(this.apiResourceUrl, apiResource, httpOptions);
    }

    delete(name: string): Observable<ApiResourceModel> {
        const url = `${this.apiResourceUrl}/${name}`;

        return this.httpClient.delete<ApiResourceModel>(url, httpOptions);
    }
}
