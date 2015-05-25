
REM  /*--   Set username, password and appCode --*/
set appTitle=%0
set appCode=%1
set cameraUrl=%2
set userName=%3
set passWord=%4

set FilePath=./%appCode%/res/raw/userdata.txt

REM /*--   Delete the project if already exists-- */
RD /S /Q  "%~dp0CameraApps\%appCode%"

REM /*--  Create a folder in CameraApps named appCode --*/
cd /
cd /D "%~dp0CameraApps"
mkdir %appCode%

REM /*-- Set  environment variable 'path' value --*/
set path=c:/windows/system32

REM /*-- Copy the content of original project to appCode folder --*/
cd /
xcopy "%~dp0OnTheFlyCam" "%~dp0CameraApps\%appCode%" /s /e
cd /
cd /D "%~dp0CameraApps"

REM /*-- Copy the content of original project to appCode folder --*/
REM ReplaceInFile.exe  "./%appCode%/AndroidManifest.xml" "package='com.camba.cam'" "package='com.camba.app_%appCode%'"  
ReplaceInFile.exe  "./%appCode%/AndroidManifest.xml" "com.camba.cam" "com.camba.app_%appCode%"  
ReplaceInFile.exe  "./%appCode%/AndroidManifest.xml" "com.camba.app_%appCode%.LiveView" "com.camba.cam.LiveView"  
ReplaceInFile.exe  "./%appCode%/AndroidManifest.xml" "@string/app_name" "%appCode%" 
ReplaceInFile.exe ".\%appCode%\src\com\camba\cam\LiveView.java" "http://killruddery1.dtdns.net:8001/snapshot1.jpg" "%cameraUrl%"
ReplaceInFile.exe ".\%appCode%\src\com\camba\cam\LiveView.java" "com.camba.cam.R;" "com.camba.app_%appCode%.R;"
ReplaceInFile.exe ".\%appCode%\src\com\camba\cam\LiveView.java" "com.camba.cam.R.drawable;" "com.camba.app_%appCode%.R.drawable;"
ReplaceInFile.exe ".\%appCode%\src\com\camba\cam\LiveView.java" "__username__" "%userName%"
ReplaceInFile.exe ".\%appCode%\src\com\camba\cam\LiveView.java" "__password__" "%passWord%"

REM /*-- Copy this image to required directories (different dpi) in \\res\\ --*/
IconFilePlaceApp.exe "%~dp0CameraApps\%appCode%\ic_launcher.png"

REM /*-- Set environment variable 'path' value --*/
REM set path=libgcc_s_dw2-1.dll
REM set path=c:/mingw/bin

cd /
cd "%~dp0CameraApps"

REM /*-- Call exe file to set data in text file --*/
SaveData.exe   "%FilePath%" "%userName%" "%passWord%" "%appCode%"

cd /

REM /*--  Create build.xml and then call ant release --*/

echo "Call android update project"

cd /
cd "%~dp0sdk\tools"
call android update project --name app_%appCode% --target 1 --path ../../CameraApps/%appCode%

set ANT_HOME=%~dp0ant
set JAVA_HOME=%~dp0jdk1.6.0_18

REM set JAVA_HOME=C:\Program Files\Java\jdk1.7.0_07
set PATH=%PATH%;%ANT_HOME%\bin
cd "%~dp0CameraApps\%appCode%"

echo "Call ant"
call ant release -q
cd /

set path=c:/windows/system32

REM /* delete any .apk file from release target location if already exists */
del "%~dp0UtilApps\OnTheFlyAppWeb1_PrecompiledWeb\downloads\app_%appCode%-release.apk"

REM /* copy .apk file to release target location */
xcopy "%~dp0CameraApps\%appCode%\bin\app_%appCode%-release.apk" "%~dp0UtilApps\OnTheFlyAppWeb1_PrecompiledWeb\downloads\" /s /e

REM /* change working directory to release target location */
cd /D "%~dp0UtilApps\OnTheFlyAppWeb1_PrecompiledWeb\downloads\"

REM /* delete any .apk file from release target location if already exists with name 'appCode.apk' */
del "%appCode%.apk"

REM /* rename recently copied 'release.apk' file with name 'app_title.apk' */
ren "app_%appCode%-release.apk"  "%appTitle%.apk"

cd /D "%~dp0"