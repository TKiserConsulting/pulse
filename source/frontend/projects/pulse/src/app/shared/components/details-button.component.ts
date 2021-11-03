import { Component, Input } from '@angular/core';

@Component({
    selector: 'app-details-button',
    template: `
        <div *ngIf="grantType + ':update' | appGranted | async; else elseBlock">
            <button
                pButton
                icon="pi pi-pencil"
                title="Edit"
                class="p-button-secondary p-button-outlined"
                [routerLink]="link"
                [queryParams]="{ ref: reference, zone: zone }"
            ></button>
        </div>
        <ng-template #elseBlock>
            <button
                pButton
                icon="pi pi-eye"
                title="View"
                class="p-button-secondary p-button-outlined"
                [routerLink]="link"
                [queryParams]="{ mode: 'view', ref: reference, zone: zone }"
            ></button>
        </ng-template>
    `,
})
export class DetailsButtonComponent {
    @Input()
    public grantType: string = 'resource';

    @Input()
    public link!: any[];

    @Input()
    public reference: any;

    @Input()
    public zone: any;
}
