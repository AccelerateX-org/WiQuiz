@echo off
powershell -ExecutionPolicy ByPass -File build.ps1 -target "AppVeyor" -configuration "Debug"
IF [%1]==[] pause