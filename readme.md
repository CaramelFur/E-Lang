# E-Lang

> E-Lang is just a random language I created for a school project

## Building E-Lang

### Run interpreter

E-Lang is built with .NET in C# so to build and run the program simply type:

```sh
dotnet run
```

This will give you an interpreter screen to test everything.

### Run file

You can also append a file to executee that file with E-Lang code:

```sh
dotnet run ./examples/fibonacci.elg
```

### Build to release

If you wish to fully compile the program you can run this command:

```sh
dotnet build -c Release
```

And you can then find the built program in `./bin/Release/netcoreapp*/E-Lang.dll`
