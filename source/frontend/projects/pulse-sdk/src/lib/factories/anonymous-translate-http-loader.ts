import { HttpClient } from '@angular/common/http';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';

export class AnonymousTranslateHttpLoader extends TranslateHttpLoader {
    private httpClient: HttpClient;

    constructor(http: HttpClient, prefix?: string, suffix?: string) {
        super(http, prefix, suffix);
        this.httpClient = http;
    }

    public getTranslation(lang: string) {
        return this.httpClient.get(`${this.prefix}${lang}${this.suffix}`, {
            headers: { Anonymous: 'true' },
        });
    }
}
