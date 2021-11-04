import { Component, EventEmitter, Output } from '@angular/core';
import { SessionCheckinDetailsDto } from '@app/api/models/session-checkin-details-dto';
import { SessionQuestionDetailsDto } from '@app/api/models/session-question-details-dto';
import { QuestionsService } from '@app/api/services/questions.service';
import { SessionUtilsService } from '@app/modules/main/services/session-utils.service';
import { firstValueFrom, Subscription } from 'rxjs';
import { InstructorSessionService } from '../../services/instructor-session.service';
import { trackProcessing } from '@pulse/sdk';

@Component({
    selector: 'app-question-list',
    templateUrl: './question-list.component.html',
    styleUrls: ['./question-list.component.scss'],
})
export class QuestionListComponent {
    @Output() endCheckin = new EventEmitter<void>();

    display: boolean = false;

    questions: SessionQuestionDetailsDto[] = [];

    processing = false;

    private questionSubscription?: Subscription;

    private checkin?: SessionCheckinDetailsDto;

    constructor(
        private sessionService: InstructorSessionService,
        private questionsService: QuestionsService,
        private sessionUtils: SessionUtilsService
    ) {}

    @trackProcessing('processing')
    public async show(checkin: SessionCheckinDetailsDto) {
        this.checkin = checkin;
        const items = await firstValueFrom(
            this.questionsService.list({
                sessionCheckinId: checkin.sessionCheckinId,
            })
        );
        this.questions = items;
        this.checkin.questions = items;
        this.questionSubscription = this.sessionService.question$.subscribe(
            this.onNewQuestion.bind(this)
        );
        this.display = true;
    }

    public onHide() {
        this.questionSubscription?.unsubscribe();
    }

    public getCheckinTitle() {
        return this.sessionUtils.getCheckinTitle(this.checkin);
    }

    @trackProcessing('processing')
    public async dismissQuestion(item: SessionQuestionDetailsDto) {
        await firstValueFrom(
            this.questionsService.dismiss({
                body: { sessionQuestionId: item.sessionQuestionId },
            })
        );

        const index = this.questions.indexOf(item);
        this.questions.splice(index, 1);
    }

    public endCheckinClick() {
        this.endCheckin.emit();
        this.display = false;
    }

    private onNewQuestion(q: SessionQuestionDetailsDto | null) {
        if (q) {
            this.questions.push(q);
        }
    }
}
