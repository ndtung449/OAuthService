import { FormControl, Validators, FormGroup } from '@angular/forms';

export class ClientFormControl extends FormControl {
    label: string;
    modelProperty: string;

    constructor(label: string, property: string, value: any, validator: any) {
        super(value, validator);
        this.label = label;
        this.modelProperty = property;
    }

    getValidationMessages() {
        const messages: string[] = [];
        if (this.errors) {
            for (const errorName in this.errors) {
                if (this.errors.hasOwnProperty(errorName)) {
                    switch (errorName) {
                        case 'required':
                            messages.push(`You must enter a ${this.label}`);
                            break;
                        case 'minlength':
                            messages.push(`A ${this.label} must be at least ${this.errors['minlength'].requiredLength} characters`);
                            break;
                        case 'maxlength':
                            messages.push(`A ${this.label} must be no more than ${this.errors['maxlength'].requiredLength} characters`);
                            break;
                        case 'pattern':
                            messages.push(`The ${this.label} contains illegal characters`);
                            break;
                    }
                }
            }
        }
        return messages;
    }
}

export class ClientFormGroup extends FormGroup {

    constructor() {
        super({
            clientId: new ClientFormControl('Client Id', 'clientId', '', Validators.compose([])),
            clientName: new ClientFormControl('Client name', 'clientName', '', Validators.compose([
                Validators.required,
                Validators.pattern('^[A-Za-z0-9 ]+$'),
                Validators.minLength(3),
                Validators.maxLength(200),
            ])),
            clientUri: new ClientFormControl('Client Uri', 'clientUri', '', Validators.compose([])),
            grantType: new ClientFormControl('Grant type', 'grantType', '', Validators.compose([
                Validators.pattern('^[A-Za-z_ ]+$'),
            ])),
            redirectUri: new ClientFormControl('Redirect Uri', 'redirectUri', '', Validators.compose([

            ])),
            postLogoutRedirectUri: new ClientFormControl('Post logout redirect Uri', 'postLogoutRedirectUri', '', Validators.compose([

            ])),
            scope: new ClientFormControl('Scope', 'scope', '', Validators.compose([
                Validators.pattern('^[A-Za-z0-9]+$'),
            ])),
        });
    }

    get clientControls(): ClientFormControl[] {
        return Object.keys(this.controls)
            .map(k => this.controls[k] as ClientFormControl);
    }

    getFormValidationMessages(): string[] {
        const messages: string[] = [];
        this.clientControls.forEach(c => c.getValidationMessages()
            .forEach(m => messages.push(m)));
        return messages;
    }
}
