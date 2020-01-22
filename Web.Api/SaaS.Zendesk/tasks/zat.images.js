const gulp = require('gulp');

gulp.task('zat.images', () => {

    return gulp.src('zat/images/**/*.*')
        .pipe(gulp.dest('zat/assets'));
});