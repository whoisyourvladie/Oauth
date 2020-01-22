const gulp = require('gulp');

require('fs').readdirSync('./tasks/').forEach(function (task) {
  require('./tasks/' + task);
});

gulp.task('app', gulp.series('app.clean', gulp.parallel('app.html', 'app.css', 'app.js', 'app.json')));

gulp.task('build', gulp.series('app'));
gulp.task('default', gulp.series('build', gulp.parallel('app.watch', 'app.serve')));