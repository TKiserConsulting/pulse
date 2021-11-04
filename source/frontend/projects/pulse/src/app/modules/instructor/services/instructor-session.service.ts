import { Injectable } from '@angular/core';
import { ActiveSessionDetailsDto } from '@app/api/models';
import { SessionQuestionDetailsDto } from '@app/api/models/session-question-details-dto';
import { SessionsService } from '@app/api/services';
import { AuthService } from '@app/shared/services/auth.service';
import * as signalR from '@microsoft/signalr';
import { BehaviorSubject } from 'rxjs';

@Injectable({
    providedIn: 'root',
})
export class InstructorSessionService {
    public session$ = new BehaviorSubject<ActiveSessionDetailsDto | null>(null);

    public emoticonTap$ = new BehaviorSubject<string | null>(null);

    public question$ = new BehaviorSubject<SessionQuestionDetailsDto | null>(
        null
    );

    public profileImageTimestamp$ = new BehaviorSubject<string>('');

    private hubConnection: signalR.HubConnection | null = null;

    constructor(
        private sessionApiService: SessionsService,
        private authService: AuthService
    ) {}

    public async getActiveSession() {
        if (this.session$.value) {
            return this.session$.value;
        }

        const session = await this.sessionApiService
            .getActiveSession()
            .toPromise();

        this.session$.next(session || null);

        return session;
    }

    public async connectHub() {
        const hubUrl = '/hubs/instructor';

        await this.disconnectHub();

        this.hubConnection = new signalR.HubConnectionBuilder()
            .configureLogging(signalR.LogLevel.Information)
            .withUrl(hubUrl, {
                accessTokenFactory: async () => {
                    return (await this.authService.getAccessToken()) || '';
                },
            })
            .build();

        this.hubConnection.onclose(this.hubConnectionClosed.bind(this));

        await this.hubConnection
            .start()
            .then(() => {
                console.log(`SignalR connected to hub ${hubUrl}`);
            })
            .catch((err) => {
                this.hubConnection = null;
                return console.error(err.toString());
            });

        if (this.hubConnection) {
            this.hubConnection.on('EmoticonTap', this.onEmoticonTap.bind(this));
            this.hubConnection.on('Question', this.onQuestion.bind(this));
        }

        return this.hubConnection;
    }

    public async disconnectHub() {
        if (this.hubConnection) {
            await this.hubConnection.stop();
            this.hubConnection = null;
        }
    }

    private hubConnectionClosed(error?: Error) {
        this.hubConnection = null;
        console.log('Hub connection terminated', error);
    }

    private onEmoticonTap(instructorEmoticonId: string) {
        const noDash = instructorEmoticonId?.replace(/-/gi, '');
        this.emoticonTap$.next(noDash);
    }

    private onQuestion(question: SessionQuestionDetailsDto) {
        const checkin = this.session$.value?.activeCheckin;
        const questionCheckinId = question.sessionCheckinId.replace(/-/gi, '');
        if (checkin && checkin.sessionCheckinId == questionCheckinId) {
            this.question$.next(question);
        }
    }
}
