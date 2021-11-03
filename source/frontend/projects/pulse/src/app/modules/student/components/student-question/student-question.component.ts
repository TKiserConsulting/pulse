import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import {
    FormBuilder,
    FormControl,
    FormGroup,
    Validators,
} from '@angular/forms';
import { SessionCheckinDetailsDto } from '@app/api/models/session-checkin-details-dto';
import { StudentsService } from '@app/api/services';
import { StudentSessionService } from '../../services/student-session.service';
import { ErrorHandlingService, trackProcessing } from '@pulse/sdk';
import { firstValueFrom } from 'rxjs';
import { SessionQuestionDetailsDto } from '@app/api/models/session-question-details-dto';
import { MessageService } from 'primeng/api';
import { SessionUtilsService } from '@app/modules/main/services/session-utils.service';

@Component({
    selector: 'app-student-question',
    templateUrl: './student-question.component.html',
    styleUrls: ['./student-question.component.scss'],
})
export class StudentQuestionComponent implements OnInit {
    display: boolean = false;

    questions: SessionQuestionDetailsDto[] = [];

    frm: FormGroup;

    processing = false;

    @ViewChild('textInput') textInput!: ElementRef;

    private checkin: SessionCheckinDetailsDto | null = null;

    constructor(
        private sessionService: StudentSessionService,
        private apiService: StudentsService,
        private errorHandlingService: ErrorHandlingService,
        private messageService: MessageService,
        private sessionUtils: SessionUtilsService,
        formBuilder: FormBuilder
    ) {
        this.frm = formBuilder.group({
            text: new FormControl('', [
                Validators.required,
                Validators.maxLength(500),
            ]),
        });
    }

    ngOnInit(): void {
        this.sessionService.activeCheckin$.subscribe((c) => {
            this.checkin = c;
            if (!c) {
                this.hide();
            }
        });
    }

    public show() {
        this.questions = this.sessionService.questions;
        this.display = true;
    }

    public getCheckinTitle() {
        return this.sessionUtils.getCheckinTitle(this.checkin);
    }

    @trackProcessing('processing')
    public async submit() {
        if (this.frm.valid) {
            try {
                const request = {
                    sessionId: this.checkin?.sessionId || '',
                    text: this.frm.value.text,
                };
                const questionId = await firstValueFrom(
                    this.apiService.createQuestion({
                        body: request,
                    })
                );
                if (questionId) {
                    this.questions.push({
                        sessionCheckinId: this.checkin?.sessionCheckinId || '',
                        sessionQuestionId: questionId,
                        text: request.text,
                        created: Date.now().toString(),
                    });

                    this.showToast(request.text);

                    this.textInput.nativeElement.focus();
                    this.frm.patchValue({ text: '' });
                    this.frm.markAsUntouched();
                    this.frm.markAsDirty();
                    this.frm.markAsPristine();
                } else {
                    this.hide();
                }
            } catch (err: any) {
                await this.errorHandlingService.populateDefault(err);
            }
        } else {
            this.frm.markAllAsTouched();
        }
    }

    private hide() {
        this.display = false;
    }

    private showToast(text: string) {
        this.messageService.clear('tap');
        this.messageService.add({
            key: 'tap',
            closable: false,
            contentStyleClass: 'tap-toast',
            life: 2000,
            detail: text,
        });
    }
}
