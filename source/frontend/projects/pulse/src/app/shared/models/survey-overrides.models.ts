export interface SurveyOverrideDefinition {
    constants: SurveyConstantOverride[];

    rootQuestions: SurveyQuestionOverride[];

    priceQuestions: SurveyQuestionOverride[];
}

export interface SurveyConstantOverride {
    name: string;

    value: number;
}

export interface SurveyQuestionOverride {
    id: string;

    selectOptions: SurveyOptionOverride[];
}

export interface SurveyOptionOverride {
    id: string;

    price?: number;

    labor?: number;

    enabled: boolean;
}
