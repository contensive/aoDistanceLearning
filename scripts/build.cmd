
@echo off

rem 
rem Must be run from the projects git\project\scripts folder - everything is relative
rem run >build [deploymentNumber]
rem deploymentNumber is YYMMDD.build-number, like 190824.5
rem
rem Setup deployment folder
rem


rem all paths are relative to the git scripts folder
rem
rem GIT folder
rem     -- aoSample
rem			-- collection
rem				-- Sample
rem					unzipped collection files, must include one .xml file describing the collection
rem			-- server 
rem 			(all files related to server code)
rem				-- aoSample (visual studio project folder)
rem			-- ui 
rem				(all files related to the ui
rem			-- etc 
rem				(all misc files)

rem -- major version 5, minor does not matter set 1
set majorVersion=5
set minorVersion=1

rem -- name of the collection on the site (should NOT include ao prefix). This is the name as it appears on the navigator
set collectionName=Distance Learning

rem -- name of the collection folder, (should NOT include ao prefix)
set collectionPath=..\collections\Distance Learning\

rem -- name of the solution. SHOULD include ao prefix
set solutionName=aoDistanceLearning.sln

rem -- name of the solution. SHOULD include ao prefix
set binPath=..\server\aoDistanceLearning\bin\debug\

rem -- name of the solution. SHOULD include ao prefix
set msbuildLocation=C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\

rem -- name of the solution. SHOULD include ao prefix
set deploymentFolderRoot=C:\Deployments\aoDistanceLearning\Dev\



set deploymentNumber=%1
set year=%date:~12,4%
set month=%date:~4,2%
set day=%date:~7,2%

rem
rem if deployment number not entered, set it to date.1
rem
IF [%deploymentNumber%] == [] (
	echo No deployment folder provided on the command line, use current date
	set deploymentTimeStamp=%year%%month%%day%
)
rem
rem if deployment folder exists, delete it and make directory
rem

set suffix=1
:tryagain
set deploymentNumber=%deploymentTimeStamp%.%suffix%
if not exist "%deploymentFolderRoot%%deploymentNumber%" goto :makefolder
set /a suffix=%suffix%+1
goto tryagain
:makefolder
md "%deploymentFolderRoot%%deploymentNumber%"

rem ==============================================================
rem
echo build 
rem
cd ..\server
"%msbuildLocation%msbuild.exe" %solutionName%
if errorlevel 1 (
   echo failure building
   pause
   exit /b %errorlevel%
)
cd ..\scripts

rem pause

rem ==============================================================
rem
echo Build addon collection
rem

rem remove old DLL files from the collection folder
del "%collectionPath%"\*.DLL
del "%collectionPath%"\*.config

rem copy bin folder assemblies to collection folder
copy "%binPath%*.dll" "%collectionPath%"

rem create new collection zip file
c:
cd %collectionPath%
del "%collectionName%.zip" /Q
"c:\program files\7-zip\7z.exe" a "%collectionName%.zip"
xcopy "%collectionName%.zip" "%deploymentFolderRoot%%deploymentNumber%" /Y
cd ..\..\scripts

