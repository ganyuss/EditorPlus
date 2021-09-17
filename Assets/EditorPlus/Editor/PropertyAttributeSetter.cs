using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace EditorPlus.Editor {
    public class PropertyAttributeSetter : ScriptableObject {

        public string GeneratedCodeFolderId = "EditorPlus.GeneratedCode.Editor";
        public string CustomDrawerAttributeFileName = "CustomDrawerAttributeDeclaration.cs";
        
        [CustomSpace(10, 10)]
        public List<string> PropertyAttributeNamespaceBlackList;

        [Button]
        public void UpdatePropertyAttributes() {
            string codeGenerationFolderPath = FolderLocator.GetFolder(GeneratedCodeFolderId);
            string drawerAttributeFilePath = Path.Combine(codeGenerationFolderPath, CustomDrawerAttributeFileName);

            Type[] decoratorAttributeTypes = DecoratorAndDrawerDatabase.GetAllDecoratorAttributeTypes();
            Type[] drawerAttributeTypes = DecoratorAndDrawerDatabase.GetAllDrawerAttributeTypes();
            List<Type> attributeTypes = new List<Type>();
            attributeTypes.AddRange(decoratorAttributeTypes);
            attributeTypes.AddRange(drawerAttributeTypes);

            attributeTypes.RemoveAll(t => PropertyAttributeNamespaceBlackList.Contains(t.Namespace));
            
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