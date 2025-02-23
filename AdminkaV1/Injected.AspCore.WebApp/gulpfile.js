import { deleteSync } from 'del';
import gulp from 'gulp';
const { src, dest, series } = gulp;

function delImages(done) {
    deleteSync([
        './wwwroot/images/**/*'
    ]);
    done();
}

function delLib(done) {
    deleteSync([
        './wwwroot/lib/**/*'
    ]);
    done();
};


function copyImages(done) {
    gulp.src('./src/images/*')
        .pipe(gulp.dest('./wwwroot/images/'));
    done();
};

function copyCss(done) {
    gulp.src('./node_modules/bootstrap/dist/css/bootstrap.css')
        .pipe(gulp.dest('./wwwroot/css/'));
    gulp.src('./node_modules/@dashboardcode/bsmultiselect/dist/css/BsMultiSelect.css')
        .pipe(gulp.dest('./wwwroot/css'));
    done();
};

function copyJs(done) {
    //gulp.src('./node_modules/@babel/polyfill/dist/polyfill.js')
    //    .pipe(gulp.dest('./wwwroot/js'));

    gulp.src('./node_modules/bootstrap/dist/js/bootstrap.js')
        .pipe(gulp.dest('./wwwroot/js/'));

    gulp.src('./node_modules/jquery/dist/jquery.js')
        .pipe(gulp.dest('./wwwroot/js'));

    gulp.src('./node_modules/jquery-validation/dist/jquery.validate.js')
        .pipe(gulp.dest('./wwwroot/js'));

    gulp.src('./node_modules/jquery-validation-unobtrusive/dist/jquery.validate.unobtrusive.js')
        .pipe(gulp.dest('./wwwroot/js'));

    gulp.src('./node_modules/@popperjs/core/dist/umd/popper.js')
        .pipe(gulp.dest('./wwwroot/js'));

    gulp.src('./node_modules/datatables.net/js/dataTables.js')
        .pipe(gulp.dest('./wwwroot/js'));

    gulp.src('./node_modules/datatables.net-bs5/js/dataTables.bootstrap5.js')
        .pipe(gulp.dest('./wwwroot/js'));

    gulp.src('./node_modules/@dashboardcode/bsmultiselect/dist/js/BsMultiSelect.js')
        .pipe(gulp.dest('./wwwroot/js'));

    done();
}

function copyFonts(done) {
    gulp.src('./node_modules/material-design-icons-iconfont/dist/fonts/*.woff2')
        .pipe(gulp.dest('./wwwroot/fonts'));
    gulp.src('./node_modules/material-design-icons-iconfont/dist/fonts/*.woff')
        .pipe(gulp.dest('./wwwroot/fonts'));
    gulp.src('./node_modules/material-design-icons-iconfont/dist/fonts/*.ttf')
        .pipe(gulp.dest('./wwwroot/fonts'));
    done();
}

const build = gulp.series(gulp.parallel(delImages, delLib), gulp.parallel(copyImages, copyCss, copyJs, copyFonts));
export default build;
