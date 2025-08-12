@echo off
setlocal enabledelayedexpansion

set "root_dir=D:\workspace\MZ.Xray.Prism\Application"

echo Cleaning all projects under %root_dir%
echo.

for /d /r "%root_dir%" %%d in (*) do (
    if exist "%%d\bin" (
        echo Deleting BIN in %%d
        rd /s /q "%%d\bin"
    )
    if exist "%%d\obj" (
        echo in %%d
        rd /s /q "%%d\obj"
    )
    if exist "%%d\..\Build" (
        echo in %%~pd
        rd /s /q "%%d\..\Build"
    )
)

echo.
echo Clean completed.
pause