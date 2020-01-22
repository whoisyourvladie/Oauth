const gulp = require('gulp');
var exec = require('child_process').exec;

gulp.task('zat.server', () => {

    return exec('zat server --path zat');
});

gulp.task('zat.validate', () => {

    return exec('zat validate --path zat');
});

gulp.task('zat.package', () => {

    return exec('zat package --path zat');
});