import { Component } from '@angular/core';
import {
    FormBuilder,
    FormControl,
    FormGroup,
    Validators,
} from '@angular/forms';
import { ErrorHandlingService, trackProcessing } from '@pulse/sdk';
import { AuthService } from '@app/shared/services/auth.service';
import { ProfilesService } from '@app/api/services';
import { MessageService } from 'primeng/api';
import { lastValueFrom } from 'rxjs';
import { InstructorProfileDetailsDto } from '@app/api/models/instructor-profile-details-dto';
import { InstructorSessionService } from '../../services/instructor-session.service';
import { Router } from '@angular/router';

@Component({
    templateUrl: './profile.component.html',
    styleUrls: ['./profile.component.scss'],
})
export class ProfileComponent {
    processing = false;
    uploading = false;

    frm: FormGroup;

    profile: InstructorProfileDetailsDto | null = null;

    // public frmPassword: FormGroup;

    private prefix = 'app.forms.profile.validations';

    constructor(
        private formBuilder: FormBuilder,
        private messageService: MessageService,
        private profilesService: ProfilesService,
        private authService: AuthService,
        private errorHandlingService: ErrorHandlingService,
        private sessionService: InstructorSessionService,
        private router: Router
    ) {
        this.frm = this.formBuilder.group({
            firstName: new FormControl('', [
                Validators.required,
                Validators.maxLength(50),
            ]),
            lastName: new FormControl('', [
                Validators.required,
                Validators.maxLength(50),
            ]),
            school: new FormControl('', [Validators.maxLength(100)]),
            subject: new FormControl('', [Validators.maxLength(100)]),
            city: new FormControl('', [Validators.maxLength(50)]),
            state: new FormControl('', [Validators.maxLength(50)]),
        });

        // this.frmPassword = this.formBuilder.group({
        //     oldPassword: new FormControl('', [Validators.required]),
        //     newPassword: new FormControl('', [Validators.required]),
        //     passwordConfirm: new FormControl('', [
        //         Validators.required,
        //         this.passwordShouldMatch.bind(this),
        //     ]),
        // });

        this.profilesService.load().subscribe((result) => {
            this.profile = result || {};
            if (result) {
                this.frm.patchValue(result);
            }
        });
    }

    public getImageLink() {
        return this.sessionService.profileImageTimestamp$;
    }

    @trackProcessing('processing')
    public async save() {
        if (this.frm.valid) {
            try {
                await lastValueFrom(
                    this.profilesService.update({ body: this.frm.value })
                );
                this.messageService.add({
                    severity: 'success',
                    detail: 'Profile updated successully',
                });
                this.navigateMainScreen();
            } catch (err: any) {
                await this.errorHandlingService.populateDefault(
                    err,
                    this.prefix
                );
            }
        } else {
            this.frm.markAllAsTouched();
        }
    }

    @trackProcessing('uploading')
    @trackProcessing('processing')
    public async saveImage(event: any) {
        const file = event.target.files[0];
        try {
            await lastValueFrom(
                this.profilesService.multipart(
                    {
                        body: { file: file },
                    },
                    'uploadImage'
                )
            );
            const imageTimestamp = new Date().getTime().toString();
            this.sessionService.profileImageTimestamp$.next(imageTimestamp);
            this.messageService.add({
                severity: 'success',
                detail: 'Image updated successully',
            });
        } catch (err: any) {
            await this.errorHandlingService.populateDefault(err, this.prefix);
        }
    }

    cancel() {
        this.navigateMainScreen();
    }

    private navigateMainScreen() {
        this.router.navigate(['instructor'], { replaceUrl: true });
    }

    // @trackProcessing('processing')
    // public async password() {
    //     if (this.frmPassword.valid) {
    //         try {
    //             await lastValueFrom(
    //                 this.profilesService.password({
    //                     body: this.frmPassword.value,
    //                 })
    //             );
    //             this.messageService.add({
    //                 severity: 'success',
    //                 detail: 'Password reset successully',
    //             });
    //         } catch (err: any) {
    //             await this.errorHandlingService.populateDefault(
    //                 err,
    //                 this.prefix
    //             );
    //         }
    //     } else {
    //         this.frmPassword.markAllAsTouched();
    //     }
    // }

    // private passwordShouldMatch(control: AbstractControl) {
    //     const invalid =
    //         control.value &&
    //         this.frmPassword &&
    //         control.value !== this.frmPassword.controls.newPassword.value;
    //     return invalid ? { notSame: true } : null;
    // }
}
