$ErrorActionPreference = "Stop";
dotnet run --project build -- $args
if ($LASTEXITCODE -ne 0) { throw "Exit code is $LASTEXITCODE" }