import { Component } from '@angular/core';
import {
    FormBuilder,
    FormControl,
    FormGroup,
    Validators,
} from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '@app/shared/services/auth.service';
import { ErrorHandlingService, trackProcessing } from '@pulse/sdk';

@Component({
    selector: 'app-register-account',
    templateUrl: './register-account.component.html',
    styleUrls: ['./register-account.component.scss'],
})
export class RegisterAccountComponent {
    public frm: FormGroup;

    private returnUrl: string | null = null;

    public processing = false;

    constructor(
        private formBuilder: FormBuilder,
        private route: ActivatedRoute,
        private router: Router,
        private authService: AuthService,
        private errorHandlingService: ErrorHandlingService
    ) {
        this.frm = this.formBuilder.group({
            email: new FormControl('', [Validators.required, Validators.email]),
            password: new FormControl('', [Validators.required]),
            firstName: new FormControl('', [Validators.required]),
            lastName: new FormControl('', [Validators.required]),
        });
    }

    @trackProcessing('processing')
    public async register() {
        if (this.frm.valid) {
            try {
                await this.authService.register(this.frm.value);
                this.router.navigateByUrl(decodeURI(this.returnUrl || ''));
            } catch (err: any) {
                await this.errorHandlingService.populateDefault(err);
            }
        } else {
            this.frm.markAllAsTouched();
        }
    }
}
