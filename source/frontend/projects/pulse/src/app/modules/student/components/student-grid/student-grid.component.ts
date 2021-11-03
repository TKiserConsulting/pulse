import { Component, OnDestroy, OnInit } from '@angular/core';
import { InstructorEmoticonListItemDto } from '@app/api/models';
import { SessionCheckinDetailsDto } from '@app/api/models/session-checkin-details-dto';
import { StudentSessionDetailsDto } from '@app/api/models/student-session-details-dto';
import { StudentsService } from '@app/api/services';
import { trackProcessing } from '@pulse/sdk';
import { MessageService } from 'primeng/api';
import { Observable } from 'rxjs';
import { StudentSessionService } from '../../services/student-session.service';

@Component({
    selector: 'app-student-grid',
    templateUrl: './student-grid.component.html',
    styleUrls: ['./student-grid.component.scss'],
})
export class StudentGridComponent implements OnInit, OnDestroy {
    session$: Observable<StudentSessionDetailsDto | null>;

    activeCheckin$: Observable<SessionCheckinDetailsDto | null>;

    toastStyle = {
        color: 'white',
        'background-color': 'blue',
    };

    public processing = false;

    constructor(
        private apiService: StudentsService,
        private sessionService: StudentSessionService,
        private messageService: MessageService
    ) {
        this.session$ = this.sessionService.session$;
        this.activeCheckin$ = this.sessionService.activeCheckin$;
        this.sessionService.loadSession();
    }

    async ngOnInit() {
        await this.sessionService.connectHub();
    }

    async ngOnDestroy() {
        await this.sessionService.disconnectHub();
    }

    public getButtonStyle(item: InstructorEmoticonListItemDto) {
        const color = item.color;
        return {
            'background-color': color,
            'border-color': color,
            color: color,
        };
    }

    @trackProcessing('processing')
    public async tap(item: InstructorEmoticonListItemDto) {
        this.messageService.clear('tap');
        this.toastStyle['background-color'] = item.color;
        this.messageService.add({
            key: 'tap',
            closable: false,
            contentStyleClass: 'tap-toast',
            life: 2000,
            detail: item.title,
        });

        await this.apiService
            .tap({
                body: {
                    instructorEmoticonId: item.id,
                },
            })
            .toPromise();
    }
}
