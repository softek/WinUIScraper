@ECHO OFF
SETLOCAL

SET outputfile=..\WinUIScraper.cs

if exist "%outputfile%" DEL "%outputfile%"

ECHO Hi

call :writeHeader
call :includeCSFilesFrom WinUIScraper\Declarative
call :includeCSFilesFrom WinUIScraper\Providers

goto :eof


:writeHeader

echo // WinUIScraper - A declarative approach to screen scraping in Windows > %outputfile%
echo. >> %outputfile%
echo // Built on %date% %time% >> %outputfile%
echo. >> %outputfile%

goto :eof

:includeCSFilesFrom
ECHO Including Files From %1
for /r %%f in (%1\*.cs) DO (
  ECHO   Including %%f.
  ECHO. >>%outputfile%
  ECHO //////////////////////////////////////////////////////////////////////// >>%outputfile%
  ECHO // %%f >>%outputfile%
  ECHO. >>%outputfile%
  type %%f. >>%outputfile%
)
goto :eof
