import { Directive, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ErrorHandlingService, trackProcessing } from '@pulse/sdk';
import { ConfirmationService, MessageService } from 'primeng/api';
import { Observable } from 'rxjs';
import { BaseCoreComponent } from './base-core.component';

@Directive()
// eslint-disable-next-line @angular-eslint/directive-class-suffix
export abstract class BaseDetailsComponent<TModel extends { id?: string }>
    extends BaseCoreComponent<TModel>
    implements OnInit
{
    public frm!: FormGroup;

    constructor(
        protected route: ActivatedRoute,
        protected formBuilder: FormBuilder,
        protected router: Router,
        protected messageService: MessageService,
        protected errorHandlingService: ErrorHandlingService,
        protected confirmationService: ConfirmationService,
        protected componentKey: string
    ) {
        super(
            route,
            router,
            messageService,
            errorHandlingService,
            confirmationService,
            componentKey
        );
    }

    protected abstract buildForm(): FormGroup;

    protected abstract createCore(req: { body: any }): Observable<TModel>;

    protected abstract updateCore(req: {
        id: string;
        body: any;
    }): Observable<TModel>;

    public async ngOnInit(): Promise<void> {
        this.frm = this.buildForm();
        if (this.readonly) {
            this.frm.disable();
        }

        await this.preload();
    }

    @trackProcessing('processing')
    public async save() {
        if (this.frm.valid) {
            try {
                let model: TModel | undefined;
                if (this.editMode) {
                    model = await this.updateCore({
                        id: this.id,
                        body: this.frm.value,
                    }).toPromise();
                } else {
                    model = await this.createCore({
                        body: this.frm.value,
                    }).toPromise();
                }

                if (model) {
                    this.bind(model);

                    if (model.id) {
                        this.postSaveNavigate();
                    }
                }

                this.messageService.add({
                    severity: 'success',
                    detail: 'Done',
                });
            } catch (err: any) {
                await this.errorHandlingService.populateDefault(
                    err,
                    this.getValidationPrefix(),
                    this.frm
                );
            }
        } else {
            this.frm.markAllAsTouched();
        }
    }

    protected postSaveNavigate() {
        this.router.navigate([`../`], {
            relativeTo: this.route,
            replaceUrl: true,
        });
        // this.router.navigateByUrl(`${this.getListPathPart()}/${id}`, {
        //     replaceUrl: true,
        // });
    }

    protected bind(model: TModel) {
        super.bind(model);

        this.patchForm(model);

        this.frm.markAsUntouched();
    }

    protected patchForm(model: any) {
        this.frm.patchValue(model);
    }
}
