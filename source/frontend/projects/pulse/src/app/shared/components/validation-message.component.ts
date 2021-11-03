import {
    Component,
    Input,
    Host,
    SkipSelf,
    AfterViewInit,
    OnDestroy,
    OnInit,
    ViewEncapsulation,
    HostBinding,
} from '@angular/core';
import {
    AbstractControl,
    FormGroup,
    ControlContainer,
    FormGroupDirective,
} from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';
// import isUndefined from 'lodash/isUndefined';
import { DeviceDetectorService } from 'ngx-device-detector';
import { forkJoin, Subscription, Observable, of } from 'rxjs';
import { map } from 'rxjs/operators';

import template from 'string-template';

@Component({
    selector: 'app-validation-message',
    template: `
        <div
            #errorCard
            class="error-card"
            *ngIf="
                control?.invalid &&
                (control?.dirty || control?.touched) &&
                errors &&
                errors.length
            "
        >
            <p-message
                *ngFor="let error of errors; trackBy: trackByE"
                severity="error"
                [text]="error"
            >
            </p-message>
        </div>
    `,
    encapsulation: ViewEncapsulation.None,
})
export class ValidationMessageComponent
    implements OnInit, AfterViewInit, OnDestroy
{
    @Input()
    public namespace = 'app.defaults.validations';

    @Input()
    public refControlName!: string;

    /*
    @ViewChild('errorCard')
    private errorCardRef: ElementRef;
    */

    @HostBinding('style.display')
    protected display = 'block';

    @HostBinding('class.position-static')
    @Input()
    private isPositionStatic = false;

    public control: AbstractControl | null = null;

    public errors: string[] = [];

    /*
    private componentPreviousOffsetTop: number;

    private scrollableParent: any;
    */

    private subscription!: Subscription;

    constructor(
        @Host()
        @SkipSelf()
        private controlContainer: ControlContainer,
        private translate: TranslateService,
        // private componentElRef: ElementRef,
        // private renderer: Renderer2,
        deviceService: DeviceDetectorService
    ) {
        if (!this.isPositionStatic) {
            this.isPositionStatic =
                deviceService.isTablet() || deviceService.isMobile();
        }
    }

    private get form(): FormGroup {
        let frm: FormGroup;
        if (this.controlContainer instanceof FormGroupDirective) {
            frm = (this.controlContainer as FormGroupDirective).form;
        } else {
            throw new Error('Cannot find related FormGroupDirective');
        }

        return frm;
    }

    /*
    private get controlNativeElement(): any {
        return this.componentElRef?.nativeElement?.previousSibling;
    }
    */

    public trackByE(index: number) {
        return index;
    }

    ngOnInit(): void {
        this.initControl();
    }

    ngAfterViewInit(): void {
        if (this.control) {
            this.subscription = this.control.statusChanges.subscribe(() =>
                this.checkError()
            );
            this.checkError();
        }

        /*
        if (!this.isPositionStatic && !!this.controlNativeElement) {
            this.scrollableParent = this.findScrollableParent(this.controlNativeElement.parentNode);

            fromEvent(this.controlNativeElement, 'mouseenter').subscribe(() =>
                this.setErrorCardPositionCoordinate()
            );
        }
        */
    }

    /*
    private setErrorCardPositionCoordinate(): void {
        const controlRect = this.controlNativeElement?.getBoundingClientRect();

        if (
            this.errorCardRef &&
            (isUndefined(this.componentPreviousOffsetTop) ||
                controlRect.top !== this.componentPreviousOffsetTop)
        ) {
            const errorCardClientRect = this.errorCardRef.nativeElement.getBoundingClientRect();
            const scrollableParentRect = this.scrollableParent?.getBoundingClientRect();
            const scrollableParentTop =
                scrollableParentRect && scrollableParentRect.top > 0 ? scrollableParentRect.top : 0;
            const errorCardBottom =
                controlRect.bottom - scrollableParentTop + errorCardClientRect.height;
            // eslint-disable-next-line no-undef
            const scrollableContainerHeight = scrollableParentRect?.height || window.innerHeight;

            let newErrorCardPositionTop;
            if (errorCardBottom >= scrollableContainerHeight) {
                newErrorCardPositionTop = controlRect.top - errorCardClientRect.height - 4;
                this.renderer.addClass(this.errorCardRef.nativeElement, 'mt-0');
            } else {
                newErrorCardPositionTop = controlRect.bottom;
                this.renderer.removeClass(this.errorCardRef.nativeElement, 'mt-0');
            }

            this.renderer.setStyle(
                this.errorCardRef.nativeElement,
                'top',
                `${newErrorCardPositionTop}px`
            );
            this.componentPreviousOffsetTop = controlRect.bottom;
        }
    }
    */

    /*
    private findScrollableParent(node: any): any {
        if (node == null) {
            return null;
        }

        if (
            node.scrollHeight > node.clientHeight &&
            ['scroll', 'auto'].includes(getComputedStyle(node).getPropertyValue('overflow-y'))
        ) {
            return node;
        }
        return this.findScrollableParent(node.parentNode);
    }
    */

    private initControl(): void {
        this.control = this.form.get(this.refControlName);
        if (!this.control) {
            throw new Error(`Cannot find control ${this.refControlName}`);
        }
    }

    private checkError() {
        const list: Observable<string>[] = Object.keys(
            this.control?.errors || {}
        ).map((k) => {
            let observable: Observable<string>;
            const d = this.control?.errors ? this.control.errors[k] : null;
            if (typeof d === 'object' && d !== null && d.message) {
                observable = of(d.message as string);
            } else {
                const key = `${this.namespace}.${k}`;
                observable = this.translate.get(key).pipe(
                    map((v) => {
                        const x = v as string;
                        const y = template(x, d);
                        return y;
                    })
                );
            }

            return observable;
        });

        forkJoin(list).subscribe((messages) => {
            this.errors = messages || [];
        });
    }

    ngOnDestroy(): void {
        this.subscription?.unsubscribe();
    }
}
