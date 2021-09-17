using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace EditorPlus.Editor {
    public static class OneTimeCreateAsset {
        private const string MenuPath = "Assets/Create ScriptableObject Asset";
    
        [MenuItem(MenuPath, false, 30)]
        private static void CreateScriptableObject() {
            TextAsset selectedAsset = Selection.activeObject as TextAsset;
            Type assetType = GetAssetTypeFrom(selectedAsset);
            string folder = Path.GetDirectoryName(AssetDatabase.GetAssetPath(selectedAsset));
            CreateAsset(folder, assetType);
        }
    
        [MenuItem(MenuPath, true, 30)]
        private static bool CanCreateScriptableObject() {
            TextAsset selectedAsset = Selection.activeObject as TextAsset;
            if (selectedAsset == null) return false;

            Type assetType = GetAssetTypeFrom(selectedAsset);
            return assetType != null;
        }

        private static Type GetAssetTypeFrom(TextAsset selectedAsset) {
            
            foreach (var type in TypeUtils.GetTypesFromName(selectedAsset.name)) {
                if (type.IsSubclassOf(typeof(ScriptableObject)))
                    return type;
            }

            return null;
        }

        private static void CreateAsset(string folderPath, Type assetType) {

            string assetPath = Path.Combine(folderPath, assetType.Name + ".asset");
            assetPath = AssetDatabase.GenerateUniqueAssetPath(assetPath);
        
            ProjectWindowUtil.CreateAsset(ScriptableObject.CreateInstance(assetType), assetPath);
        }
    }
}