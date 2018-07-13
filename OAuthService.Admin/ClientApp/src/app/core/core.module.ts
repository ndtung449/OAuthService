import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ClientService } from './services';

@NgModule({
    imports: [
        CommonModule,
    ],
    providers: [
        ClientService,
    ],
    declarations: []
})
export class CoreModule {}
