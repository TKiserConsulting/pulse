import { NgModule } from '@angular/core';
import { StudentRoutingModule } from './student-routing.module';
import { StudentLayoutComponent } from '../../layouts/student-layout/student-layout.component';
import { StudentQuestionComponent } from './components/student-question/student-question.component';
import { StudentGridComponent } from './components/student-grid/student-grid.component';
import { FormControlsModule } from '@app/shared/modules/form-controls.module';
import { SharedModule } from '@app/shared/modules/shared.module';

@NgModule({
    declarations: [
        StudentLayoutComponent,
        StudentQuestionComponent,
        StudentGridComponent,
    ],
    imports: [StudentRoutingModule, SharedModule, FormControlsModule],
})
export class StudentModule {}
