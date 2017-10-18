@echo off
powershell -ExecutionPolicy ByPass -File build.ps1 -target "Developer-Build" -configuration "Debug"
IF [%1]==[] pause