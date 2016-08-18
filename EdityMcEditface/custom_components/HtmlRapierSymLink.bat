set HERE=%~dp0
cd ..\wwwroot\lib
mklink /D HtmlRapier %HERE%HtmlRapier
cd %HERE%