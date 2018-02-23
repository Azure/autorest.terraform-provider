const gulp = require('gulp');
const { 
    clean: dotnet_clean,
    build: dotnet_build
} = require('gulp-dotnet-cli');

gulp.task('clean', () => {
    return gulp.src('*.sln', { read: false })
        .pipe(dotnet_clean());
});

gulp.task('build:debug', () => {
    return gulp.src('*.sln', { read: false })
        .pipe(dotnet_build({ configuration: 'Debug' }));
});

gulp.task('build', ['clean'], () => {
    return gulp.src('*.sln', { read: false })
        .pipe(dotnet_build({ configuration: 'Release' }));
});
