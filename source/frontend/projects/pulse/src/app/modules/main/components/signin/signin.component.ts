import { Component, OnInit } from '@angular/core';
import {
    FormBuilder,
    FormControl,
    FormGroup,
    Validators,
} from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ErrorHandlingService, trackProcessing } from '@pulse/sdk';
import { AuthService } from '@app/shared/services/auth.service';

@Component({
    templateUrl: './signin.component.html',
    styleUrls: ['./signin.component.scss'],
})
export class SigninComponent implements OnInit {
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
        });
    }

    ngOnInit() {
        this.authService.clearAuth();
        this.returnUrl = this.route.snapshot.queryParamMap.get('return') || '';
        if (this.route.snapshot.data.logout) {
            this.authService.signout();
            this.router.navigateByUrl(decodeURI(this.returnUrl || ''));
        }
    }

    @trackProcessing('processing')
    public async signin() {
        if (this.frm.valid) {
            try {
                await this.authService.signin(this.frm.value);
                this.router.navigateByUrl(decodeURI(this.returnUrl || ''));
            } catch (err: any) {
                await this.errorHandlingService.populateDefault(err);
            }
        } else {
            this.frm.markAllAsTouched();
        }
    }

    public register() {
        this.router.navigate(['register']);
    }
}
