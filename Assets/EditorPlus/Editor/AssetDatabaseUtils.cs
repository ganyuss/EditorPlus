using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EditorPlus.Editor {
    public static class AssetDatabaseUtils
    {
        private static AssetType[] GuidsToArray<AssetType>(string[] guids) where AssetType : Object {

            List<AssetType> output = new List<AssetType>();
            for (var i = 0; i < guids.Length; i++) {
                var path = AssetDatabase.GUIDToAssetPath(guids[i]);
                var element = AssetDatabase.LoadAssetAtPath<AssetType>(path);
                if (element) {
                    output.Add(element);
                }
            }
            return output.ToArray();
        }
    
        [NotNull]
        public static AssetType[] GetAll<AssetType>(string[] searchInFolders = null) where AssetType : Object {
            string[] guids = AssetDatabase.FindAssets("t:" + typeof(AssetType).Name, searchInFolders);
            return GuidsToArray<AssetType>(guids);
        }
    
        [NotNull]
        public static AssetType[] GetAllByLabel<AssetType>(params string[] labels) where AssetType : Object {
            string filter = string.Join(" ", labels.Select(label => $"l:{label}"));
            string[] guids = AssetDatabase.FindAssets(filter, null);
            return GuidsToArray<AssetType>(guids);
        }

        private static AssetType AssertSingle<AssetType>(AssetType[] values) where AssetType : Object {
            if (values.Length > 1) {
                Debug.LogWarning("Called AssetDatabaseUtils.GetSingle with a class that has multiple assets instances.");
            }
            return values.Length == 0 ? null : values[0];
        }

        public static AssetType GetSingle<AssetType>(string[] searchInFolders = null) where AssetType : Object {
            AssetType[] all = GetAll<AssetType>(searchInFolders);
            return AssertSingle(all);
        }
    
        public static AssetType GetSingleByLabel<AssetType>(params string[] labels) where AssetType : Object {
            AssetType[] all = GetAllByLabel<AssetType>(labels);
            return AssertSingle(all);
        }
    
        [NotNull] 
        public static AssetType GetSingleOrCreate<AssetType>(string newAssetPath) where AssetType : ScriptableObject {
            AssetType asset = GetSingle<AssetType>();
            if (asset is null) {
                asset = ScriptableObject.CreateInstance<AssetType>();
            
                MakeSureDirectoryExist(Path.GetDirectoryName(newAssetPath));
                AssetDatabase.CreateAsset(asset, newAssetPath);
            }
        
            return asset;
        }
        
        public static ScriptableObject[] GetAll(Type assetType) {
            string[] guids = AssetDatabase.FindAssets("t:" + assetType.Name, null);
            return GuidsToArray<ScriptableObject>(guids);
        }
        
        public static ScriptableObject GetSingle(Type assetType) {
            ScriptableObject[] all = GetAll(assetType);
            return AssertSingle(all);
        }
        
        private static void MakeSureDirectoryExist(string directoryPath) {

            if (Directory.Exists(directoryPath)) {
                return;
            }

            string parent = Path.GetDirectoryName(directoryPath);
            MakeSureDirectoryExist(parent);
            AssetDatabase.CreateFolder(parent, Path.GetFileName(directoryPath));
        }
    }
}
