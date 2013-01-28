@ECHO OFF

SET BuildDir=%~dp0\Build
SET BinDir=.\Build\Bin
SET Register=true

IF "%1"=="/?" GOTO HELP

IF DEFINED DevEnvDir GOTO OPTIONS

IF NOT DEFINED VS110COMNTOOLS GOTO VSNOTFOUND

ECHO.
ECHO ------------------------------------------
ECHO Setting the build environment
ECHO ------------------------------------------
ECHO.
CALL "%VS110COMNTOOLS%\vsvars32.bat" > NUL 
IF ERRORLEVEL 1 GOTO ERROR


:OPTIONS

REM  ----------------------------------------------------
REM  If the current parameter is /u, removes the 
REM  registration for the assemblies.
REM  ----------------------------------------------------

IF /i "%1"=="/u" (
	SET Register=false
	SHIFT
)


ECHO.
ECHO ------------------------------------------
ECHO Registration initiated
ECHO ------------------------------------------
ECHO.

REM Expand folder variables to full paths
FOR %%G IN (%BinDir%) DO SET BinDir=%%~fG

CALL msbuild.exe "%BuildDir%\RegisterAssemblies.msbuild" /verbosity:normal /p:BinFolder="%BinDir%" /p:Register=%Register%
IF ERRORLEVEL 1 GOTO ERROR

IF /i "%Register%" == "true" (
	ECHO.
	ECHO ------------------------------------------
	ECHO Registration completed
	ECHO ------------------------------------------
	ECHO.
) ELSE (
	ECHO.
	ECHO ------------------------------------------
	ECHO Registration removed
	ECHO ------------------------------------------
	ECHO.
)


:EXIT

@EXIT /B

:VSNOTFOUND

ECHO.
ECHO ------------------------------------------
ECHO 'VS100COMNTOOLS' not set
ECHO Cannot set the build environment
ECHO ------------------------------------------
ECHO.

@EXIT /B

:ERROR

ECHO.
ECHO ------------------------------------------
ECHO An error occured while updating the library - %ERRORLEVEL%
ECHO ------------------------------------------
ECHO.

@EXIT /B

:HELP

ECHO.
ECHO RegisterBinaries.bat:
ECHO Registers the binaries folders as assembly folders.
ECHO Must be executed from the source folder
ECHO.
ECHO Usage: 
ECHO RegisterBinaries.bat /?
ECHO RegisterBinaries.bat [/u]
ECHO.
ECHO Options:
ECHO - /? : Show this help message
ECHO - /u : Remove the binaries registration
ECHO.

ECHO ON
@EXIT /B