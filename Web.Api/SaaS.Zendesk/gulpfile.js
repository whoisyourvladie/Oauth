const gulp = require('gulp');

require('fs').readdirSync('./tasks/').forEach(function (task) {
  require('./tasks/' + task);
});

gulp.task('app', gulp.series('app.clean', gulp.parallel('app.html', 'app.css', 'app.js', 'app.font')));
gulp.task('zat', gulp.series('zat.clean', gulp.parallel('zat.html', 'zat.css', 'zat.js', 'zat.font', 'zat.images')));

// gulp.task('qa', gulp.series('release'));

gulp.task('build', gulp.series('app', 'zat'));
gulp.task('package', gulp.series('build', 'zat.package'));
gulp.task('default', gulp.series('build', gulp.parallel('app.watch', 'zat.watch', 'zat.server')));


// nfotenyuk 28.03.2019

// to start in Windows
gulp.task('build.windows', gulp.series('app'));
gulp.task('start.windows', gulp.series('build.windows', gulp.parallel('app.watch')));
/*gulp.task('package.windows', gulp.series('build.windows'));*/

// to start in UBUNTU
gulp.task('start.ubuntu', gulp.series('zat', gulp.parallel('zat.watch', 'zat.server'))); //run this task in bash (UBUNTU)