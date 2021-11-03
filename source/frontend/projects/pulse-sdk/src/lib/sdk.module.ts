import { NgModule } from '@angular/core';
import { AutoFocusDirective, SubmitIfValidDirective } from './directives';

@NgModule({
    declarations: [AutoFocusDirective, SubmitIfValidDirective],
    imports: [],
    exports: [AutoFocusDirective, SubmitIfValidDirective],
})
export class SdkModule {}
