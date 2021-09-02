rmdir /s /q "build-debug-server"
dotnet publish Server/Server.csproj -r win-x64 -c Debug --output build-debug-server
del /s "NetCoreBeauty"
pause