
rem all paths are relative to the git scripts folder


rem -- the application on the local server where this collection will be installed
set appName=veronica

call build.cmd

rem upload to contensive application
c:
cd "%deploymentFolderRoot%%versionNumber%"
cc -a %appName% --installFile "%collectionName%.zip"
if errorlevel 1 (
   echo failure installing
   pause
   exit /b %errorlevel%
)
pause