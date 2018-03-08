@echo off
echo === PostBuild
setlocal

set ProjDir=%~1
set Config=%~2

echo ProjDir:     %ProjDir%
echo Config:      %Config%

pushd "%ProjDir%"
rem dotnet test --no-build
popd