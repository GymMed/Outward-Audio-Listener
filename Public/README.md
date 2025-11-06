<h1 align="center">
    Outward Audio Listener
</h1>
<br/>
<div align="center">
  <img src="https://raw.githubusercontent.com/GymMed/Outward-Audio-Listener/refs/heads/main/preview/images/1.png" alt="Outward Audio Listener Menu."/>
</div>

<div align="center">
	<a href="https://thunderstore.io/c/outward/p/GymMed/Audio_Listener/">
		<img src="https://img.shields.io/thunderstore/dt/GymMed/Audio_Listener" alt="Thunderstore Downloads">
	</a>
	<a href="https://github.com/GymMed/Outward-Audio_Listener/releases/latest">
		<img src="https://img.shields.io/thunderstore/v/GymMed/Audio_Listener" alt="Thunderstore Version">
	</a>
</div>

This mod lets you listen to all available sounds in Outward. It adds a menu and a custom keybinding in the settings to show or hide the menu.

## Why use this mod?

It’s designed for developers who want to preview sounds and reuse them in their own mods.

## Tip

Because Outward includes over 2,700 sounds, Unity’s dropdown can become slow when displaying all of them. This mod uses coroutines and hashsets to reduce lag, but the dropdown still rebuilds internally. For best performance, use the filtering system to narrow down the list.

## How to use

1. Either clone/download the repository with Git or GitHub Desktop, or simply download the code manually.
2. Open `src/OutwardAudioListener.sln` with any C# IDE (Visual Studio, Rider, etc)
3. When you're ready, build the solution. It will be built to the `Release` folder (next to the `src` folder).
4. Take the DLL from the `Release` folder and put it in the `BepInEx/plugins/` folder. If you use r2modman, this can be found by going into r2modman settings and clicking on `Browse Profile Folder`.

### If you liked the mod leave a star on [GitHub](https://github.com/GymMed/Outward-AudioListener) it's free
