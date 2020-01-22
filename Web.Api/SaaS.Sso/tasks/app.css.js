const gulp = require('gulp');
const concat = require('gulp-concat');
const cleanCSS = require('gulp-clean-css');

gulp.task('app.css:app', () => {

    return gulp.src(['src/css/**/*.css'])
        .pipe(concat('app.min.css'))
        .pipe(cleanCSS())
        .pipe(gulp.dest('dist/css'));
});
gulp.task('app.css:vendor', () => {

    return gulp.src([
        'node_modules/bootstrap/dist/css/bootstrap.min.css'
    ])
        .pipe(concat('vendor.min.css'))
        .pipe(gulp.dest('dist/css'))
});
gulp.task('app.css:watch', () => {
    return gulp.watch('src/css/**/*.css', gulp.series('app.css:app'));
});
gulp.task('app.css', gulp.parallel('app.css:app', 'app.css:vendor'));