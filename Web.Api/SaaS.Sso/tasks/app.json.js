const gulp = require('gulp');

gulp.task('app.json:i18n', () => {

    return gulp.src('src/i18n/**/*.json')
        .pipe(gulp.dest('dist/i18n'))
});
gulp.task('app.json:watch', () => {
    return gulp.watch('src/i18n/**/*.json', gulp.series('app.json:i18n'));
});
gulp.task('app.json', gulp.parallel('app.json:i18n'));