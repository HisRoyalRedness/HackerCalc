@echo off
echo === Update coco.exe
setlocal

set ProjDir=%~1

echo ProjDir:     %ProjDir%

pushd "%ProjDir%"
copy /y "..\..\CocoR-CSharp\bin\Release\coco.exe" Coco
popd

