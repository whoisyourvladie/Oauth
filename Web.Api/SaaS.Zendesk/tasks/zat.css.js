const gulp = require('gulp');

gulp.task('zat.css:assets', () => {

    return gulp.src('dist/css/*.css')
        .pipe(gulp.dest('zat/assets/css'));
});
gulp.task('zat.css:watch', () => {
    return gulp.watch('dist/css/*.css', gulp.series('zat.css:assets'));
});

gulp.task('zat.css', gulp.series('zat.css:assets'));