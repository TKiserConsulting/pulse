import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
    selector: 'app-search-input',
    template: `
        <div class="p-inputgroup">
            <input
                type="text"
                pInputText
                title="Search"
                [(ngModel)]="value"
                placeholder="{{ placeholder }}"
                appDebounceInput
                (debounceInput)="doSearch()"
                (keyup.enter)="doSearch()"
                class="p-inputtext p-component"
            />
            <button
                class="p-ripple p-button p-component p-button-icon-only"
                (click)="doSearch()"
                title="search"
            >
                <i class="pi pi-search"></i>
            </button>
        </div>
    `,
    styles: [
        `
            :host {
                width: 100%;
            }
        `,
    ],
})
export class SearchInputComponent {
    @Input()
    public placeholder = 'Search...';

    @Input()
    public value: string | null = null;

    @Output()
    public valueChange = new EventEmitter<string | null>();

    @Output()
    public search = new EventEmitter<{ value: string | null }>();

    public doSearch() {
        this.search.emit({ value: this.value });
        this.valueChange.emit(this.value);
    }
}
