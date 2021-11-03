import { ErrorModel } from '../../models/ui-error.models';

export interface ErrorHandlingManager {
    scope(name: string): ErrorHandlingManager;
    populate(err: ErrorModel): Promise<void>;
}
