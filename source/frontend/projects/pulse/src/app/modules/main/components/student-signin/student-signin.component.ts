import { Component, OnInit } from '@angular/core';
import {
    FormGroup,
    FormBuilder,
    FormControl,
    Validators,
} from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { StudentAuthService } from '@app/shared/services/student-auth.service';
import { ErrorHandlingService, trackProcessing } from '@pulse/sdk';
import { MessageService } from 'primeng/api';

@Component({
    selector: 'app-student-signin',
    templateUrl: './student-signin.component.html',
    styleUrls: ['./student-signin.component.scss'],
})
export class StudentSigninComponent implements OnInit {
    public frm: FormGroup;

    private returnUrl: string | null = null;

    public displaySessionEnded = false;

    public processing = false;
    constructor(
        private formBuilder: FormBuilder,
        private route: ActivatedRoute,
        private router: Router,
        private authService: StudentAuthService,
        private messageService: MessageService,
        private errorHandlingService: ErrorHandlingService
    ) {
        this.frm = this.formBuilder.group({
            sessionCode: new FormControl('', [Validators.required]),
        });
    }

    ngOnInit() {
        this.returnUrl = this.route.snapshot.queryParamMap.get('return') || '';

        const sessionEnded = this.route.snapshot.queryParamMap.has('ended');
        if (sessionEnded) {
            this.displaySessionEnded = true;
        }

        if (this.route.snapshot.data.logout) {
            // this.authService.signout();
            if (this.returnUrl) {
                this.router.navigateByUrl(decodeURI(this.returnUrl));
            } else {
                this.router.navigate(['student', 'join']);
            }
        }
    }

    @trackProcessing('processing')
    public async signin() {
        if (this.frm.valid) {
            try {
                await this.authService.signin(this.frm.value);
                if (this.returnUrl) {
                    this.router.navigateByUrl(decodeURI(this.returnUrl));
                } else {
                    this.router.navigate(['student', 'session'], {
                        replaceUrl: true,
                    });
                }
            } catch (err: any) {
                console.log(err);
                if (err.code == 'UNAUTHENTICATED') {
                    this.messageService.add({
                        severity: 'error',
                        detail: `There is no active session for code '${this.frm.value.sessionCode}'`,
                    });
                } else {
                    await this.errorHandlingService.populateDefault(err);
                }
            }
        } else {
            this.frm.markAllAsTouched();
        }
    }

    public closeSessionEndedDialog() {
        this.displaySessionEnded = false;
        this.router.navigate(['student', 'join'], {
            replaceUrl: true,
        });
    }
}
