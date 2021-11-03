import {
    Component,
    ContentChildren,
    HostBinding,
    Input,
    QueryList,
    ViewEncapsulation,
} from '@angular/core';

@Component({
    selector: 'app-pane',
    template: `<ng-content></ng-content>`,
    encapsulation: ViewEncapsulation.None,
})
export class ActionsToolbarPaneComponent {
    @Input()
    public position = 'left';

    @Input()
    @HostBinding('class')
    public styleClass = '';
}

@Component({
    selector: 'app-actions-toolbar',
    template: `
        <div class="p-grid p-nogutter app-actions-toolbar-wrap">
            <div
                *ngIf="leftPresent"
                class="app-actions-toolbar-left p-col-12"
                [ngClass]="{ 'p-md-6': rightPresent }"
            >
                <ng-content select="app-pane[position=left]"></ng-content>
            </div>
            <div
                *ngIf="rightPresent"
                class="app-actions-toolbar-right p-col-12"
                [ngClass]="{ 'p-md-6': leftPresent }"
            >
                <ng-content select="app-pane[position=right]"></ng-content>
            </div>
        </div>
    `,
    styleUrls: ['actions-toolbar.component.scss'],
    encapsulation: ViewEncapsulation.None,
})
export class ActionsToolbarComponent {
    @ContentChildren(ActionsToolbarPaneComponent)
    children!: QueryList<ActionsToolbarPaneComponent>;

    public get leftPresent(): boolean {
        return this.children.some((c) => c.position === 'left');
    }

    public get rightPresent(): boolean {
        return this.children.some((c) => c.position === 'right');
    }
}
