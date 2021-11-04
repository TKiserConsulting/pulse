import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ClassDetailsDto } from '@app/api/models';
import { ClassesService } from '@app/api/services';
import { BaseDetailsComponent } from '@app/shared/components/base-details.component';
import { ErrorHandlingService } from '@pulse/sdk';
import { MessageService, ConfirmationService } from 'primeng/api';
import { Observable } from 'rxjs';

@Component({
    selector: 'app-class-details',
    templateUrl: './class-details.component.html',
    styleUrls: ['./class-details.component.scss'],
})
export class ClassDetailsComponent extends BaseDetailsComponent<ClassDetailsDto> {
    constructor(
        route: ActivatedRoute,
        formBuilder: FormBuilder,
        router: Router,
        messageService: MessageService,
        errorHandlingService: ErrorHandlingService,
        confirmationService: ConfirmationService,
        private apiService: ClassesService
    ) {
        super(
            route,
            formBuilder,
            router,
            messageService,
            errorHandlingService,
            confirmationService,
            'class'
        );
    }

    protected buildForm(): FormGroup {
        return this.formBuilder.group({
            name: [null, [Validators.required]],
        });
    }

    protected loadCore(req: { id: string }): Observable<ClassDetailsDto> {
        //this.classId = req.id;
        return this.apiService.read(req);
    }

    protected createCore(req: { body: any }): Observable<ClassDetailsDto> {
        return this.apiService.create(req);
    }

    protected updateCore(req: {
        id: string;
        body: any;
    }): Observable<ClassDetailsDto> {
        return this.apiService.update(req);
    }

    protected removeCore(req: { id: string }): Observable<void> {
        return this.apiService.delete(req);
    }
}
