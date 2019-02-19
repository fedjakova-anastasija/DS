@echo off

start /d Frontend dotnet Frontend.dll
start /d Backend dotnet Backend.dll

start "" /wait "http://127.0.0.1:5001"