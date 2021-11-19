import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { Identity } from '@app/shared/models/auth.models';
import { AuthService } from '@app/shared/services/auth.service';
import { MenuItem } from 'primeng/api';
import { InstructorSessionService } from '../../services/instructor-session.service';

@Component({
    selector: 'app-header-bar',
    templateUrl: './header-bar.component.html',
    styleUrls: ['./header-bar.component.scss'],
})
export class HeaderBarComponent {
    user: Identity | null = null;

    menuItems: MenuItem[];

    constructor(
        private authService: AuthService,
        private router: Router,
        private sessionService: InstructorSessionService
    ) {
        this.menuItems = [
            {
                label: 'Profile',
                command: () => this.navigateProfile(),
            },
            {
                label: 'Sign Out',
                command: () => this.signout(),
            },
        ];

        this.authService.identity$.subscribe((i) => {
            this.user = i;
        });
    }

    public getImageLink() {
        return this.sessionService.profileImageTimestamp$;
    }

    private navigateProfile() {
        this.router.navigate(['instructor', 'profile'], { replaceUrl: true });
    }

    private signout() {
        this.router.navigate(['signout']);
    }
}
