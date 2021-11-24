import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import {
    InstructorEmoticonListItemDto,
    InstructorSettingsDetailsDto,
} from '@app/api/models';
import { EmoticonsService } from '@app/api/services/emoticons.service';
import { firstValueFrom } from 'rxjs';
import { ErrorHandlingService, trackProcessing } from '@pulse/sdk';
import { MessageService } from 'primeng/api';
import {
    FormBuilder,
    FormControl,
    FormGroup,
    Validators,
} from '@angular/forms';
import { InstructorSettingsService } from '@app/api/services';

@Component({
    selector: 'app-settings',
    templateUrl: './settings.component.html',
    styleUrls: ['./settings.component.scss'],
})
export class SettingsComponent implements OnInit {
    settings: InstructorSettingsDetailsDto | null = null;

    emoticons: InstructorEmoticonListItemDto[] | null = null;

    model: any;

    processing = false;

    frm: FormGroup;

    constructor(
        private formBuilder: FormBuilder,
        private messageService: MessageService,
        private emoticonsService: EmoticonsService,
        private settingsService: InstructorSettingsService,
        private router: Router,
        private errorHandlingService: ErrorHandlingService
    ) {
        this.frm = this.formBuilder.group({
            sessionTimeoutHours: new FormControl('', [
                Validators.required,
                Validators.maxLength(5),
            ]),
            emoticonTapDelaySeconds: new FormControl('', [
                Validators.required,
                Validators.maxLength(5),
            ]),
        });
    }

    async ngOnInit(): Promise<void> {
        this.settingsService.load().subscribe((value) => {
            this.settings = value;
            this.frm.patchValue(this.settings);
        });

        this.emoticonsService.list().subscribe((value) => {
            this.emoticons = value;
            this.model = this.emoticons.reduce(
                (obj, item) =>
                    Object.assign(obj, {
                        [item.id]: {
                            id: item.id,
                            title: item.title,
                            color: item.color,
                        },
                    }),
                {}
            );
        });
    }

    onColor(event: any) {
        console.log('Color changed', event.value);
    }

    @trackProcessing('processing')
    async saveSettings() {
        try {
            await firstValueFrom(
                this.settingsService.update({
                    body: { ...this.frm.value },
                })
            );

            this.messageService.add({
                severity: 'success',
                detail: 'Settings updated',
            });
        } catch (err: any) {
            await this.errorHandlingService.populateDefault(err);
        }
    }

    @trackProcessing('processing')
    async saveEmoticons() {
        if (!this.emoticons) {
            return;
        }

        this.emoticons.forEach((e) => {
            e.title = this.model[e.id].title || e.title;
            e.color = this.model[e.id].color;
        });

        try {
            await firstValueFrom(
                this.emoticonsService.update({
                    body: { items: this.emoticons },
                })
            );

            this.messageService.add({
                severity: 'success',
                detail: 'Emoticons updated',
            });
        } catch (err: any) {
            await this.errorHandlingService.populateDefault(err);
        }
    }

    cancel() {
        this.navigateMainScreen();
    }

    private navigateMainScreen() {
        this.router.navigate(['instructor'], { replaceUrl: true });
    }
}
