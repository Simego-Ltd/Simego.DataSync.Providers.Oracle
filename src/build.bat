set p=Simego.DataSync.Providers.Oracle
"C:\Program Files\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\msbuild.exe" /t:Build /p:Configuration=Release /p:NoWarn=1591
rmdir ..\dist\ /S /Q
mkdir ..\dist\files\%p%
xcopy ..\src\%p%\bin\Release\net48\*.* ..\dist\files\%p%\*.* /y
cd ..\dist\files\
del .\%p%\Simego.DataSync.dll
del .\%p%\Simego.DataSync.Providers.Ado.dll
del .\%p%\Simego.DataSync.Core.dll
del .\%p%\Simego.DataSync.Security.dll
tar.exe -acf ..\%p%.zip *.*
cd ..\..\src


