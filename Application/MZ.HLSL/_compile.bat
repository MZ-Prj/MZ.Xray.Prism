@echo off
setlocal enabledelayedexpansion

set "FXC_PATH=C:\Program Files (x86)\Windows Kits\10\bin\10.0.22621.0\x64\fxc.exe"

if "%1"=="" (
    echo Usage: compile_hlsl.bat [HLSL_FILE] [TARGET_SHADER_MODEL] [OUTPUT_FILE]
    echo Example: compile_hlsl.bat my_shader.hlsl ps_4_0 my_shader.ps
    pause
    exit /b
)

set "HLSL_FILE=%1"
set "TARGET=%2"
set "OUTPUT_FILE=%3"

if "%TARGET%"=="" set "TARGET=ps_4_0"
if "%OUTPUT_FILE%"=="" set "OUTPUT_FILE=%~n1.ps"

set "ASM_FILE=%~n1.asm"

echo Compiling "%HLSL_FILE%" with target "%TARGET%"...
call "%FXC_PATH%" /T %TARGET% /Fo "%OUTPUT_FILE%" /Fc "%ASM_FILE%" "%HLSL_FILE%"

if exist "%OUTPUT_FILE%" (
    echo Compilation successful! Output: "%OUTPUT_FILE%"
    echo Checking instruction count in "%ASM_FILE%"...
    findstr "instruction slots" "%ASM_FILE%"
) else (
    echo Compilation failed. Please check your HLSL file and try again.
)

pause