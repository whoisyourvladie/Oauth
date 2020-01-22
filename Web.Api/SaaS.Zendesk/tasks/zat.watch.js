const gulp = require('gulp');

gulp.task('zat.watch', gulp.parallel(
    'zat.css:watch',
    'zat.html:watch',
    'zat.js:watch'));
