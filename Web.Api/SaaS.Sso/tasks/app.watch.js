const gulp = require('gulp');

gulp.task('app.watch', gulp.parallel(
    'app.html:watch',
    'app.css:watch',
    'app.js:watch',
    'app.json:watch'));
