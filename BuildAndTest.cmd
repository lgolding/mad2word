@echo off
setlocal

if exist bld rmdir /s /q bld

set Platform=Any CPU
set Configuration=Release

:NextArg
if "%1" == "" goto :EndArgs
if "%1" == "/config" (
    if not "%2" == "Debug" if not "%2" == "Release" echo error: /config must be either Debug or Release && goto :ExitFailed
    set Configuration=%2&& shift && shift && goto :NextArg
)
echo Unrecognized option "%1" && goto :ExitFailed

:EndArgs

msbuild /verbosity:minimal /target:rebuild src\mad2word.sln /p:Configuration=%Configuration% /filelogger /fileloggerparameters:Verbosity=detailed
if "%ERRORLEVEL%" NEQ "0" (
goto ExitFailed
)

set XUNIT=src\packages\xunit.runner.console.2.1.0\tools\xunit.console.x86.exe

%XUNIT% bld\bin\Mad2WordLib.UnitTests\AnyCPU_%Configuration%\Mad2WordLib.UnitTests.dll
if "%ERRORLEVEL%" NEQ "0" (
goto ExitFailed
)

goto Exit

:ExitFailed
@echo .
@echo SCRIPT FAILED

:Exit

endlocal && exit /b %ERRORLEVEL%