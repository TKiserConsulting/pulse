import { Injector } from '@angular/core';

/**
 * Allows for retrieving singletons using `StaticInjector.get(MyService)` (whereas
 * `ReflectiveInjector.resolveAndCreate(MyService)` would create a new instance
 * of the service).
 */
// eslint-disable-next-line import/no-mutable-exports
export let StaticInjector: Injector;

/**
 * Helper to set the exported {@link StaticInjector}, needed as ES6 modules export
 * immutable bindings (see http://2ality.com/2015/07/es6-module-exports.html) for
 * which trying to make changes after using `import {StaticInjector}` would throw:
 * "TS2539: Cannot assign to 'StaticInjector' because it is not a variable".
 */
export function setStaticInjector(injector: Injector) {
    if (StaticInjector) {
        // Should not happen
        // eslint-disable-next-line no-console
        console.error('Programming error: StaticInjector was already set');
    } else {
        StaticInjector = injector;
    }
}
