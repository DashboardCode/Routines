var gulp = require('gulp');

gulp.task('copy-images', function () {
    gulp.src('./src/images/*')
        .pipe(gulp.dest('./wwwroot/images/'));
});
