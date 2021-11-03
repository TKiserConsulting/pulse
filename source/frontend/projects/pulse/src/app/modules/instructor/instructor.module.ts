import { NgModule } from '@angular/core';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { HeaderBarComponent } from './components/header-bar/header-bar.component';
import { InstructorLayoutComponent } from '../../layouts/instructor-layout/instructor-layout.component';
import { ProfileComponent } from './components/profile/profile.component';
import { SharedModule } from '@app/shared/modules/shared.module';
import { FormControlsModule } from '@app/shared/modules/form-controls.module';
import { InstructorRoutingModule } from './instructor-routing.modue';
import { ClassListComponent } from './components/class-list/class-list.component';
import { ClassDetailsComponent } from './components/class-details/class-details.component';
import { ActiveSessionComponent } from './components/active-session/active-session.component';
import { QuestionListComponent } from './components/question-list/question-list.component';
import { SettingsComponent } from './components/settings/settings.component';
import { ProfileImagePipe } from './pipes/profile-image.pipe';
import { StartSessionComponent } from './components/start-session/start-session.component';
import { ReportParametersComponent } from './components/report-parameters/report-parameters.component';
import { ReportViewComponent } from './components/report-view/report-view.component';
import { ExternalControlsModule } from './external-controls.module';

@NgModule({
    imports: [
        InstructorRoutingModule,
        SharedModule,
        FormControlsModule,
        ExternalControlsModule,
    ],
    declarations: [
        InstructorLayoutComponent,
        DashboardComponent,
        HeaderBarComponent,
        ProfileComponent,
        ClassListComponent,
        ClassDetailsComponent,
        ActiveSessionComponent,
        QuestionListComponent,
        SettingsComponent,
        ProfileImagePipe,
        StartSessionComponent,
        ReportParametersComponent,
        ReportViewComponent,
    ],
})
export class InstructorModule {}
