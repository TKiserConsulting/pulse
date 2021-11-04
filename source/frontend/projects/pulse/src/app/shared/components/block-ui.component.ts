import { Component, Input } from '@angular/core';

@Component({
    selector: 'app-block-ui',
    template: ` <p-blockUI [blocked]="blocked" [target]="target"></p-blockUI> `,
})
export class BlockUiComponent {
    @Input()
    public blocked: boolean = false;

    @Input()
    public target: any;
}
