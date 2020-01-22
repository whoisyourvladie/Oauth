const gulp = require('gulp');
const htmlmin = require('gulp-htmlmin');

gulp.task('app.html:ui', () => {

    return gulp.src(['src/index.html', 'src/ui/**/*.html'])
        .pipe(htmlmin({
            collapseWhitespace: true,
            removeComments: true,
            sortAttributes: true,
            sortClassName: true
        }))
        .pipe(gulp.dest('dist'))
});
gulp.task('app.html:watch', () => {
    return gulp.watch(['src/index.html', 'src/ui/**/*.html'], gulp.series('app.html:ui'));
});
gulp.task('app.html', gulp.series('app.html:ui'));