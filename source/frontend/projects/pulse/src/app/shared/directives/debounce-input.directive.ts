import {
    Directive,
    EventEmitter,
    HostListener,
    Input,
    OnDestroy,
    OnInit,
    Output,
} from '@angular/core';
import { Subject, Subscription } from 'rxjs';
import { debounceTime } from 'rxjs/operators';

@Directive({
    selector: '[appDebounceInput]',
})
export class DebounceInputDirective implements OnInit, OnDestroy {
    @Input() debounceTime = 500;

    @Output() debounceInput = new EventEmitter();

    private events = new Subject();

    private subscription!: Subscription;

    public ngOnInit() {
        this.subscription = this.events
            .pipe(debounceTime(this.debounceTime))
            .subscribe((e) => this.debounceInput.emit(e));
    }

    public ngOnDestroy() {
        this.subscription.unsubscribe();
    }

    @HostListener('input', ['$event'])
    public inputEvent(event: any) {
        event.preventDefault();
        event.stopPropagation();
        this.events.next(event);
    }
}
