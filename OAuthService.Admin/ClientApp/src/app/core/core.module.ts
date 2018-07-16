import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ClientService, ApiResourceService } from './services/index';

@NgModule({
    imports: [
        CommonModule,
    ],
    providers: [
        ClientService,
        ApiResourceService,
    ],
    declarations: []
})
export class CoreModule {}
