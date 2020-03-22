dotnet build ..\source\VirtualDesktop.sln -c Release
dotnet pack ..\source\VirtualDesktop.sln -c Release --no-build -o %CD%
