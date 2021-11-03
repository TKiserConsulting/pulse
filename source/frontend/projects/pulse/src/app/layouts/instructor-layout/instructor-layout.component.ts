import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { SessionsService } from '@app/api/services';
import { InstructorSessionService } from '@app/modules/instructor/services/instructor-session.service';
import { environment } from '@env/environment';
import { ActiveSessionComponent } from '../../modules/instructor/components/active-session/active-session.component';

@Component({
    selector: 'app-instructor-layout',
    templateUrl: './instructor-layout.component.html',
    styleUrls: ['./instructor-layout.component.scss'],
})
export class InstructorLayoutComponent implements OnInit {
    public version = environment.version;

    public initialized = false;

    constructor(
        private sessionService: InstructorSessionService,
        private router: Router,
        private route: ActivatedRoute
    ) {}

    public async ngOnInit(): Promise<void> {
        let activeSession = await this.sessionService.getActiveSession();
        if (activeSession) {
            if (this.route.snapshot.component !== ActiveSessionComponent) {
                this.router.navigate(['instructor', 'session'], {
                    replaceUrl: true,
                });
            }
        }
        this.initialized = true;
    }
}
