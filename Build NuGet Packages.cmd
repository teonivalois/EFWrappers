@echo off
if not exist Binaries mkdir Binaries
if not exist Binaries\NuGet mkdir Binaries\NuGet
Util\nuget.exe pack -sym Source\EFProviderWrapperToolkit\EFProviderWrapperToolkit.csproj -o Binaries\NuGet
Util\nuget.exe pack -sym Source\EFTracingProvider\EFTracingProvider.csproj -o Binaries\NuGet
pause