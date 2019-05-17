@echo off
cls
rem Get the base filename
for %%i in (%1) do set FILE_BASE=%%~dpni

rem Convert from svg to png (256x256)
"C:\Program Files\Inkscape\inkscape.exe" "%FILE_BASE%.svg" -e "%FILE_BASE%.png" --export-area-page -w 256 -h 256 --without-gui

rem Resize the png into smaller bmps, then add them all into an ico file
"C:\Program Files\ImageMagick-7.0.8-Q16-HDRI\magick.exe" "%FILE_BASE%.png" -define icon:auto-resize="256,128,96,64,48,32,16" "%FILE_BASE%.ico"

del /f "%FILE_BASE%.png"