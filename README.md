# OpenBOR Extractor and Builder

# Building
install [dotnet 8](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
build with `dotnet build`

# Usage
```
#Extract
#output to output folder
./bin/Debug/net8.0/OpenBORExtract x /path/to/file.pak
#optionally specify output directory
./bin/Debug/net8.0/OpenBORExtract x /path/to/file.pak /path/to/output

#Build Pak File
./bin/Debug/net8.0/OpenBORExtract b /path/to/directory 
# or specify to specific file
./bin/Debug/net8.0/OpenBORExtract b /path/to/directory /path/to/output.pak
```
