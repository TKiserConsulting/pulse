import packageInfo from 'package.json';

declare const localStorage: Storage;

export function available(name: string) {
    return !(localStorage.getItem(`features.${name}`) === 'false');
}

export function hidden(name: string) {
    return localStorage.getItem(`features.${name}`) === 'true';
}

export const environment = {
    production: true,
    development: false,
    version: packageInfo.version,
    features: {
        authorization: true,
        paidFeatures: available('paid-features'),
    },
};
