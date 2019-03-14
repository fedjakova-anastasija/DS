@echo off

start /d Frontend dotnet Frontend.dll
start /d Backend dotnet Backend.dll
start /d TextListener dotnet TextListener.dll
start /d TextRankCalc dotnet TextRankCalc.dll

start "" /wait "http://127.0.0.1:5001"

set file=%CD%\config\config.txt
for /f "tokens=1,2 delims=:" %%a in (%file%) do (
for /l %%i in (1, 1, %%b) do start /d %%a dotnet %%a.dll
)
