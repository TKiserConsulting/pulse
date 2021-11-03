import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ClassListItemDto, SessionListItemDto } from '@app/api/models';
import { ClassesService, SessionsService } from '@app/api/services';
import { ErrorHandlingService, trackProcessing } from '@pulse/sdk';
import { lastValueFrom, map } from 'rxjs';

@Component({
    selector: 'app-report-parameters',
    templateUrl: './report-parameters.component.html',
    styleUrls: ['./report-parameters.component.scss'],
})
export class ReportParametersComponent implements OnInit {
    processing = false;
    loadingClasses = false;
    loadingDates = false;

    classes!: ClassListItemDto[];

    selectedClass?: ClassListItemDto;

    dates!: SessionListItemDto[];

    selectedDate?: SessionListItemDto;

    constructor(
        private classesService: ClassesService,
        private sessionsService: SessionsService,
        private errorHandlingService: ErrorHandlingService,
        private router: Router
    ) {}

    sleep(ms: number) {
        return new Promise((resolve) => setTimeout(resolve, ms));
    }

    @trackProcessing('loadingClasses')
    async ngOnInit() {
        this.classes = await lastValueFrom(
            this.classesService.find({
                'range-disable-total': true,
            })
        );
    }

    @trackProcessing('loadingDates')
    async classSelected(event: any) {
        const theClass = event.value as ClassListItemDto;
        this.dates = [];
        this.selectedDate = undefined;
        this.dates = await lastValueFrom(
            this.sessionsService
                .find({
                    'range-disable-total': true,
                    filterBy: { classId: theClass.id },
                })
                .pipe(
                    map((items) => {
                        items.forEach((m) => {
                            (m as any).name = new Date(
                                m.created
                            ).toLocaleString('en-US');
                            // .toISOString()
                            // .substring(0, 10);
                        });
                        return items;
                    })
                )
        );
    }

    back() {
        this.router.navigate(['instructor'], { replaceUrl: true });
    }

    showReport() {
        this.router.navigate(['instructor', 'report'], {
            replaceUrl: true,
            queryParams: {
                sessionId: this.selectedDate?.id,
            },
        });
    }
}
