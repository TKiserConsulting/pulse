import { Directive, EventEmitter, HostListener, Output } from '@angular/core';
import {
    ControlContainer,
    FormGroupDirective,
    FormGroup,
} from '@angular/forms';

@Directive({
    selector: '[appSubmitIfValid]',
})
export class SubmitIfValidDirective {
    @Output('appSubmitIfValid') valid = new EventEmitter<void>(); // tslint:disable-line:no-output-rename

    constructor(private controlContainer: ControlContainer) {}

    private get formRef(): FormGroup {
        return (this.controlContainer as FormGroupDirective).form;
    }

    @HostListener('click')
    handleClick() {
        this.markFieldsAsDirty();
        this.emitIfValid();
    }

    private markFieldsAsDirty() {
        Object.keys(this.formRef.controls).forEach((fieldName) =>
            this.formRef.controls[fieldName].markAsDirty()
        );
    }

    private emitIfValid() {
        if (this.formRef.valid) {
            this.valid.emit();
        }
    }
}
