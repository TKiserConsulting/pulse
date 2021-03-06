import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ClassListItemDto } from '@app/api/models';
import { ClassesService, SessionsService } from '@app/api/services';
import { ErrorHandlingService, trackProcessing } from '@pulse/sdk';
import { ConfirmationService, MenuItem } from 'primeng/api';
import { ContextMenu } from 'primeng/contextmenu';
import { firstValueFrom, lastValueFrom } from 'rxjs';
import { InstructorSessionService } from '../../services/instructor-session.service';

@Component({
    selector: 'app-class-list',
    templateUrl: './class-list.component.html',
    styleUrls: ['./class-list.component.scss'],
})
export class ClassListComponent implements OnInit {
    @ViewChild('contextMenu') contextMenu!: ContextMenu;

    private menuItemRename: MenuItem = { label: 'Rename' };

    private menuItemDelete: MenuItem = { label: 'Delete' };

    private menuItemReports: MenuItem = { label: 'Reports' };

    private menuItemSession: MenuItem = { label: 'Start a Session' };

    public loading = false;

    public processing = false;

    public items: ClassListItemDto[] = [];

    public menuItems: MenuItem[] = [
        this.menuItemRename,
        this.menuItemDelete,
        this.menuItemReports,
        this.menuItemSession,
    ];

    constructor(
        private errorHandlingService: ErrorHandlingService,
        private apiService: ClassesService,
        private router: Router,
        private route: ActivatedRoute,
        private confirmationService: ConfirmationService,
        private sessionsService: SessionsService,
        private sessionService: InstructorSessionService
    ) {}

    async ngOnInit(): Promise<void> {
        await this.load();
    }

    @trackProcessing('loading')
    public async load() {
        try {
            this.items = await lastValueFrom(
                this.apiService.find({
                    'range-disable-total': true,
                })
            );
        } catch (err: any) {
            await this.errorHandlingService.populateDefault(err);
        }
    }

    public showContextMenu(item: ClassListItemDto, event: any) {
        this.menuItemRename.command = () => this.renameClass(item);
        this.menuItemDelete.command = () => this.confirmDeleteClass(item);
        this.menuItemSession.command = () => this.confirmStartSession(item);
        this.contextMenu.show(event);
    }

    private async renameClass(item: ClassListItemDto) {
        this.router.navigate([item.id], { relativeTo: this.route });
    }

    private confirmDeleteClass(item: ClassListItemDto) {
        this.confirmationService.confirm({
            // target: event.target,
            message: `Delete class '${item.name}'?`,
            icon: 'pi',
            accept: () => {
                this.deleteClass(item);
            },
        });
    }

    @trackProcessing('processing')
    private async deleteClass(item: ClassListItemDto) {
        await firstValueFrom(this.apiService.delete({ id: item.id }));
        await this.load();
    }

    private confirmStartSession(item: ClassListItemDto) {
        this.confirmationService.confirm({
            // target: event.target,
            message: `Start new session for the class '${item.name}'?`,
            icon: 'pi',
            accept: () => {
                this.startSession(item);
            },
        });
    }

    @trackProcessing('processing')
    private async startSession(item: ClassListItemDto) {
        await firstValueFrom(
            this.sessionsService.create({ body: { classId: item.id } })
        );
        this.sessionService.session$.next(null);
        this.router.navigate(['instructor', 'session'], {
            replaceUrl: true,
        });
    }
}
