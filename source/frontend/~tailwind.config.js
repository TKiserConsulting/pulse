module.exports = {
    future: {
        removeDeprecatedGapUtilities: true,
        purgeLayersByDefault: true,
    },
    purge: [
        './projects/pulse/src/**/*.html',
        './projects/pulse/src/**/*.ts',
    ],
    theme: {
        extend: {},
    },
    variants: {},
    plugins: [],
};
