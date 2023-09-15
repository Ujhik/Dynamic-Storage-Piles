## Development
BepInEx must be setup at manual or with r2modman/Thunderstore Mod Manager.
Jötunn must be installed.
Everything else is setup to be as automatic as possible, there is no need to copy any files or folders.

Note the master branch will always use a stable Jötunn version while others may use a directly compiled one.

Create a file called `Environment.props` inside the project root.
Copy the example and change the Valheim install path to your location.
If you use r2modman/Tunderstore Mod Manager you can set the path too, but this is optional.

```
<?xml version="1.0" encoding="utf-8"?>
<Project ToolsVersion="Current" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
    <PropertyGroup>
        <!-- Valheim install folder. This is normally found automatically, uncomment to overwrite it. Needs to be your path to the base Valheim folder. -->
        <!-- <VALHEIM_INSTALL>X:\PathToYourSteamLibary\steamapps\common\Valheim</VALHEIM_INSTALL>-->
    
        <!-- This is the folder where your build gets copied to when building the project -->
        <DEPLOY_FOLDER>$(AppData)\r2modmanPlus-local\Valheim\profiles\Develop</DEPLOY_FOLDER>
    </PropertyGroup>
</Project>
```
This project requires the publicized Valheim dlls present at `VALHEIM_INSTALL\valheim_Data\Managed\publicized_assemblies\assembly_[assembly]_publicized.dll`.
Use any publicizer, see here for example: https://github.com/CabbageCrow/AssemblyPublicizer

If the paths are set correctly, all assemblies are loaded and the project can be build.
Now you can run `deploy.sh`, this will copy the mod to your BepInEx plugin folder as you setup in `Environment.props`.

If you want to rebuild the asset bundle, open the Unity project.
Because ValheimHopper.dll, Jotunn.dll and dependencies must be present inside Unity, `deploy.sh` had to be ran at least once.
Now you can run `Assets/Build AssetBundles` at the toolbar, this copies the asset bundle back to the C# project.
