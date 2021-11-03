import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ClassListItemDto } from '@app/api/models';
import { ClassesService, SessionsService } from '@app/api/services';
import { ErrorHandlingService, trackProcessing } from '@pulse/sdk';
import { lastValueFrom, Observable } from 'rxjs';

@Component({
    selector: 'app-start-session',
    templateUrl: './start-session.component.html',
    styleUrls: ['./start-session.component.scss'],
})
export class StartSessionComponent implements OnInit {
    processing = false;

    classes$: Observable<ClassListItemDto[]>;

    selectedClass?: ClassListItemDto;

    constructor(
        private classesService: ClassesService,
        private sessionsService: SessionsService,
        private errorHandlingService: ErrorHandlingService,
        private router: Router
    ) {
        this.classes$ = this.classesService.find({
            'range-disable-total': true,
        });
    }

    ngOnInit(): void {}

    onShow() {
        this.selectedClass = undefined;
    }

    @trackProcessing('processing')
    async startSession() {
        if (!this.selectedClass) {
            return;
        }

        try {
            await lastValueFrom(
                this.sessionsService.create({
                    body: { classId: this.selectedClass.id },
                })
            );
            this.router.navigate(['instructor', 'session'], {
                replaceUrl: true,
            });
        } catch (err: any) {
            await this.errorHandlingService.populateDefault(err);
        }
    }
}
