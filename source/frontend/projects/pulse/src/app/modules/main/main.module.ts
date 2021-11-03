import { NgModule } from '@angular/core';
import { ConfirmationService, MessageService } from 'primeng/api';
import { SharedModule } from 'projects/pulse/src/app/shared/modules/shared.module';
import { ReactiveFormsModule } from '@angular/forms';
import { ERROR_HANDLING_MESSAGE_SERVICE } from '@pulse/sdk';
import { AnonymousLayoutComponent } from '../../layouts/anonymous-layout/anonymous-layout.component';
import { HeaderLogoComponent } from './components/header-logo/header-logo.component';
import { ErrorComponent } from './components/errors/error.component';
import { Error403Component } from './components/errors/error403.component';
import { PageNotFoundComponent } from './components/errors/page-not-found.component';
import { SigninComponent } from './components/signin/signin.component';
import { RegisterAccountComponent } from './components/register-account/register-account.component';
import { EssentialControlsModule } from '@app/shared/modules/essential-controls.module';
import { StudentSigninComponent } from './components/student-signin/student-signin.component';

@NgModule({
    imports: [SharedModule, ReactiveFormsModule, EssentialControlsModule],
    providers: [
        ConfirmationService,
        MessageService,
        {
            provide: ERROR_HANDLING_MESSAGE_SERVICE,
            useExisting: MessageService,
        },
    ],
    declarations: [
        AnonymousLayoutComponent,
        HeaderLogoComponent,
        PageNotFoundComponent,
        ErrorComponent,
        Error403Component,
        SigninComponent,
        RegisterAccountComponent,
        StudentSigninComponent,
    ],
    exports: [],
})
export class MainModule {}
