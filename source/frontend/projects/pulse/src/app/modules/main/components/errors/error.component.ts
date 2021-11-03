import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
    templateUrl: './error.component.html',
    styleUrls: ['./error.component.scss'],
})
export class ErrorComponent implements OnInit {
    public code: string | null = null;

    public message: string | null = null;

    constructor(private route: ActivatedRoute) {}

    public ngOnInit() {
        this.code = this.route.snapshot.queryParamMap.get('code');
        this.message = this.route.snapshot.queryParamMap.get('message');
    }
}
