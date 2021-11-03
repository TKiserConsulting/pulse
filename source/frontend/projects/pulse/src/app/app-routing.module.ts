import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { UserRole } from './api/models';
import { AnonymousLayoutComponent } from './layouts/anonymous-layout/anonymous-layout.component';
import { ErrorComponent } from './modules/main/components/errors/error.component';
import { Error403Component } from './modules/main/components/errors/error403.component';
import { PageNotFoundComponent } from './modules/main/components/errors/page-not-found.component';
import { RegisterAccountComponent } from './modules/main/components/register-account/register-account.component';
import { SigninComponent } from './modules/main/components/signin/signin.component';
import { StudentSigninComponent } from './modules/main/components/student-signin/student-signin.component';
import { AuthGuard } from './shared/guards/auth.guard';

const routes: Routes = [
    {
        path: '',
        redirectTo: '/instructor/class',
        pathMatch: 'full',
    },
    {
        path: 'student',
        canActivate: [AuthGuard],
        data: {
            userRole: UserRole.Student,
            accessDeniedRoute: ['student', 'join'],
            signinRoute: ['student', 'join'],
        },
        loadChildren: () =>
            import('./modules/student/student.module').then(
                (m) => m.StudentModule
            ),
    },
    {
        path: 'student',
        component: AnonymousLayoutComponent,
        children: [
            {
                path: '',
                redirectTo: '/student/session',
                pathMatch: 'full',
            },
            { path: 'join', component: StudentSigninComponent },
            {
                path: 'signout',
                component: StudentSigninComponent,
                data: { logout: true },
            },
            { path: 'error403', component: Error403Component },
            { path: 'error404', component: PageNotFoundComponent },
            { path: 'error', component: ErrorComponent },
            { path: '**', component: PageNotFoundComponent },
        ],
    },
    {
        path: 'instructor',
        canActivate: [AuthGuard],
        data: {
            userRole: UserRole.Instructor,
            accessDeniedRoute: ['student', 'session'],
        },
        loadChildren: () =>
            import('./modules/instructor/instructor.module').then(
                (m) => m.InstructorModule
            ),
    },
    {
        path: '',
        component: AnonymousLayoutComponent,
        children: [
            { path: 'signin', component: SigninComponent },
            {
                path: 'signout',
                component: SigninComponent,
                data: { logout: true },
            },
            { path: 'register', component: RegisterAccountComponent },
            { path: 'error403', component: Error403Component },
            { path: 'error404', component: PageNotFoundComponent },
            { path: 'error', component: ErrorComponent },
            { path: '**', component: PageNotFoundComponent },
        ],
    },
];

@NgModule({
    imports: [RouterModule.forRoot(routes)],
    exports: [RouterModule],
})
export class AppRoutingModule {}
