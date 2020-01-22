const del = require('del');
const gulp = require('gulp');

gulp.task('app.clean', () => {

    return del(['dist']);
});