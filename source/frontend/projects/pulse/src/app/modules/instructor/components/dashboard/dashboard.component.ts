import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { Identity } from '@app/shared/models/auth.models';
import { AuthService } from '@app/shared/services/auth.service';

@Component({
    selector: 'app-dashboard',
    templateUrl: './dashboard.component.html',
    styleUrls: ['./dashboard.component.scss'],
})
export class DashboardComponent {
    user: Identity | null = null;

    constructor(private authService: AuthService, private router: Router) {
        this.authService.identity$.subscribe((i) => {
            this.user = i;
        });
    }

    navigateSettings() {
        this.router.navigate(['instructor', 'settings'], { replaceUrl: true });
    }

    navigateReports() {
        this.router.navigate(['instructor', 'report-params'], {
            replaceUrl: true,
        });
    }
}
