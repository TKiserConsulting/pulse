import { createAction, props, createReducer, on } from '@ngrx/store';
import { Identity, JwtAuthModel } from '../models/auth.models';

export const featureKey = 'auth';

export interface State {
    identity: Identity;
    jwt: JwtAuthModel | null;
}

export function createAnonymousIdentity(): Identity {
    return {
        userId: null,
        userName: 'anonymous',
        email: null,
        userRole: null,
        isAuthenticated: false,
    };
}

export const initialState: State = {
    identity: createAnonymousIdentity(),
    jwt: null,
};

export const setAuth = createAction(
    '[auth action] set auth',
    props<{ jwt: JwtAuthModel; identity: Identity }>()
);

export const setTokens = createAction(
    '[auth action] set tokens',
    props<{ jwt: JwtAuthModel }>()
);

export const clearAuth = createAction('[auth action] clear auth');

export const reducer = createReducer(
    initialState,
    on(setAuth, (state, { jwt, identity }) => ({ ...state, jwt, identity })),
    on(setTokens, (state, { jwt }) => ({ ...state, jwt })),
    on(clearAuth, (state) => {
        return { ...state, ...initialState };
    })
);

export const getAuth = (state: any) => state[featureKey] as State;
export const getAuthJwt = (state: any) => (state[featureKey] as State).jwt;
export const getAuthIdentity = (state: any) =>
    (state[featureKey] as State).identity;

export const isAuthenticated = (state: any) => {
    const p = getAuthIdentity(state);
    return p && p.isAuthenticated;
};
