import { createAction, props, createReducer, on } from '@ngrx/store';
import {
    ConfigSettings,
    IConfigSettings,
} from '../models/config-settings.model';

export const featureKey = 'config';

export interface State extends IConfigSettings {}

export const initialState: State = {
    defaultUiLanguage: 'en',
    uiLanguages: ['en'],
    version: '0.0.0',
    settings: {},
};

export const changeConfig = createAction(
    '[config] change config settings',
    props<{ config: ConfigSettings }>()
);

export const updateSettings = createAction(
    '[config] update portal settings',
    props<{ settings: { [key: string]: string } }>()
);

export const reducer = createReducer(
    initialState,
    on(changeConfig, (state, { config }) => ({ ...state, ...config })),
    on(updateSettings, (state, { settings }) => ({ ...state, settings }))
);

export const getConfig = (state: any): State => state[featureKey];
