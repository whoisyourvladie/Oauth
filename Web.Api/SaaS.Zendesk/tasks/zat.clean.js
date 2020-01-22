const del = require('del');
const gulp = require('gulp');

gulp.task('zat.clean', () => {

    return del(['zat/assets']);
});