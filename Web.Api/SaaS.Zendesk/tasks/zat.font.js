const gulp = require('gulp');

gulp.task('zat.font:webfonts', () => {

    return gulp.src('dist/webfonts/**')
        .pipe(gulp.dest('zat/assets/webfonts'));
});

gulp.task('zat.font', gulp.series('zat.font:webfonts'));