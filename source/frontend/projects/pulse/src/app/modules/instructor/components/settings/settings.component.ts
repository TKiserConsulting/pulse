import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { InstructorEmoticonListItemDto } from '@app/api/models';
import { EmoticonsService } from '@app/api/services/emoticons.service';
import { firstValueFrom } from 'rxjs';
import { ErrorHandlingService, trackProcessing } from '@pulse/sdk';
import { MessageService } from 'primeng/api';

@Component({
    selector: 'app-settings',
    templateUrl: './settings.component.html',
    styleUrls: ['./settings.component.scss'],
})
export class SettingsComponent implements OnInit {
    emoticons: InstructorEmoticonListItemDto[] | null = null;

    model: any;

    processing = false;

    constructor(
        private messageService: MessageService,
        private emoticonsService: EmoticonsService,
        private router: Router,
        private errorHandlingService: ErrorHandlingService
    ) {}

    async ngOnInit(): Promise<void> {
        this.emoticons = await firstValueFrom(this.emoticonsService.list());

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

        console.log('Model: ', this.model);
    }

    onColor(event: any) {
        console.log('Color changed', event.value);
    }

    @trackProcessing('processing')
    async save() {
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
                detail: 'Settings updated successully',
            });

            this.navigateMainScreen();
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
