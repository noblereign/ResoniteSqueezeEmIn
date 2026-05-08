# Squeeze 'Em In!

A [ResoniteModLoader](https://github.com/resonite-modding-group/ResoniteModLoader) mod for [Resonite](https://resonite.com/) that lets you allow specific users into your hosted sessions, even if they're at maximum capacity.

> [!TIP]
> An example scenario where this could be useful:
>
> Say you're the owner of a headless, with a session packed to the user limit. If you want to join that session, right now you might have to randomly kick someone or add a single extra user to the limit.
>
> With this mod, you could instead simply add your User ID to the config, and not have to do either! Just join as you normally would, there's always room for you 😊

## Usage

The config file for the mod will look something like this:
```json
{
  "version": "1.0.0",
  "values": {
    "Enabled": true,
    "User IDs": "",
    "User ID Array": []
  }
}
```
For headless installs, `User ID Array` is the recommended method as it's much cleaner and easier to read:
```json
"User ID Array": [
    "U-Noble",
    "U-Frooxius"
]
```

If you're using the mod on a graphical client, then the `User IDs` string is also provided for easy in-game editing. Just remember to seperate the User IDs with commas.
```json
"User IDs": "U-Noble,U-Frooxius"
```
User IDs are trimmed during processing, so you can use spaces after your commas if you wish.

## Headless Note

> [!NOTE] If you're using this on a headless install, please use [SqueezeEmIn-Headless.dll](https://github.com/noblereign/ResoniteSqueezeEmIn/releases/latest/download/SqueezeEmIn-Headless.dll), as it comes with a console command and [HeadlessTweaks](https://github.com/New-Project-Final-Final-WIP/HeadlessTweaks) integration!

<sub>The headless version has an incompatibility with [ContextMenuHookLib](https://git.unix.dog/yosh/ResoniteContextMenuHookLib/releases), hence the seperate dll. If you don't use ContextMenuHookLib, then feel free to use whichever dll you want, I guess</sub>

## Installation
1. Install [ResoniteModLoader](https://github.com/resonite-modding-group/ResoniteModLoader).
1. Place [SqueezeEmIn.dll](https://github.com/noblereign/ResoniteSqueezeEmIn/releases/latest/download/SqueezeEmIn.dll) (or [SqueezeEmIn-Headless.dll](https://github.com/noblereign/ResoniteSqueezeEmIn/releases/latest/download/SqueezeEmIn-Headless.dll)) into your `rml_mods` folder. This folder should be at `C:\Program Files (x86)\Steam\steamapps\common\Resonite\rml_mods` for a default install. You can create it if it's missing, or if you launch the game once with ResoniteModLoader installed it will create this folder for you.
1. Start the game. If you want to verify that the mod is working you can check your Resonite logs.
