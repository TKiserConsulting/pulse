/* eslint-disable no-console */
import {
    Action,
    ActionReducer,
    ActionReducerMap,
    MetaReducer,
} from '@ngrx/store';
import { InjectionToken } from '@angular/core';
import { localStorageSync } from 'ngrx-store-localstorage';
import * as fromConfig from './config.reducer';
import * as fromAuth from './auth.reducer';

export interface State {
    [fromConfig.featureKey]: fromConfig.State;
    [fromAuth.featureKey]: fromAuth.State;
}

export const ROOT_REDUCERS = new InjectionToken<
    ActionReducerMap<State, Action>
>('Root reducers token', {
    factory: () => ({
        [fromConfig.featureKey]: fromConfig.reducer,
        [fromAuth.featureKey]: fromAuth.reducer,
    }),
});

// reload from local storage and sync reducers
export function localStorageSyncReducer(
    reducer: ActionReducer<any>
): ActionReducer<any> {
    return localStorageSync({
        keys: [fromAuth.featureKey],
        rehydrate: true,
        removeOnUndefined: true,
        storageKeySerializer: (key) =>
            `pulse.${window.location.hostname}.${key}`,
    })(reducer);
}
export const metaReducers: Array<MetaReducer<any, any>> = [
    localStorageSyncReducer,
];
