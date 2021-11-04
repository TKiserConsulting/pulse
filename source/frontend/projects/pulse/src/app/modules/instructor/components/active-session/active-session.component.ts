import { Component, OnDestroy, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ActiveSessionDetailsDto } from '@app/api/models';
import { CheckinType } from '@app/api/models/checkin-type';
import { SessionsService } from '@app/api/services';
import { CheckinService } from '@app/api/services/checkin.service';
import { SessionUtilsService } from '@app/modules/main/services/session-utils.service';
import { trackProcessing } from '@pulse/sdk';
import { ConfirmationService } from 'primeng/api';
import { filter, Subscription } from 'rxjs';
import { InstructorSessionService } from '../../services/instructor-session.service';

@Component({
    selector: 'app-active-session',
    templateUrl: './active-session.component.html',
    styleUrls: ['./active-session.component.scss'],
})
export class ActiveSessionComponent implements OnInit, OnDestroy {
    session?: ActiveSessionDetailsDto;

    public processing = false;

    public checkinButtonsVisible = false;

    private tapSubscription?: Subscription;

    constructor(
        private router: Router,
        private confirmationService: ConfirmationService,
        private sessionsService: SessionsService,
        private sessionService: InstructorSessionService,
        private checkinService: CheckinService,
        private sessionUtils: SessionUtilsService
    ) {}

    async ngOnInit() {
        this.session = await this.sessionService.getActiveSession();
        if (this.session) {
            await this.sessionService.connectHub();
            this.tapSubscription = this.sessionService.emoticonTap$
                .pipe(filter((m) => m != null))
                .subscribe(this.onTap.bind(this));
        } else {
            this.router.navigate(['instructor'], { replaceUrl: true });
        }
    }

    async ngOnDestroy() {
        this.tapSubscription?.unsubscribe();
        this.sessionService.disconnectHub();
    }

    public confirmEndSession() {
        this.confirmationService.confirm({
            message: 'Do you want to end this session?',
            icon: 'pi',
            accept: () => {
                this.endSession();
            },
        });
    }

    @trackProcessing('processing')
    private async endSession() {
        await this.sessionsService.endSession().toPromise();
        this.router.navigate(['instructor'], { replaceUrl: true });
    }

    public getCheckinTitle() {
        return this.sessionUtils.getCheckinTitle(this.session?.activeCheckin);
    }

    @trackProcessing('processing')
    public async checkIn(checkin: boolean) {
        const result = await this.checkinService
            .create({
                body: {
                    sessionId: this.session?.id || '',
                    checkinType: checkin
                        ? CheckinType.CheckIn
                        : CheckinType.WrapUp,
                },
            })
            .toPromise();

        if (result && this.session) {
            this.session.activeCheckin = result;
        }

        this.checkinButtonsVisible = false;
    }

    public getQuestionCount() {
        const count = this.session?.activeCheckin?.questions?.length;

        return count ? count.toString() : '';
    }

    @trackProcessing('processing')
    public async endCheckin() {
        if (this.session) {
            await this.checkinService
                .finish({
                    body: {
                        sessionCheckinId:
                            this.session.activeCheckin?.sessionCheckinId || '',
                    },
                })
                .toPromise();

            this.session.activeCheckin = undefined;

            this.checkinButtonsVisible = false;
        }
    }

    private onTap(instructorEmoticonId: string | null) {
        const item = this.session?.emoticons.find(
            (i) => i.instructorEmoticonId === instructorEmoticonId
        );
        if (item) {
            item.tapCount += 1;
            console.log('+1', item.title);
        }
    }
}
