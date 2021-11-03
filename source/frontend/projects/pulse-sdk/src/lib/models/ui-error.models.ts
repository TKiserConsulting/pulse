export interface ErrorDescriptionModel {
    code: string;
    reason: string;
    extendedData?: any;
}

export interface ErrorModel extends Error {
    code: string;
    title: string;
    errors: { [propertyName: string]: ErrorDescriptionModel[] };
    processed: boolean;
}

export interface ClientErrorModel {
    scope: string;
    propertyName?: string;
    code: string;
    message: string;
    extendedData?: any;
}
