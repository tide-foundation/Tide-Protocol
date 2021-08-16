@echo off
set ASPNETCORE_ENVIRONMENT=ork%1
dotnet watch --project Tide.Ork run --no-build --no-launch-profile --urls=http://0.0.0.0:500%1 -- --register http://localhost:500%1
