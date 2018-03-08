@echo off
echo === Test
setlocal

set SolutionDir=%~1
set Config=%~2

echo SolutionDir: %SolutionDir%
echo Config:      %Config%

pushd "%SolutionDir%"
rem dotnet test HackerCalc.Test\HackerCalc.Test.csproj --no-build
popd