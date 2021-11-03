import {
    Component,
    ContentChild,
    EventEmitter,
    Input,
    Output,
    TemplateRef,
    ViewChild,
} from '@angular/core';
// import { LazyLoadEvent } from 'primeng/api';
import { Table } from 'primeng/table';
import template from 'string-template';

@Component({
    selector: 'app-grid',
    templateUrl: 'grid.component.html',
})
export class GridComponent {
    // @ViewChild('dt')
    // public dt: any;

    @Input()
    public pageSize: number = 10;

    @Input()
    public totalRecords: number = 0;

    @Input()
    public loading: boolean = false;

    @Input()
    public items: any[] = [];

    @Input()
    public columns: string[] = [];

    @Input()
    public grantType: string = 'resource';

    @Input()
    public detailsLinkFormat: string = '{id}';

    @Input()
    public defaultActions: boolean = true;

    @Input()
    public paginator: boolean = true;

    @Input()
    public showActionsHeader: boolean = true;

    @Input()
    public styleClass: string = 'ui-table-list-default';

    /*
    @Input()
    public searchCaption: string = 'Search...';
    */

    // eslint-disable-next-line @angular-eslint/no-output-on-prefix
    @Output()
    public lazyLoad = new EventEmitter<any>();

    @ContentChild('headerTemplate') headerTemplateRef!: TemplateRef<any>;

    @ContentChild('bodyColumnsTemplate')
    bodyColumnsTemplateRef!: TemplateRef<any>;

    @ContentChild('customActionsTemplate')
    customActionsTemplateRef!: TemplateRef<any>;

    @ContentChild('headerFilterTemplate')
    headerFilterTemplateRef!: TemplateRef<any>;

    @ViewChild(Table) pTable!: Table;

    public get parsedColumns(): any[] {
        return this.columns.map((d: any) => {
            let v: any = {};
            if (typeof d === 'string') {
                v.field = d;
            } else {
                v = { ...d };
            }

            if (!v.header) {
                v.header = v.field;
            }

            if (!v.title) {
                v.title = v.header;
            }

            return v;
        });
    }

    public search(value: string) {
        this.pTable.filter(value, 'text', 'contains');
    }

    public getDetailsLink(item: any) {
        const link = template(this.detailsLinkFormat, item);
        return [link];
    }
}
