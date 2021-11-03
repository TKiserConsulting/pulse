export interface ApiFieldError {
    field: string;
    code: string;
    message: string;
}

export interface ApiErrorDescription {
    traceId?: string;
    code: string;
    title?: string;
    message?: string;
    errors?: ApiFieldError[];
}

export class ApiError extends Error implements ApiErrorDescription {
    public traceId?: string;

    public code!: string;

    public title!: string;

    public errors!: ApiFieldError[];

    public innerError?: any;
}
