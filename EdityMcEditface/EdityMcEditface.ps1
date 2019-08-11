# This script starts EdityMcEditface when it is published.
$scriptPath = Split-Path $script:MyInvocation.MyCommand.Path
dotnet "$scriptPath/EdityMcEditface/EdityMcEditface.dll"