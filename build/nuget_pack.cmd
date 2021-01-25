IF "%DevEnvDir%%" == "" (
    echo "This script must be run from a Developer Command Prompt for Visual Studio"
    exit /b 1
)

CALL set_envs.cmd
SET _configuration=Debug
IF /I "%1" == "-release" SET _configuration=Release
IF /I "%1" == "-version" SET _version=%2
IF /I "%2" == "-version" SET _version=%3

SET _build_dir=%_src_root%\bin\%_configuration%

nuget pack %_src_root% -Build -OutputDirectory %_build_dir% -version %_version% -Properties Configuration=%_configuration%