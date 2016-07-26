set HERE=%~dp0
cd ..\wwwroot\lib
mklink /D htmlrest %HERE%htmlrest
cd %HERE%