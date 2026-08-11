# My-Avatar

A Ready Player Me avatar rigged for VRChat, built in Blender 5.1.2 via Blender MCP.

## Contents

- `My_Avatar.blend` — full Blender scene with the avatar meshes bound to a Rigify-generated humanoid armature
- `My_Avatar_Rigged.fbx` — FBX export of the rig + meshes, ready to import into Unity for VRChat SDK setup

## Rig notes

- Armature is a Rigify human rig, scaled and fitted to the avatar's proportions (arm/leg bone positions measured from the actual mesh geometry, not just eyeballed)
- Deform bones use the `DEF-*` naming convention — in Unity, set the FBX rig's Animation Type to **Humanoid** and manually map the `DEF-*` bones to Unity's Humanoid bone slots
- Eyes are rigidly weighted to the head bone; all other meshes (body, head, outfit, teeth) use automatic weights
- ARKit viseme blend shapes from Ready Player Me (`viseme_aa`, `viseme_PP`, etc.) are preserved on the head and teeth meshes for VRChat lip sync
