@echo off
setlocal enabledelayedexpansion

:: Source paths
set "sourceDll=C:\Users\pc\source\repos\OutwardAudioListener\Release\OutwardAudioListener.dll"
set "sourceBundle=F:\AudioListener\Assets\AssetsBundles\outwardaudiolistenerbundle"

:: Profiles array (quoted entries for readability)
set profiles="Main" "Development"

:: Base destination folder
set "baseProfilePath=C:\Users\pc\AppData\Roaming\r2modmanPlus-local\OutwardDe\profiles"

:: Extra bundle copy target
set "publicBundlePath=C:\Users\pc\source\repos\OutwardAudioListener\Public\BepInEx\plugins\SideLoader\AssetBundles\outwardaudiolistenerbundle"

:: --- Copy DLL into each profile ---
if exist "%sourceDll%" (
    for %%p in (%profiles%) do (
        set "destinationDll=%baseProfilePath%\%%~p\BepInEx\plugins\gymmed-OutwardAudioListener"
        echo Copying "%sourceDll%" to "!destinationDll!"
        if not exist "!destinationDll!" mkdir "!destinationDll!"
        copy /Y "%sourceDll%" "!destinationDll!"
    )
) else (
    echo Source dll file does not exist!
)

:: --- Copy Bundle into each profile ---
if exist "%sourceBundle%" (
    for %%p in (%profiles%) do (
        set "destinationBundle=%baseProfilePath%\%%~p\BepInEx\plugins\gymmed-OutwardAudioListener\SideLoader\AssetBundles"
        echo Copying "%sourceBundle%" to "!destinationBundle!\outwardaudiolistenerbundle"
        if not exist "!destinationBundle!" mkdir "!destinationBundle!"
        copy /Y "%sourceBundle%" "!destinationBundle!\outwardaudiolistenerbundle"
    )
    
    :: Also copy to Public folder
    echo Copying "%sourceBundle%" to "%publicBundlePath%"
    if not exist "%~dp0%publicBundlePath%" mkdir "%~dp0%publicBundlePath%"
    copy /Y "%sourceBundle%" "%publicBundlePath%"
) else (
    echo Source Bundle file does not exist!
)

pause