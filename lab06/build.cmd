@echo off
set newVersion=%~1

if "%newVersion%" == "" (
	echo Wrong number of arguments
	echo Usage: build version
	exit /b 0
)

if exist %newVersion% rd %newVersion% /S /Q
md %newVersion%
md %newVersion%\config
echo VowelConsCounter: 1 >> %newVersion%/config/config.txt 
echo VowelConsRater: 1 >> %newVersion%/config/config.txt 

cd src/Frontend/
dotnet publish -c Release -o ../../%newVersion%/Frontend
if %ERRORLEVEL% NEQ 0 (
    echo Unsuccessful build
	exit /b 0
)

cd ../../src/Backend/
dotnet publish -c Release -o ../../%newVersion%/Backend
if %ERRORLEVEL% NEQ 0 (
    echo Unsuccessful build
	exit /b 0
)

cd ../../src/TextListener/
dotnet publish -c Release -o ../../%newVersion%/TextListener
if %ERRORLEVEL% NEQ 0 (
    echo Unsuccessful build
	exit /b 0
)

cd ../../src/TextRankCalc/
dotnet publish -c Release -o ../../%newVersion%/TextRankCalc
if %ERRORLEVEL% NEQ 0 (
    echo Unsuccessful build
	exit /b 0
)

cd ../../src/VowelConsCounter/
dotnet publish -c Release -o ../../%newVersion%/VowelConsCounter
if %ERRORLEVEL% NEQ 0 (
    echo Unsuccessful build
	exit /b 0
)

cd ../../src/VowelConsRater/
dotnet publish -c Release -o ../../%newVersion%/VowelConsRater
if %ERRORLEVEL% NEQ 0 (
    echo Unsuccessful build
	exit /b 0
)

cd ../../src
xcopy run.cmd ..\%newVersion%
xcopy stop.cmd ..\%newVersion%

cd ../%newVersion%
echo Successful build
