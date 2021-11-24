import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';

import { CommonModule, CurrencyPipe } from '@angular/common';
import { InputTextModule } from 'primeng/inputtext';
import { MessagesModule } from 'primeng/messages';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { RouterModule } from '@angular/router';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { DropdownModule } from 'primeng/dropdown';
import { BlockUIModule } from 'primeng/blockui';
import { OverlayPanelModule } from 'primeng/overlaypanel';
import { PanelModule } from 'primeng/panel';
import { FieldsetModule } from 'primeng/fieldset';
import { ContextMenuModule } from 'primeng/contextmenu';
import { ConfirmPopupModule } from 'primeng/confirmpopup';
import { ScrollPanelModule } from 'primeng/scrollpanel';
import { InputTrimModule } from 'ng2-trim-directive';
import { DebounceInputDirective } from '../directives/debounce-input.directive';
import { DateTimePipe } from '../pipes/date-time.pipe';
import { AuditorPipe } from '../pipes/auditor.pipe';
import { GrantedPipe } from '../pipes/granted.pipe';
import { SearchInputComponent } from '../components/search-input.component';
import { CreateButtonComponent } from '../components/create-button.component';
import {
    ActionsToolbarComponent,
    ActionsToolbarPaneComponent,
} from '../components/actions-toolbar.component';
import { DetailsButtonComponent } from '../components/details-button.component';
import { BackButtonComponent } from '../components/back-button.component';
import { DeleteButtonComponent } from '../components/delete-button.component';
import { SaveButtonComponent } from '../components/save-button.component';
import { GridComponent } from '../components/grid.component';
import { SafePipe } from '../pipes/safe.pipe';
import { EmptyComponent } from '../components/empty.component';
import { EssentialControlsModule } from './essential-controls.module';
import { InputNumberModule } from 'primeng/inputnumber';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        RouterModule,
        InputTextModule,
        MessagesModule,
        ConfirmDialogModule,
        InputTextareaModule,
        DropdownModule,
        BlockUIModule,
        OverlayPanelModule,
        PanelModule,
        InputTrimModule,
        ContextMenuModule,
        ConfirmPopupModule,
        ScrollPanelModule,
        InputNumberModule,
        EssentialControlsModule,
    ],
    declarations: [
        DebounceInputDirective,
        DateTimePipe,
        AuditorPipe,
        SafePipe,
        GrantedPipe,
        SearchInputComponent,
        CreateButtonComponent,
        ActionsToolbarComponent,
        ActionsToolbarPaneComponent,
        DetailsButtonComponent,
        BackButtonComponent,
        DeleteButtonComponent,
        SaveButtonComponent,
        EmptyComponent,
        GridComponent,
    ],
    exports: [
        FormsModule,
        ReactiveFormsModule,
        RouterModule,
        DebounceInputDirective,
        DateTimePipe,
        AuditorPipe,
        SafePipe,
        GrantedPipe,
        InputTextModule,
        MessagesModule,
        ConfirmDialogModule,
        SearchInputComponent,
        CreateButtonComponent,
        ActionsToolbarComponent,
        ActionsToolbarPaneComponent,
        DetailsButtonComponent,
        BackButtonComponent,
        DeleteButtonComponent,
        SaveButtonComponent,
        InputTextareaModule,
        DropdownModule,
        OverlayPanelModule,
        PanelModule,
        InputTrimModule,
        FieldsetModule,
        ContextMenuModule,
        ConfirmPopupModule,
        ScrollPanelModule,
        InputNumberModule,
        EssentialControlsModule,
    ],
    schemas: [CUSTOM_ELEMENTS_SCHEMA],
    providers: [CurrencyPipe],
})
export class FormControlsModule {}
