const gulp = require('gulp');

gulp.task('app.font:font-awesome-5', () => {

    return gulp.src(['node_modules/font-awesome-5-css/webfonts/**/*.*'])
        .pipe(gulp.dest('dist/webfonts'))
});
gulp.task('app.font', gulp.parallel('app.font:font-awesome-5'));