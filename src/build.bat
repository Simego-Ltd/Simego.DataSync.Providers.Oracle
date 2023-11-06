set p=Simego.DataSync.Providers.Oracle
dotnet build -c Release
rmdir ..\dist\ /S /Q
mkdir ..\dist\files\%p%
xcopy ..\src\%p%\bin\Release\net7.0-windows\*.* ..\dist\files\%p%\*.* /y /s
cd ..\dist\files\
del .\%p%\Simego.DataSync.Core.dll
del .\%p%\Simego.DataSync.Providers.Ado.dll
tar.exe -acf ..\%p%.zip *.*
cd ..\..\src


