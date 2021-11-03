import { Injectable } from '@angular/core';
import { SurveyOverrideDefinition } from '../models/survey-overrides.models';
import { SurveyDefinition, SurveyQuestion, SurveySelectOption } from '../models/surveys.models';

@Injectable({ providedIn: 'root' })
export class SurveysHelperService {
    public static findRootQuestions(definition: SurveyDefinition): SurveyQuestion[] {
        return definition.questions.filter((q) => {
            return !q.nextQuestion && !q.terminal && q.selectOptions?.length > 0;
        });
    }

    public static findPriceQuestions(definition: SurveyDefinition): SurveyQuestion[] {
        const filterPriceOptions = (o: SurveySelectOption) => {
            return !!o.labor || !!o.price;
        };

        return definition.questions
            .filter((q: SurveyQuestion) => {
                return (q.selectOptions || []).some(filterPriceOptions);
            })
            .map((q) => {
                return {
                    ...q,
                    ...{
                        selectOptions: q.selectOptions.filter(filterPriceOptions),
                    },
                };
            });
    }

    public static parseSurvey(surveyJson: string): SurveyDefinition {
        const obj = JSON.parse(surveyJson || '{}');
        const survey: SurveyDefinition = {
            ...obj,
        };
        return survey;
    }

    public static parseSurveyOverrides(surveyOverrideJson: string): SurveyOverrideDefinition {
        const obj = JSON.parse(surveyOverrideJson || '{}');
        return obj as SurveyOverrideDefinition;
    }

    public static applySurveyOverrides(
        survey: SurveyDefinition,
        overrides: SurveyOverrideDefinition
    ) {
        survey.constants?.forEach((cnst) => {
            const override = overrides.constants?.find((o) => o.name === cnst.name);
            if (override) {
                cnst.value = override.value;
            }
        });

        survey.questions?.forEach((question) => {
            const rootOverride = overrides.rootQuestions?.find((q) => q.id === question.id);
            if (rootOverride) {
                question.selectOptions = question.selectOptions?.filter((opt) => {
                    const optOverride = rootOverride.selectOptions?.find((o) => o.id === opt.id);
                    return !optOverride || optOverride.enabled;
                });
            }

            const priceOverride = overrides.priceQuestions?.find((q) => q.id === question.id);
            if (priceOverride) {
                question.selectOptions = question.selectOptions?.filter((opt) => {
                    const optOverride = priceOverride.selectOptions?.find((o) => o.id === opt.id);
                    const optEnabled =
                        optOverride && (optOverride.enabled || optOverride.enabled == null);
                    if (optEnabled) {
                        opt.price = optOverride.price != null ? optOverride.price : opt.price;
                        opt.labor = optOverride.labor != null ? optOverride.labor : opt.labor;
                    }
                    return !optOverride || optEnabled;
                });
            }
        });
    }
}
