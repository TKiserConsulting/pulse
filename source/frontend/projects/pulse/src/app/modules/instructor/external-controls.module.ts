import { NgModule } from '@angular/core';
import { BadgeModule } from 'primeng/badge';
import { ColorPickerModule } from 'primeng/colorpicker';
import { FileUploadModule } from 'primeng/fileupload';
import { ListboxModule } from 'primeng/listbox';
import { ChartModule } from 'primeng/chart';

@NgModule({
    imports: [
        BadgeModule,
        FileUploadModule,
        ListboxModule,
        ColorPickerModule,
        ChartModule,
    ],
    exports: [
        BadgeModule,
        FileUploadModule,
        ListboxModule,
        ColorPickerModule,
        ChartModule,
    ],
})
export class ExternalControlsModule {}
