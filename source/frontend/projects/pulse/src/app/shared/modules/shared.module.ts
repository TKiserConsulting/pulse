import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { RouterModule } from '@angular/router';
import { ToastModule } from 'primeng/toast';
import { MessageModule } from 'primeng/message';

@NgModule({
    imports: [
        CommonModule,
        RouterModule,
        TranslateModule,
        ToastModule,
        MessageModule,
    ],
    exports: [
        CommonModule,
        RouterModule,
        TranslateModule,
        ToastModule,
        MessageModule,
    ],
})
export class SharedModule {}
