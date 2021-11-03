export interface ErrorHandlingRule {
    code?: string;
    propertyName?: string;
    localizationKey?: string;
    toast?: boolean;
    validation?: boolean;
    summary?: boolean;
    orderIndex?: number;
}
