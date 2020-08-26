@echo off
set ASPNETCORE_ENVIRONMENT=ork%1
dotnet watch --project Tide.Ork run --urls=http://0.0.0.0:500%1 --launch-profile "ork%1" --logger "console;verbosity=detailed"