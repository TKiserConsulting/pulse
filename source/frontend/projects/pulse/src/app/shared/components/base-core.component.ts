import { Directive, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ErrorHandlingService, trackProcessing } from '@pulse/sdk';
import { ConfirmationService, MessageService } from 'primeng/api';
import { Observable } from 'rxjs';

@Directive()
// eslint-disable-next-line @angular-eslint/directive-class-suffix
export abstract class BaseCoreComponent<TModel extends { id?: string }>
    implements OnInit
{
    public loading = false;

    public processing = false;

    public editMode = false;

    public id: string;

    public readonly: boolean;

    public caption = '';

    protected validationPrefix: string = 'app.forms._default.validations';

    // return reference + zone
    protected reference: string;

    protected zone: string;

    protected mode: string;

    constructor(
        protected route: ActivatedRoute,
        protected router: Router,
        protected messageService: MessageService,
        protected errorHandlingService: ErrorHandlingService,
        protected confirmationService: ConfirmationService,
        protected componentKey: string
    ) {
        this.id =
            this.route.snapshot.params.id ||
            this.route.parent?.snapshot.params.id;

        this.readonly = this.route.snapshot.queryParams.mode === 'view';
        this.reference = this.route.snapshot.queryParams.ref;
        this.zone = this.route.snapshot.queryParams.zone;
        this.mode = this.route.snapshot.queryParams.mode;

        this.editMode = !!this.id;
    }

    protected getCaption(model: TModel): string {
        const o = model as any;
        return o.name || '';
    }

    protected getValidationPrefix(): string {
        return `app.forms.${this.componentKey}.validations`;
    }

    public getListPathPart(): string {
        return `/${this.componentKey}`;
        // return '..';
    }

    protected abstract loadCore(req: { id: string }): Observable<TModel>;

    protected abstract removeCore(req: { id: string }): Observable<void>;

    public async ngOnInit(): Promise<void> {
        await this.preload();
    }

    protected async preload(): Promise<void> {
        if (this.editMode) {
            await this.load();
        }
    }

    @trackProcessing('loading')
    public async load(): Promise<void> {
        try {
            const model = await this.loadCore({ id: this.id }).toPromise();
            if (model) {
                this.bind(model);
            }
        } catch (err: any) {
            await this.errorHandlingService.populateDefault(
                err,
                this.getValidationPrefix()
            );
        }
    }

    @trackProcessing('processing')
    public async remove(force?: boolean) {
        if (force) {
            try {
                await this.removeCore({
                    id: this.id,
                }).toPromise();

                this.router.navigateByUrl(`${this.getListPathPart()}`, {
                    replaceUrl: true,
                });

                this.messageService.add({
                    severity: 'success',
                    detail: 'Removed',
                });
            } catch (err: any) {
                await this.errorHandlingService.populateDefault(
                    err,
                    this.getValidationPrefix()
                );
            }
        } else {
            this.confirmationService.confirm({
                message: 'Are you sure you want to delete this record?',
                accept: () => {
                    this.remove(true);
                },
            });
        }
    }

    protected bind(model: TModel) {
        this.caption = this.getCaption(model);

        if (model.id) {
            this.id = model.id;
        }
    }
}
