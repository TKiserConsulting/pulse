import { NgModule } from '@angular/core';
import { BlockUIModule } from 'primeng/blockui';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { MessageModule } from 'primeng/message';
import { DialogModule } from 'primeng/dialog';
import { BlockUiComponent } from '../components/block-ui.component';
import { ValidationMessageComponent } from '../components/validation-message.component';

@NgModule({
    imports: [
        ButtonModule,
        BlockUIModule,
        CardModule,
        MessageModule,
        DialogModule,
    ],
    declarations: [BlockUiComponent, ValidationMessageComponent],
    exports: [
        ButtonModule,
        BlockUIModule,
        CardModule,
        MessageModule,
        BlockUiComponent,
        ValidationMessageComponent,
        DialogModule,
    ],
})
export class EssentialControlsModule {}
