import { Component, Input } from '@angular/core';

@Component({
    selector: 'app-back-button',
    template: `
        <button
            type="button"
            pButton
            class="p-button-secondary"
            label="Back"
            [routerLink]="[address]"
        ></button>
    `,
})
export class BackButtonComponent {
    @Input()
    public address: string | null = null;
}
