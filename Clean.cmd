@echo off
cls
powershell -nologo -executionpolicy remotesigned -command "& { %~dpn0.ps1 }"
"%~dp0HackerCalc.Parser\Coco\PreBuild.cmd" "%~dp0HackerCalc.Parser" "Debug"
pause