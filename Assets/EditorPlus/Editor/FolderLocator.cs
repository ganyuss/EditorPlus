using System.IO;
using UnityEditor;
using UnityEngine;

namespace EditorPlus.Editor {
    public class FolderLocator : ScriptableObject {

        public string FolderId;

        /// <summary>
        /// Tries to find the <see cref="FolderLocator"/> with the given ID and return the folder path
        /// </summary>
        /// <param name="id">The ID of the <see cref="FolderLocator"/> asset to look for</param>
        /// <returns>The path of the <see cref="FolderLocator"/>'s folder, or null if no <see cref="FolderLocator"/> has been found</returns>
        public static string GetFolder(string id) {

            FolderLocator[] locators = AssetDatabaseUtils.GetAll<FolderLocator>();

            foreach (var folderLocator in locators) {
                if (folderLocator.FolderId == id) {
                    return Path.GetFullPath(Path.GetDirectoryName(AssetDatabase.GetAssetPath(folderLocator)));
                }
            }

            return null;
        }

        // [OnInspectorGUI]
        public void OnGUI() {
            EditorGUILayout.HelpBox("This class is used as a marker for a folder.", MessageType.Info);
        }
    }
}
