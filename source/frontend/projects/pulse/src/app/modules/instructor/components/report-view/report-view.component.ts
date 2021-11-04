import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import {
    CheckinData,
    CheckinType,
    SessionReportResultDto,
} from '@app/api/models';
import { ReportsService } from '@app/api/services';
import { SessionUtilsService } from '@app/modules/main/services/session-utils.service';
import { lastValueFrom } from 'rxjs';

@Component({
    selector: 'app-report-view',
    templateUrl: './report-view.component.html',
    styleUrls: ['./report-view.component.scss'],
})
export class ReportViewComponent implements OnInit {
    data!: SessionReportResultDto;

    title: string = '';

    chartData: any;

    chartOptions: any;

    selectedCheckin?: CheckinData;

    constructor(
        private activatedRoute: ActivatedRoute,
        private reportsService: ReportsService,
        private sessionUtils: SessionUtilsService
    ) {
        this.chartOptions = {
            plugins: {
                legend: {
                    labels: {
                        color: '#495057',
                    },
                },
            },
            scales: {
                x: {
                    ticks: {
                        color: '#495057',
                    },
                    grid: {
                        color: '#ebedef',
                    },
                },
                y: {
                    ticks: {
                        color: '#495057',
                    },
                    grid: {
                        color: '#ebedef',
                    },
                },
            },
        };
    }

    async ngOnInit() {
        const sessionId =
            this.activatedRoute.snapshot.queryParamMap.get('sessionId');

        if (!sessionId) {
            return;
        }

        this.data = await lastValueFrom(
            this.reportsService.sessionReport({ sessionId: sessionId })
        );

        this.title = new Date(this.data.session?.created || '').toLocaleString(
            'en-US'
        );

        this.buildChartData();

        this.buildCheckins();
    }

    private buildChartData() {
        let startDate = new Date(this.data.session.created);
        let endDate = startDate;

        const hasTaps = this.data.emoticons.some((e) => e.taps.length > 0);

        // Calculate report finish date
        if (this.data.session.finished) {
            endDate = new Date(this.data.session.finished);
        } else if (hasTaps) {
            this.data.emoticons.forEach((e) => {
                if (e.taps.length > 0) {
                    const tapDate = new Date(
                        e.taps[e.taps.length - 1].timestamp
                    );
                    if (tapDate > endDate) {
                        endDate = tapDate;
                    }
                }
            });
        }

        // Calculate report start date
        if (hasTaps) {
            startDate = endDate;
            this.data.emoticons.forEach((e) => {
                if (e.taps.length > 0) {
                    const tapDate = new Date(e.taps[0].timestamp);
                    if (tapDate < startDate) {
                        startDate = tapDate;
                    }
                }
            });
        }

        const tapCount: Array<Array<number>> = this.data.emoticons.map(
            () => []
        );

        // Fill out empty intervals
        this.data.emoticons.forEach((e, emoticonIndex) => {
            let date = startDate;
            let i = 0;
            while (date <= endDate) {
                const newDate = this.addMinutes(
                    date,
                    this.data.intervalMinutes
                );
                if (
                    i < e.taps.length &&
                    new Date(e.taps[i].timestamp) < newDate
                ) {
                    tapCount[emoticonIndex].push(e.taps[i].tapCount);
                    i++;
                } else {
                    tapCount[emoticonIndex].push(0);
                }
                date = newDate;
            }
        });

        const labels: Array<string> = [];
        let date = startDate;
        while (date < endDate) {
            labels.push(`${date.getHours()}:${date.getMinutes()}`);
            date = this.addMinutes(date, this.data.intervalMinutes);
        }

        const datasets = this.data.emoticons.map((e, index) => {
            return {
                label: e.emoticon.title,
                data: tapCount[index],
                fill: false,
                borderColor: e.emoticon.color,
                tension: 0.4,
            };
        });

        this.chartData = {
            labels,
            datasets,
        };
    }

    private buildCheckins() {
        var manyCheckins =
            this.data.checkins.filter(
                (c) => c.checkin.checkinType == CheckinType.CheckIn
            ).length > 1;
        var manyWrapups =
            this.data.checkins.filter(
                (c) => c.checkin.checkinType == CheckinType.WrapUp
            ).length > 1;

        let i = 1;
        this.data.checkins.forEach((c) => {
            if (c.checkin.checkinType == CheckinType.CheckIn) {
                (c as any).name = manyCheckins ? `Check-in ${i++}` : 'Check-in';
            }
        });

        i = 1;
        this.data.checkins.forEach((c) => {
            if (c.checkin.checkinType == CheckinType.WrapUp) {
                (c as any).name = manyWrapups ? `Wrap Up ${i++}` : 'Wrap Up';
            }
        });
    }

    private addMinutes(date: Date, minutes: number) {
        return new Date(date.getTime() + minutes * 60000);
    }
}
