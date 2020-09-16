CALL set_envs.cmd

IF /I "%1" == "-version" SET _version=%2
IF /I "%2" == "-version" SET _version=%3

CALL nuget_pack.cmd -version %_version%

SET _deploydll=0
IF /I "%1" == "-dll" SET _deploydll=1
IF /I "%2" == "-dll" SET _deploydll=1
IF /I "%3" == "-dll" SET _deploydll=1
IF %_deploydll% == 1 (
    echo Copy dll to %USERPROFILE%\.nuget\packages\%_PROJECT_NAME%\%_version%\lib\net461
    copy /Y %_build_dir%\%_PROJECT_NAME%.dll "%USERPROFILE%\.nuget\packages\%_PROJECT_NAME%\%_version%\lib\net461"
)
copy /Y %_build_dir%\%_PROJECT_NAME%.%_version%.nupkg "C:\NuGet"