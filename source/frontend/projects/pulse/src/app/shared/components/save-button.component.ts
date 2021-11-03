import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
    selector: 'app-save-button',
    template: `
        <button
            type="submit"
            pButton
            class="p-button-primary"
            label="Save"
            (click)="onClick.emit($event)"
            [disabled]="disabled"
            *ngIf="grantType + ':' + (editMode ? 'update' : 'create') | appGranted | async"
        ></button>
    `,
})
export class SaveButtonComponent {
    @Input()
    grantType: string = 'resource';

    @Input()
    disabled: boolean = false;

    @Input()
    editMode: boolean = false;

    // eslint-disable-next-line @angular-eslint/no-output-on-prefix
    @Output()
    onClick: EventEmitter<any> = new EventEmitter();
}
