const gulp = require('gulp');
const change = require('gulp-change');
const htmlmin = require('gulp-htmlmin');

gulp.task('app.html:ui', () => {

    return gulp.src(['src/index.html', 'src/ui/**/*.html'])
        .pipe(htmlmin({
            collapseWhitespace: true,
            removeComments: true,
            sortAttributes: true,
            sortClassName: true
        }))
        .pipe(change(function (content) {

            var relativePath = this.file.path.substr(this.file.base.length + 1);

            if (relativePath === 'index.html') {

                content = content.replace(new RegExp('.min.js', 'g'), `.min.js?version=${Date.now()}`);
                content = content.replace(new RegExp('.min.css', 'g'), `.min.css?version=${Date.now()}`);

                return content;
            }

            return content;
        }))

        .pipe(gulp.dest('dist'));
});
gulp.task('app.html:watch', () => {
    return gulp.watch(['src/index.html', 'src/ui/**/*.html'], gulp.series('app.html:ui'));
});
gulp.task('app.html', gulp.series('app.html:ui'));