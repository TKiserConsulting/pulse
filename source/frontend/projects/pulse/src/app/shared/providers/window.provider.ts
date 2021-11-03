import { Injectable } from '@angular/core';

export interface ICustomWindow extends Window {}

function getWindow(): any {
    // eslint-disable-next-line no-undef
    return window;
}

@Injectable()
export class WindowRefService {
    get nativeWindow(): ICustomWindow {
        return getWindow();
    }
}
