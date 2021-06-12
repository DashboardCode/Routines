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

    //gulp.src('./node_modules/@babel/polyfill/dist/polyfill.js')
    //    .pipe(gulp.dest('./wwwroot/js'));

    gulp.src('./node_modules/bootstrap/dist/js/bootstrap.js')
        .pipe(gulp.dest('./wwwroot/js/'));
    gulp.src('./node_modules/bootstrap/dist/css/bootstrap.css')
        .pipe(gulp.dest('./wwwroot/css/'));

    gulp.src('./node_modules/jquery/dist/jquery.js')
        .pipe(gulp.dest('./wwwroot/js'));

    gulp.src('./node_modules/jquery-validation/dist/jquery.validate.js')
        .pipe(gulp.dest('./wwwroot/js'));

    gulp.src('./node_modules/jquery-validation-unobtrusive/dist/jquery.validate.unobtrusive.js')
        .pipe(gulp.dest('./wwwroot/js'));

    gulp.src('./node_modules/@popperjs/core/dist/umd/popper.js')
        .pipe(gulp.dest('./wwwroot/js'));

    gulp.src('./node_modules/datatables.net/js/jquery.dataTables.js')
        .pipe(gulp.dest('./wwwroot/js'));

    gulp.src('./node_modules/datatables.net-bs5/js/dataTables.bootstrap5.js')
        .pipe(gulp.dest('./wwwroot/js'));

    gulp.src('./node_modules/@dashboardcode/bsmultiselect/dist/css/BsMultiSelect.css')
        .pipe(gulp.dest('./wwwroot/css'));

    gulp.src('./node_modules/@dashboardcode/bsmultiselect/dist/js/BsMultiSelect.js')
        .pipe(gulp.dest('./wwwroot/js'));

    //gulp.src('./node_modules/material-design-icons/iconfont/*.woff2')
    //    .pipe(gulp.dest('./wwwroot/fonts'));
    //gulp.src('./node_modules/material-design-icons/iconfont/*.woff')
    //    .pipe(gulp.dest('./wwwroot/fonts'));
    //gulp.src('./node_modules/material-design-icons/iconfont/*.ttf')
    //    .pipe(gulp.dest('./wwwroot/fonts'));

    gulp.src('./node_modules/material-design-icons-iconfont/dist/fonts/*.woff2')
        .pipe(gulp.dest('./wwwroot/fonts'));
    gulp.src('./node_modules/material-design-icons-iconfont/dist/fonts/*.woff')
        .pipe(gulp.dest('./wwwroot/fonts'));
    gulp.src('./node_modules/material-design-icons-iconfont/dist/fonts/*.ttf')
        .pipe(gulp.dest('./wwwroot/fonts'));

    done();
});

gulp.task('default', gulp.series('del-images', 'copy-images', 'del-lib', 'copy-lib'));