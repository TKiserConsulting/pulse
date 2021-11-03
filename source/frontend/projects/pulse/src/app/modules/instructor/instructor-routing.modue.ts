import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { InstructorLayoutComponent } from '../../layouts/instructor-layout/instructor-layout.component';
import { ActiveSessionComponent } from './components/active-session/active-session.component';
import { ClassDetailsComponent } from './components/class-details/class-details.component';
import { ClassListComponent } from './components/class-list/class-list.component';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { ProfileComponent } from './components/profile/profile.component';
import { ReportParametersComponent } from './components/report-parameters/report-parameters.component';
import { ReportViewComponent } from './components/report-view/report-view.component';
import { SettingsComponent } from './components/settings/settings.component';

const routes: Routes = [
    {
        path: '',
        component: InstructorLayoutComponent,
        //pathMatch: 'full',
        children: [
            {
                path: '',
                component: DashboardComponent,
                children: [
                    {
                        path: '',
                        redirectTo: 'class',
                        pathMatch: 'full',
                    },
                    {
                        path: 'class',
                        component: ClassListComponent,
                        pathMatch: 'full',
                    },
                    {
                        path: 'class/add',
                        component: ClassDetailsComponent,
                    },
                    {
                        path: 'class/:id',
                        component: ClassDetailsComponent,
                    },
                    {
                        path: 'settings',
                        component: SettingsComponent,
                    },
                    {
                        path: 'profile',
                        component: ProfileComponent,
                    },
                    {
                        path: 'report-params',
                        component: ReportParametersComponent,
                    },
                    {
                        path: 'report',
                        component: ReportViewComponent,
                    },
                ],
            },
            {
                path: 'session',
                component: ActiveSessionComponent,
            },
        ],
    },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
})
export class InstructorRoutingModule {}
