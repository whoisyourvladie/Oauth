const gulp = require('gulp');

gulp.task('app.watch', gulp.parallel(
    'app.css:watch',
    'app.html:watch',
    'app.js:watch'));
