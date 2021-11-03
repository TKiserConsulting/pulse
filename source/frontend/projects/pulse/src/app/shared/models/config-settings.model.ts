export interface IConfigSettings {
    version: string;
    defaultUiLanguage: string;
    uiLanguages: string[];
    settings: { [key: string]: string };
}

export class ConfigSettings implements IConfigSettings {
    public version: string = '';

    public defaultUiLanguage: string = '';

    public uiLanguages: string[] = [];

    public settings: {} = {};
}
