@echo off

start /d Frontend dotnet Frontend.dll
start /d Backend dotnet Backend.dll
start /d TextListener dotnet TextListener.dll
start /d TextRankCalc dotnet TextRankCalc.dll
rem start /d VowelConsCounter dotnet VowelConsCounter.dll
rem start /d VowelConsRater dotnet VowelConsRater.dll

start "" /wait "http://127.0.0.1:5001"

rem cd ..
rem set file=%CD%\config\number.txt
rem for /f "usebackq delims=" %%a in (%file%) do 
rem (
rem 	for /l %%i in (1, 1, %%b) do (start /d %%a dotnet %%a.dll)
rem )

set file=%CD%\config\config.txt
for /f "tokens=1,2 delims=:" %%a in (%file%) do (
for /l %%i in (1, 1, %%b) do start /d %%a dotnet %%a.dll
)
