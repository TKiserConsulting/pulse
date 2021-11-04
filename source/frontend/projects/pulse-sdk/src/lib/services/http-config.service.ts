import { HttpRequest } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class HttpConfigService {
    private callback: ((req: HttpRequest<any>) => HttpRequest<any>) | null =
        null;

    register(action: (req: HttpRequest<any>) => HttpRequest<any>) {
        this.callback = action;
    }

    clear() {
        this.callback = null;
    }

    apply(req: HttpRequest<any>): HttpRequest<any> {
        if (this.callback) {
            req = this.callback(req);
        }
        return req;
    }
}
