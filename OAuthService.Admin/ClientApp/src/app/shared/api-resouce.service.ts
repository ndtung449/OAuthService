import {
    Injectable,
} from '@angular/core';
import { HttpClient } from '@angular/common/http';
@Injectable()
export class ApiResourceService {
    constructor(private httpClient: HttpClient) {

    }
}
