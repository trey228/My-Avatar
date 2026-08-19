using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

// Swaps the avatar's PC-detail meshes for the decimated Quest meshes (and back),
// since VRChat requires building the SAME avatar object for both platforms.
// Use before each platform-specific SDK build: Tools > Avatar > Switch To Quest/PC Meshes.
public static class QuestMeshSwitcher
{
    const string PcFbxPath = "Assets/Model/MyAvatar.fbx";
    const string QuestFbxPath = "Assets/Model/MyAvatar_Quest.fbx";

    static readonly string[] SwappableMeshNames =
    {
        "trey", "shirt", "sneakers_Baked", "Retopo_jean pants_Baked", "thick_band_watch", "eyes.001"
    };

    [MenuItem("Tools/Avatar/Switch To Quest Meshes")]
    public static void SwitchToQuest()
    {
        var avatar = FindAvatarRoot();
        if (avatar == null) { Debug.LogError("No VRCAvatarDescriptor found in scene."); return; }

        var questMeshes = LoadMeshLookup(QuestFbxPath, "_Quest");

        foreach (var name in SwappableMeshNames)
        {
            var smr = FindSkinnedMeshRenderer(avatar, name);
            if (smr == null) { Debug.LogWarning($"Could not find SkinnedMeshRenderer '{name}'"); continue; }
            if (!questMeshes.TryGetValue(name, out var questMesh)) { Debug.LogWarning($"No Quest mesh found for '{name}'"); continue; }

            smr.sharedMesh = questMesh;
            smr.sharedMaterials = RemapMaterials(questMesh);
        }

        var pendant = FindSkinnedMeshRenderer(avatar, "pendant");
        if (pendant != null)
        {
            pendant.sharedMesh = null;
            pendant.gameObject.SetActive(false);
        }

        EditorUtility.SetDirty(avatar);
        Debug.Log("Switched avatar to Quest (low-poly) meshes. Build/upload for Android now, then run 'Switch To PC Meshes' before building for Windows.");
    }

    [MenuItem("Tools/Avatar/Switch To PC Meshes")]
    public static void SwitchToPc()
    {
        var avatar = FindAvatarRoot();
        if (avatar == null) { Debug.LogError("No VRCAvatarDescriptor found in scene."); return; }

        var pcMeshes = LoadMeshLookup(PcFbxPath, "");

        foreach (var name in SwappableMeshNames)
        {
            var smr = FindSkinnedMeshRenderer(avatar, name);
            if (smr == null) continue;
            if (!pcMeshes.TryGetValue(name, out var pcMesh)) { Debug.LogWarning($"No PC mesh found for '{name}'"); continue; }

            smr.sharedMesh = pcMesh;
            smr.sharedMaterials = RemapMaterials(pcMesh);
        }

        var pendant = FindSkinnedMeshRenderer(avatar, "pendant");
        if (pendant != null)
        {
            if (pcMeshes.TryGetValue("pendant", out var pendantMesh))
                pendant.sharedMesh = pendantMesh;
            pendant.sharedMaterials = RemapMaterials(pendant.sharedMesh);
            pendant.gameObject.SetActive(true);
        }

        EditorUtility.SetDirty(avatar);
        Debug.Log("Switched avatar back to PC (full-detail) meshes.");
    }

    // Builds the renderer's material array in the mesh's submesh order, preferring
    // the project's "<name>_VRC" material variant (matches what's already on the avatar),
    // falling back to the plain "<name>" material if no _VRC variant exists.
    static Material[] RemapMaterials(Mesh mesh)
    {
        var assets = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(mesh));
        var slotNames = assets.OfType<Mesh>().FirstOrDefault(m => m == mesh) != null
            ? GetMeshMaterialNamesFromFbx(mesh)
            : new string[0];

        var result = new Material[slotNames.Length];
        for (int i = 0; i < slotNames.Length; i++)
        {
            result[i] = FindMaterial(slotNames[i] + "_VRC") ?? FindMaterial(slotNames[i]);
        }
        return result;
    }

    static string[] GetMeshMaterialNamesFromFbx(Mesh mesh)
    {
        var path = AssetDatabase.GetAssetPath(mesh);
        var allAssets = AssetDatabase.LoadAllAssetsAtPath(path);
        var go = allAssets.OfType<GameObject>().FirstOrDefault(g =>
        {
            var smr = g.GetComponent<SkinnedMeshRenderer>();
            return smr != null && smr.sharedMesh == mesh;
        });
        if (go == null) return new string[0];
        var renderer = go.GetComponent<SkinnedMeshRenderer>();
        return renderer.sharedMaterials.Select(m => m != null ? m.name : "").ToArray();
    }

    static Material FindMaterial(string exactName)
    {
        var guids = AssetDatabase.FindAssets($"t:Material {exactName}");
        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat != null && mat.name == exactName) return mat;
        }
        return null;
    }

    static GameObject FindAvatarRoot()
    {
        var all = Object.FindObjectsOfType<MonoBehaviour>(true);
        foreach (var mb in all)
            if (mb.GetType().Name == "VRCAvatarDescriptor")
                return mb.gameObject;
        return null;
    }

    static SkinnedMeshRenderer FindSkinnedMeshRenderer(GameObject root, string name)
    {
        return root.GetComponentsInChildren<SkinnedMeshRenderer>(true)
            .FirstOrDefault(s => s.gameObject.name == name);
    }

    static Dictionary<string, Mesh> LoadMeshLookup(string fbxPath, string stripSuffix)
    {
        var dict = new Dictionary<string, Mesh>();
        var assets = AssetDatabase.LoadAllAssetsAtPath(fbxPath);
        foreach (var a in assets)
        {
            if (a is Mesh mesh)
            {
                var key = mesh.name;
                if (!string.IsNullOrEmpty(stripSuffix) && key.EndsWith(stripSuffix))
                    key = key.Substring(0, key.Length - stripSuffix.Length);
                dict[key] = mesh;
            }
        }
        return dict;
    }
}
