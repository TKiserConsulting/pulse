import { ClientErrorModel } from '../../models/ui-error.models';
import { ErrorHandlingRule } from './error-handling-rule';

export class ErrorHandlingErrorModel implements ClientErrorModel {
    public localizationKey: string | null = null;

    public rule: ErrorHandlingRule | null = null;

    public constructor(
        public scope: string,
        public code: string,
        public message: string = '',
        public propertyName?: string,
        public extendedData?: any
    ) {}
}
