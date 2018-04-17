# Development Document

This file is the development document for `autorest.terraform` extension.

## Engineering Lifecycle

Use `git` command to clone this repository. And after that, make sure you have installed the latest [`Node JS` and `npm`](https://nodejs.org/). And run the following command to fulfill all dependencies of this project.

```shell
$ npm install
```

### Build

#### Release

The `build` task uses the "Release" configuration to build the project.

```shell
$ npm run build
```

The command creates binary files in `bin/netcoreapp2.0` directory.

#### Debug

You can also build it with "Debug" configuration by running this command.

```shell
$ npm run build:debug
```

The command also generates binary files in `bin/netcoreapp2.0` directory.

> *NOTICE* The `build` task of both `Debug` configuration and `Release` configuration creates binaries to the same directory.

### Clean

The `clean` task clears all intermediate files as well as all the binary files.

```shell
$ npm run clean
```

### Package

The `package` task generates a ready-to-publish *.tgz file which includes all the binaries. This task also forces to rebuild the whole project using `Release` configuration.

```shell
$ npm run package
```

## Working with IDE

### Visual Studio

This project supports Visual Studio 2017. By opening `AutoRest.Terraform.sln` solution file in Visual Studio, you are able to do all the building tasks in it (for example, clicking menu item "`Build` → `Clean AutoRest.Terraform`" is equivalent to "`npm run clean`").
