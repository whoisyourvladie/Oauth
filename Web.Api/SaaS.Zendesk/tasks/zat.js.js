const gulp = require('gulp');

gulp.task('zat.js:assets', () => {

    return gulp.src('dist/js/*.js')
        .pipe(gulp.dest('zat/assets/js'));
});
gulp.task('zat.js:watch', () => {
    return gulp.watch('dist/js/*.js', gulp.series('zat.js:assets'));
});

gulp.task('zat.js', gulp.series('zat.js:assets'));