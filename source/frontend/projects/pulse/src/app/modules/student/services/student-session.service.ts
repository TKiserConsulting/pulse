import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { StudentSessionDetailsDto } from '@app/api/models/student-session-details-dto';
import { BehaviorSubject } from 'rxjs';
import { SessionCheckinDetailsDto } from '@app/api/models/session-checkin-details-dto';
import { Router } from '@angular/router';
import { StudentsService } from '@app/api/services';
import { AuthService } from '@app/shared/services/auth.service';
import { SessionQuestionDetailsDto } from '@app/api/models/session-question-details-dto';

@Injectable({
    providedIn: 'root',
})
export class StudentSessionService {
    public session$ = new BehaviorSubject<StudentSessionDetailsDto | null>(
        null
    );

    public activeCheckin$ =
        new BehaviorSubject<SessionCheckinDetailsDto | null>(null);

    public questions: SessionQuestionDetailsDto[] = [];

    private hubConnection: signalR.HubConnection | null = null;

    constructor(
        private apiService: StudentsService,
        private router: Router,
        private authService: AuthService
    ) {}

    public async loadSession() {
        if (!this.session$.value) {
            this.authService.identity$.subscribe(async (identity) => {
                const session = await this.apiService
                    .getSession({
                        SessionId: identity?.sessionId || undefined,
                    })
                    .toPromise();

                if (session) {
                    this.activeCheckin$.next(session.activeCheckin || null);
                    this.questions = session.questions || [];
                } else {
                    this.router.navigate(['student', 'join'], {
                        replaceUrl: true,
                    });
                }

                this.session$.next(session || null);
            });
        }
    }

    public async connectHub() {
        const hubUrl = '/hubs/student';

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
            this.hubConnection.on('Checkin', this.onCheckin.bind(this));
            this.hubConnection.on(
                'CheckinFinish',
                this.onCheckinFinish.bind(this)
            );
            this.hubConnection.on(
                'SessionFinish',
                this.onSessionFinish.bind(this)
            );
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

    private onCheckin(checkin: SessionCheckinDetailsDto) {
        checkin.sessionCheckinId = checkin.sessionCheckinId.replace(/-/gi, '');
        checkin.sessionId = checkin.sessionId.replace(/-/gi, '');
        console.log('[Student] Checkin received', checkin);
        this.activeCheckin$.next(checkin);
    }

    private onCheckinFinish() {
        console.log('[Student] Checkin finish received');
        this.activeCheckin$.next(null);
    }

    private onSessionFinish() {
        this.session$.next(null);
        this.router.navigate(['student', 'join'], {
            replaceUrl: true,
            queryParams: { ended: true },
        });
    }
}
