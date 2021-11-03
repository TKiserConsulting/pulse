import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Identity } from '@app/shared/models/auth.models';
import { AuthService } from '@app/shared/services/auth.service';
import { MessageService } from 'primeng/api';

@Component({
    selector: 'app-dashboard',
    templateUrl: './dashboard.component.html',
    styleUrls: ['./dashboard.component.scss'],
})
export class DashboardComponent implements OnInit {
    user: Identity | null = null;

    constructor(
        private messageService: MessageService,
        private authService: AuthService,
        private router: Router
    ) {
        this.authService.identity$.subscribe((i) => {
            this.user = i;
        });
    }

    ngOnInit(): void {}

    navigateSettings() {
        this.router.navigate(['instructor', 'settings'], { replaceUrl: true });
    }

    navigateReports() {
        this.router.navigate(['instructor', 'report-params'], {
            replaceUrl: true,
        });
    }
}
