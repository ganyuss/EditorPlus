using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace EditorPlus.Editor {
    /// <summary>
    /// This class allows for automatic edition of the <see cref="CustomPropertyDrawer"/> attribute
    /// list associated to EditorPlus' <see cref="EditorPlusPropertyDrawer"/>. See this class for more information.
    /// </summary>
    public class PropertyAttributeSetter : ScriptableObject {

        public string GeneratedCodeFolderId = "EditorPlus.GeneratedCode.Editor";
        public string CustomDrawerAttributeFileName = "CustomDrawerAttributeDeclaration.cs";
        
        [HelpBox("All the namespaces in this list will not be taken in account when " +
                 "searching for PropertyAttributes that can be drawn using EditorPlus' " +
                 "CustomUnityDrawer. This can be useful when adding custom Drawers for the attributes " +
                 "to work with EditorPlus, while letting the regular drawer draw when there is no EditorPlus " +
                 "decorators. See the CustomPropertyDrawer documentation for more information.", HelpBoxType.Info)]
        [CustomSpace(15, 15)]
        public List<string> PropertyAttributeNamespaceBlackList;

        [Button]
        public void UpdatePropertyAttributes() {
            string codeGenerationFolderPath = FolderLocator.GetFolder(GeneratedCodeFolderId);
            string drawerAttributeFilePath = Path.Combine(codeGenerationFolderPath, CustomDrawerAttributeFileName);

            Type[] decoratorAttributeTypes = DecoratorAndDrawerDatabase.GetAllDecoratorAttributeTypes();
            Type[] drawerTypes = DecoratorAndDrawerDatabase.GetAllDrawerTypes();
            List<Type> drawnTypes = new List<Type>();
            drawnTypes.AddRange(decoratorAttributeTypes);
            drawnTypes.AddRange(drawerTypes);

            drawnTypes.RemoveAll(t => PropertyAttributeNamespaceBlackList.Contains(t.Namespace));
            
            StringBuilder fileContents = new StringBuilder();
            fileContents.Append("using UnityEditor;\n\n");
            fileContents.Append("namespace EditorPlus.Editor {\n\n");
            
            foreach (var drawnType in drawnTypes) {
                fileContents.Append($"\t[CustomPropertyDrawer(typeof({drawnType.FullName}))]\n");
            }
            fileContents.Append($"\tpublic partial class {nameof(EditorPlusPropertyDrawer)} {{ }}\n");
            fileContents.Append("}\n");
            
            File.WriteAllText(drawerAttributeFilePath, fileContents.ToString());
            AssetDatabase.Refresh();
        }
    }
}