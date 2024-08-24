rmdir /s /q "build-server"
dotnet publish Server/Server.csproj -r win-x64 -c Release --output build-server
del /s "NetCoreBeauty"
pause