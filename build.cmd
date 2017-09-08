@echo off
powershell -ExecutionPolicy ByPass -File build.ps1 -target "Default" -configuration "Debug"
IF [%1]==[] pause