@echo off 
Rem Copyright (C) Michael Lam, All Rights Reserved
Rem This script is used to run a maya script
set mayaPath=%1
set melCommand=%2
C:
cd %mayaPath%
mayabatch.exe -noAutoloadPlugins -prompt -command %melCommand%
exit /b