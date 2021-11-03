import { Component } from '@angular/core';
import { environment } from '@env/environment';

@Component({
    selector: 'app-layout-anonymous',
    templateUrl: './anonymous-layout.component.html',
    styleUrls: ['./anonymous-layout.component.scss'],
})
export class AnonymousLayoutComponent {
    public version = environment.version;
}
