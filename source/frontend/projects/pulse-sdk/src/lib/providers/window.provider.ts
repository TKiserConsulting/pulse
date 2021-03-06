import { InjectionToken, FactoryProvider } from '@angular/core';

export const WINDOW = new InjectionToken<Window>('window');

const windowProvider: FactoryProvider = {
    provide: WINDOW,
    // eslint-disable-next-line no-undef
    useFactory: () => window,
};

export const WINDOW_PROVIDER = [windowProvider];
