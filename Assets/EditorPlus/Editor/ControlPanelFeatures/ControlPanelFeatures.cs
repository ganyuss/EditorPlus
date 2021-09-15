using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace EditorPlus.Editor {
    public static class ControlPanelFeatures {
        private const string GeneratedCodeFolderId = "EditorPlus.GeneratedCode.Editor";
        private const string CustomDrawerAttributeFileName = "CustomDrawerAttributeDeclaration.cs";

        public static void GenerateCustomDrawerAttributes(List<string> namespaceBlacklist) {
            string codeGenerationFolderPath = FolderLocator.GetFolder(GeneratedCodeFolderId);
            string drawerAttributeFilePath = Path.Combine(codeGenerationFolderPath, CustomDrawerAttributeFileName);

            Type[] decoratorAttributeTypes = DecoratorAndDrawerDatabase.GetAllDecoratorAttributeTypes();
            Type[] drawerAttributeTypes = DecoratorAndDrawerDatabase.GetAllDrawerAttributeTypes();
            List<Type> attributeTypes = new List<Type>();
            attributeTypes.AddRange(decoratorAttributeTypes);
            attributeTypes.AddRange(drawerAttributeTypes);
            
            
            StringBuilder fileContents = new StringBuilder();
            fileContents.Append("using UnityEditor;\n\n");
            fileContents.Append("namespace EditorPlus.Editor {\n\n");
            
            foreach (var attributeType in attributeTypes) {
                fileContents.Append($"\t[CustomPropertyDrawer(typeof({attributeType.FullName}))]\n");
            }
            fileContents.Append($"\tpublic partial class {nameof(CustomUnityDrawer)} {{ }}\n");
            fileContents.Append("}\n");
            
            File.WriteAllText(drawerAttributeFilePath, fileContents.ToString());
            AssetDatabase.Refresh();
        }
    }
}
