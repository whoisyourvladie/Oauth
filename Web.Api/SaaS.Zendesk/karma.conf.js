module.exports = function (config) {

    config.set({
        browsers: ['Chrome'],
        frameworks: ['jasmine'],
        plugins: [
            'karma-chrome-launcher',
            'karma-jasmine',
            'karma-coverage'
        ],
        files: [
            'dist/js/vendor.min.js',
            'dist/js/app.min.js',
            
            'node_modules/angular-mocks/angular-mocks.js',
            'qa/**/*.js'
        ],
        logLevel: config.LOG_INFO,
        reporters: ['progress', 'coverage'],
        colors: true,
        coverageReporter: {
            type: 'html',
            dir: 'coverage'
        }
    });
};