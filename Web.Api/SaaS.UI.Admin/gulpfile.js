/// <binding ProjectOpened='sass, sass:watch' />
"use strict";

var gulp = require("gulp"),
    concat = require("gulp-concat"),
    uglify = require("gulp-uglify"),
    sass = require("gulp-sass");

gulp.task('bundle', function (done) {

    var src = ['scripts/jquery-2.2.0.min.js',
               'scripts/jquery.cookie-1.4.1.min.js',
               'scripts/bootstrap.min.js',
               'scripts/respond.min.js',
               'scripts/metisMenu.min.js',
               'scripts/angular.js',

               'scripts/angular-ui/ui-bootstrap-tpls-2.5.0.min.js',
               'scripts/angular-notify.min.js'];

    gulp.src(src)
        .pipe(concat('bundle.js'))
        .pipe(uglify())
        .pipe(gulp.dest('js'));

    done();
});

//gulp.task('sass', function () {

//    return gulp.src('stylesheets/**/*.scss')
//        .pipe(sass({ outputStyle: 'compressed' }))
//        .pipe(gulp.dest('css'));
//});

//gulp.task('sass:watch', function () {
//    return gulp.watch('stylesheets/**/*.scss', ['sass']);
//});