const gulp = require('gulp');

gulp.task('zat.html:assets', () => {

    return gulp.src('dist/**/*.html')
        .pipe(gulp.dest('zat/assets'));
});
gulp.task('zat.html:watch', () => {
    return gulp.watch('dist/**/*.html', gulp.series('zat.html:assets'));
});

gulp.task('zat.html', gulp.series('zat.html:assets'));