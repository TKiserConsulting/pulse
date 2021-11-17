import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { Identity } from '@app/shared/models/auth.models';
import { AuthService } from '@app/shared/services/auth.service';
import { InstructorSessionService } from '../../services/instructor-session.service';

@Component({
    selector: 'app-dashboard',
    templateUrl: './dashboard.component.html',
    styleUrls: ['./dashboard.component.scss'],
})
export class DashboardComponent {
    user: Identity | null = null;

    constructor(
        private authService: AuthService,
        private router: Router,
        private sessionService: InstructorSessionService
    ) {
        this.authService.identity$.subscribe((i) => {
            this.user = i;
        });
    }

    navigateSettings() {
        this.router.navigate(['instructor', 'settings'], { replaceUrl: true });
    }

    navigateClasses() {
        this.sessionService.session$.next(null);
        this.router.navigate(['instructor'], {
            replaceUrl: true,
        });
    }

    navigateReports() {
        this.router.navigate(['instructor', 'report-params'], {
            replaceUrl: true,
        });
    }
}
