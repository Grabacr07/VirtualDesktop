dotnet build ..\src\VirtualDesktop.sln -c Release
dotnet pack ..\src\VirtualDesktop.sln -c Release --no-build -o %CD%
