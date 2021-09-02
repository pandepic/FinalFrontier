rmdir /s /q "build-windows"
dotnet publish Client/Client.csproj -r win-x64 -c Debug --output build-debug-windows
del /s "NetCoreBeauty"
pause