var gulp = require('gulp');
var del = require('del');

gulp.task('del-images', function () {
    return del([
        './wwwroot/images/**/*'
    ]);
});

gulp.task('copy-images', function () {
    return gulp.src('./src/images/*')
        .pipe(gulp.dest('./wwwroot/images/'));
});

gulp.task('del-lib', function () {
    return del([
        './wwwroot/lib/**/*'
    ]);
});

gulp.task('copy-lib', function (done) {
    gulp.src('./node_modules/bootstrap/dist/**/*')
        .pipe(gulp.dest('./wwwroot/lib/bootstrap/dist'));
    gulp.src('./node_modules/jquery/dist/**/*')
        .pipe(gulp.dest('./wwwroot/lib/jquery/dist'));
    gulp.src('./node_modules/jquery-validation/dist/**/*')
        .pipe(gulp.dest('./wwwroot/lib/jquery-validation/dist'));
    gulp.src('./node_modules/jquery-validation-unobtrusive/dist/**/*')
        .pipe(gulp.dest('./wwwroot/lib/jquery-validation-unobtrusive/dist'));
    gulp.src('./node_modules/popper.js/dist/**/*')
        .pipe(gulp.dest('./wwwroot/lib/popper.js/dist'));
    gulp.src('./node_modules/datatables.net/**/*')
        .pipe(gulp.dest('./wwwroot/lib/datatables.net'));
    gulp.src('./node_modules/datatables.net-bs4/**/*')
        .pipe(gulp.dest('./wwwroot/lib/datatables.net-bs4'));
    done();
});

gulp.task('default', gulp.series('del-images', 'copy-images', 'del-lib', 'copy-lib'));