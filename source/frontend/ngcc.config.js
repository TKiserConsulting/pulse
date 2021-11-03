module.exports = {
    packages: {
        // https://github.com/angular/angular/issues/35615
        '@pulse/sdk': {
            entryPoints: {
                './dist/pulse-sdk': {
                    ignore: true,
                },
            },
        },
    },
};
