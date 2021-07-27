@echo off

setlocal
cd %~dp0

REM Validate Maven
call mvn -v 1> NUL 2> NUL
if %errorlevel% neq 0 goto :maven_notfound

call mvn clean package

setlocal enabledelayedexpansion enableextensions
set LIST=
for %%x in (./target/demoserver-*.jar) do set LIST=!LIST! %%x
set JARFILE=%LIST:~1%

call java -javaagent:./jetty-alpn-agent-2.0.9.jar -cp ./target/%JARFILE% com.universal_tools.demoserver.HttpServer

goto :EOF

:maven_notfound
echo Maven not found!
echo See installation instructions here: https://maven.apache.org/install.html
exit /B 1