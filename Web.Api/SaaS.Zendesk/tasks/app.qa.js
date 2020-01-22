//const gulp = require('gulp');
//const karma = require('gulp-karma');

// var server = require('karma').Server;

// gulp.task('app.qa', function (done) {

//     new server({
//         configFile: __dirname + '/../karma.conf.js',
//         singleRun: false
//     }, done).start();
// });

// gulp.task('test', () => {

//     return gulp.src([
//         'node_modules/angular/angular.min.js',
//         'node_modules/angular-mocks/angular-mocks.js',
//         'dist/js/app.min.js',
//         'qa/**/*.js'
//     ])
//         .pipe(karma({
//             configFile: 'karma.conf.js',
//             action: 'run'
//         }))
//         .on('error', function (err) {
//             // Make sure failed tests cause gulp to exit non-zero
//             throw err;
//         });
// });