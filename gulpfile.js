const path = require('path');

const gulp = require('gulp');
const { 
    clean: dotnet_clean,
    build: dotnet_build
} = require('gulp-dotnet-cli');
const shrun = require('gulp-run');

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


function getAutoRestCommand(config) {
    const plugin = path.resolve('.').replace(/\\/g, '\\\\');
    const metadata = path.resolve(config.metadata).replace(/\\/g, '\\\\');
    const output = path.resolve(metadata, '../generated').replace(/\\/g, '\\\\');
    const commands = ['autorest',
        '--terraform-provider', '--use="' + plugin + '"',
        '--input-file="' + config.input + '"',
        '--namespace="' + config.namespace + '"',
        '--terraform-provider.metadata-file="' + metadata + '"',
        '--clear-output-folder', '--can-clear-output-folder', '--output-folder="' + output + '"'];
    if (config.displayModel) {
        commands.push('--terraform-provider.display-model=' + config.displayModel);
    }
    return commands.join(' ');
}

gulp.task('run:hdinsight', () => {
    const config = require('./samples/hdinsight/autorest');
    return shrun(getAutoRestCommand(config)).exec();
});
