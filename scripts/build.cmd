
: @echo off

: 
: Must be run from the projects git\project\scripts folder - everything is relative
: run >build [deploymentNumber]
: deploymentNumber is YYMMDD.build-number, like 190824.5
:
: Setup deployment folder
:


: all paths are relative to the git scripts folder
:
: GIT folder
:     -- aoSample
:			-- collection
:				-- Sample
:					unzipped collection files, must include one .xml file describing the collection
:			-- server 
: 			(all files related to server code)
:				-- aoSample (visual studio project folder)
:			-- ui 
:				(all files related to the ui
:			-- etc 
:				(all misc files)


: -- name of the collection on the site (should NOT include ao prefix). This is the name as it appears on the navigator
set collectionName=aoDistanceLearning

: -- name of the collection folder, (should NOT include ao prefix)
set collectionPath=..\collections\aoDistanceLearning\

: -- name of the solution. SHOULD include ao prefix
set solutionName=aoDistanceLearning.sln

: -- name of the solution. SHOULD include ao prefix
set binPath=..\server\aoDistanceLearning\bin\debug\

: -- name of the solution. SHOULD include ao prefix
set msbuildLocation=C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\

: -- name of the solution. SHOULD include ao prefix
set deploymentFolderRoot=C:\Deployments\aoDistanceLearning\Dev\



set deploymentNumber=%1
set year=%date:~12,4%
set month=%date:~4,2%
set day=%date:~7,2%

:
: if deployment number not entered, set it to date.1
:
IF [%deploymentNumber%] == [] (
	echo No deployment folder provided on the command line, use current date
	set deploymentTimeStamp=%year%%month%%day%
)
:
: if deployment folder exists, delete it and make directory
:

set suffix=1
:tryagain
set deploymentNumber=%deploymentTimeStamp%.%suffix%
if not exist "%deploymentFolderRoot%%deploymentNumber%" goto :makefolder
set /a suffix=%suffix%+1
goto tryagain
:makefolder
md "%deploymentFolderRoot%%deploymentNumber%"

: ==============================================================
:
echo build 
:
cd ..\server
"%msbuildLocation%msbuild.exe" %solutionName%
if errorlevel 1 (
   echo failure building
   pause
   exit /b %errorlevel%
)
cd ..\scripts

:
: ==============================================================
:
echo Build addon collection
:

: copy bin folder assemblies to collection folder
copy "%binPath%*.dll" "%collectionPath%"

pause

: build ui zip
cd ..\ui
"c:\program files\7-zip\7z.exe" a "%collectionPath%uiDistanceLearning.zip" 
cd ..\scripts

pause

: create collection and copy to deploy folder
cd %collectionPath%
"c:\program files\7-zip\7z.exe" a "%collectionName%.zip"
xcopy "%collectionName%.zip" "%deploymentFolderRoot%%versionNumber%" /Y
xcopy "%collectionName%.zip" "c:\deployments\_current_sprint" /Y
cd ..\..\scripts

pause

: clean collection folder (leave collection xml)
del "%collectionPath%*.dll" /Q
del "%collectionPath%*.zip" /Q

pause