module.exports = {
    extends: ['stylelint-config-recommended'],
    rules: {
        'at-rule-no-unknown': [
            true,
            {
                ignoreAtRules: [
                    'tailwind',
                    'apply',
                    'variants',
                    'responsive',
                    'screen',
                    'include',
                    'mixin',
                ],
            },
        ],
        'selector-pseudo-element-no-unknown': [
            true,
            {
                ignorePseudoElements: ['ng-deep'],
            },
        ],
        'declaration-block-trailing-semicolon': null,
        'no-descending-specificity': null,
        'no-duplicate-selectors': null,
        'selector-type-no-unknown': null,
    },
};
