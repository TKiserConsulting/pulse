import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
    selector: 'app-delete-button',
    template: `
        <button
            type="button"
            pButton
            class="p-button-danger"
            label="Delete"
            (click)="onClick.emit($event)"
            *ngIf="grantType + ':delete' | appGranted | async"
        ></button>
    `,
})
export class DeleteButtonComponent {
    @Input()
    grantType: string = 'resource';

    // eslint-disable-next-line @angular-eslint/no-output-on-prefix
    @Output()
    onClick: EventEmitter<any> = new EventEmitter();
}
