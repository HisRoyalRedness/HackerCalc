@echo off
echo === PreBuild
setlocal

set ProjDir=%~1
set Config=%~2

echo ProjDir:     %ProjDir%
echo Config:      %Config%

pushd "%ProjDir%"


"%ProjDir%Coco\Coco.exe" Coco.ATG -namespace HisRoyalRedness.com -frames "%ProjDir%Coco" -trace FJSX -symnames