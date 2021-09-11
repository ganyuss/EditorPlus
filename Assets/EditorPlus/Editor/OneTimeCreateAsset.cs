using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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
            List<string> typeNames = GetAllTypeNamesIn(selectedAsset);

            foreach (string typeName in typeNames) {
                foreach (var type in TypeUtils.GetTypesFromName(typeName)) {
                    if (type.IsSubclassOf(typeof(ScriptableObject)))
                        return type;
                }
            }

            return null;
        }
    
        private static List<string> GetAllTypeNamesIn(TextAsset selectedAsset) {
            string fileText = selectedAsset.text;

            Regex namespaceRegex = new Regex(@"(?<!@)namespace\s+(?<name>\S*)\s*{");
            Regex classNameRegex = new Regex(@"(?<!@)class\s+(?<name>\S*)\s*");

            MatchCollection namespaceMatches = namespaceRegex.Matches(fileText);
            List<string> namespaces = (from Match namespaceMatch in namespaceMatches select namespaceMatch.Groups["name"].Value).ToList();
        
            MatchCollection classNameMatches = classNameRegex.Matches(fileText);
            List<string> classNames = (from Match classNameMatch in classNameMatches select classNameMatch.Groups["name"].Value).ToList();

            if (namespaceMatches.Count == 0)
                return classNames;
        
            List<string> output = new List<string>();
            foreach (var className in classNames) {
                foreach (var ns in namespaces) {
                    output.Add($"{ns}.{className}");
                }
            }

            return output;
        }

        private static void CreateAsset(string folderPath, Type assetType) {

            string assetPath = Path.Combine(folderPath, assetType.Name + ".asset");
            assetPath = AssetDatabase.GenerateUniqueAssetPath(assetPath);
        
            ProjectWindowUtil.CreateAsset(ScriptableObject.CreateInstance(assetType), assetPath);
        }
    }
}