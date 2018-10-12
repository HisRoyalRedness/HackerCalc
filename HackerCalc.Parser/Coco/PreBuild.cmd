@echo off
echo === PreBuild
setlocal

set ProjDir=%~1
set Config=%~2

echo ProjDir:     %ProjDir%
echo Config:      %Config%
echo CmdLine:     %%ProjDir%%\Coco\Coco.exe HackerCalc.ATG -namespace HisRoyalRedness.com -frames %%ProjDir%%\Coco -trace FJSX -symnames

pushd "%ProjDir%"
"%ProjDir%\Coco\Coco.exe" HackerCalc.ATG -namespace HisRoyalRedness.com -frames "%ProjDir%\Coco" -trace FJSX -symnames
popd