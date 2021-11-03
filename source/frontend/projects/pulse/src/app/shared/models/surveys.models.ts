export class SurveyDefinition {
    public name: string;

    public constants: SurveyConstant[];

    public formulas: { [name: string]: string };

    public questions: SurveyQuestion[];
}

export class SurveyConstant {
    public name: string;

    public label: string;

    public value: number;
}

export class SurveyQuestion {
    // question unique identifier
    public id: string;

    // name of the variable that is linked to this question
    public variable: string;

    // description of the question
    public title?: string;

    // the lable that shows in user interface along with the input control
    public label?: string;

    // type of the control
    public type: SurveyVariableType;

    // how much vertical lines for hext input (allows define multiline inputs)
    public textLines?: number;

    // max alloved length of the text value
    public textMaxLength?: number;

    // min value for numeric type
    public numberMinValue?: number;

    // max value for numeric type
    public numberMaxValue?: number;

    // regular expression for input validation
    public inputMask?: string;

    // list of available options for variable of type 'select'
    public selectOptions?: SurveySelectOption[];

    // link to the next question
    public nextQuestion?: string;

    // whether question is the last question in survey
    public terminal: boolean = false;

    // reference to the formula containing in the property Formulas of the class SurveyDefinition
    public formulaName?: string;
}

export enum SurveyVariableType {
    Select = 'select',
    Number = 'number',
    Text = 'text',
}

export class SurveySelectOption {
    // unique identifier for select option, allows reffer the option to turn on/off flow branches
    public id?: string;

    // option value
    public value: number | string;

    // label displayd in user interface for the option
    public label?: string;

    // large description for rich select
    public description?: string;

    // image for rich select. Two formats are possile:
    // - http://http://domain.co/path/to/image.jpg
    // or
    // - data:image/png;base64,iVBORw0KGgoAAA...
    public img?: string;

    // price value associated with the option
    public price?: number;

    // labor value associated with the option
    public labor?: number;

    // link to the next question when user selects this option
    public nextQuestion?: string;
}

export class SurveyAnswer {
    public questionId: string;

    public value: number | string;

    public selectOptionId?: string;
}
