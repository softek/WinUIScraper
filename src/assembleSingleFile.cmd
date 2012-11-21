@ECHO OFF
SETLOCAL

SET outputfile=..\WinUIScraper.cs

if exist "%outputfile%" DEL "%outputfile%"

call :writeHeader
call :includeCSFilesFrom WinUIScraper\Declarative
call :includeCSFilesFrom WinUIScraper\Providers
call :compile

goto :eof


:writeHeader

echo // WinUIScraper - A declarative approach to screen scraping in Windows > %outputfile%
echo. >> %outputfile%
echo // Built on %date% %time% >> %outputfile%
echo. >> %outputfile%

goto :eof

:includeCSFilesFrom
ECHO Including Files From %1
for /r %1 %%f in (*.cs) DO (
  ECHO   Including %%f.
  ECHO. >>%outputfile%
  ECHO //////////////////////////////////////////////////////////////////////// >>%outputfile%
  ECHO // %%f >>%outputfile%
  ECHO. >>%outputfile%
  type %%f. >>%outputfile%
)
goto :eof

:compile

ECHO.
ECHO.
ECHO ==========================================================================
ECHO Building WinUIScraper.dll to prove that the source compiles
ECHO.

DEL WinUIScraper.dll
IF ERRORLEVEL 1 GOTO :EOF
csc /target:library ..\WinUIScraper.cs /r:"%ProgramFiles(x86)%\Reference Assemblies\Microsoft\Framework\v3.0\UIAutomationClient.dll" /r:"%ProgramFiles(x86)%\Reference Assemblies\Microsoft\Framework\v3.0\UIAutomationClient.dll"  /r:"%ProgramFiles(x86)%\Reference Assemblies\Microsoft\Framework\v3.0\UIAutomationClientsideProviders.dll"  /r:"%ProgramFiles(x86)%\Reference Assemblies\Microsoft\Framework\v3.0\UIAutomationProvider.dll" /r:"%ProgramFiles(x86)%\Reference Assemblies\Microsoft\Framework\v3.0\UIAutomationTypes.dll"
IF EXIST WinUIScraper.dll ECHO OK

goto :eof