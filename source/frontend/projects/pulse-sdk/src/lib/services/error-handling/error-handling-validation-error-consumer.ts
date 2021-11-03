import { ClientErrorModel } from '../../models/ui-error.models';

export interface ErrorHandlingValidationErrorConsumer {
    setValidationErrors: (m: ClientErrorModel[]) => void;
}
