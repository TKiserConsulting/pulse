import { Component, Input } from '@angular/core';

@Component({
    selector: 'app-create-button',
    template: `
        <button
            pButton
            class="p-button-primary"
            [label]="label"
            [routerLink]="['add']"
            *ngIf="grantType + ':create' | appGranted | async"
        ></button>
    `,
})
export class CreateButtonComponent {
    @Input()
    public grantType: string = 'resource';

    @Input()
    public label: string = 'Create';
}
