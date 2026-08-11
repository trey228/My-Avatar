# My-Avatar

A Ready Player Me avatar rigged for VRChat. This repo is the full Unity VRChat avatar project (`MyAvatar_Claude`), built with the VRChat Avatars SDK3.

## Structure

- `Assets/Model/Avatar2026.blend` — the source Blender file (Blender 5.1.2), containing the avatar meshes bound to a Rigify-generated humanoid armature. This is the live file Unity imports directly.
- `Assets/` — Unity assets for the avatar project
- `Packages/` — package manifest, including the VRChat SDK3 (Avatars) and the [mcp-unity](https://github.com/CoderGamester/mcp-unity) editor bridge used for AI-assisted development
- `ProjectSettings/` — Unity project settings

`Library/`, `Temp/`, `Logs/`, `UserSettings/`, and `obj/` are intentionally excluded (see `.gitignore`) — they're regenerable Unity cache, not source.

## Rig notes

- Armature is a Rigify human rig, scaled and fitted to the avatar's proportions (arm/leg bone positions measured from the actual mesh geometry, not just eyeballed)
- Deform bones use the `DEF-*` naming convention — Unity's Model Importer has this set to **Humanoid** animation type; bone mapping may need manual review in the Avatar configuration
- Eyes are rigidly weighted to the head bone; all other meshes (body, head, outfit, teeth) use automatic weights (body mesh required envelope weighting due to fragmented topology breaking Blender's heat solver)
- ARKit viseme blend shapes from Ready Player Me (`viseme_aa`, `viseme_PP`, etc.) are preserved on the head and teeth meshes for VRChat lip sync

## Setup

Requires the VRChat Creator Companion (VCC) or manual VPM package resolution to pull in `com.vrchat.base` / `com.vrchat.avatars`. To use the `mcp-unity` AI bridge, see `Tools > MCP Unity > Server Window` inside the Unity Editor.
