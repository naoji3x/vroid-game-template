using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using TMPro;

namespace TinyShrine.Base.Editor
{
    public static class BulkAssignTMPFont_Limited
    {
        // 対象フォルダ（必要に応じて増減OK）
        private static readonly string[] SceneFolders = { "Assets/Scenes", "Assets/Common/Scenes" };
        private static readonly string[] PrefabFolders = { "Assets/Prefabs", "Assets/Common/Prefabs" };

        [MenuItem("Tools/TMP/Assign Default Font (Null Only) in Scenes+Prefabs")]
        public static void AssignNullOnly()
        {
            Run(assignAll: false);
        }

        [MenuItem("Tools/TMP/Assign Default Font (Force All) in Scenes+Prefabs")]
        public static void AssignForceAll()
        {
            Run(assignAll: true);
        }

        private static void Run(bool assignAll)
        {
            var font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Common/Fonts/MPLUS2-Medium SDF.asset");
            if (font == null)
            {
                EditorUtility.DisplayDialog(
                    "TMP Font 一括適用",
                    "指定されたフォント（Assets/Common/Fonts/MPLUS2-Medium SDF.asset）が見つかりません。",
                    "OK"
                );
                return;
            }

            try
            {
                AssetDatabase.StartAssetEditing();

                // ---- Prefab ----
                var prefabSearch = SceneOrPrefabSearchFolders(PrefabFolders);
                if (prefabSearch.Length > 0)
                {
                    var prefabGuids = AssetDatabase.FindAssets("t:Prefab", prefabSearch);
                    for (int i = 0; i < prefabGuids.Length; i++)
                    {
                        var path = AssetDatabase.GUIDToAssetPath(prefabGuids[i]);
                        var root = PrefabUtility.LoadPrefabContents(path);
                        bool changed = AssignInHierarchy(root, font, assignAll);
                        if (changed)
                        {
                            PrefabUtility.SaveAsPrefabAsset(root, path);
                        }
                        PrefabUtility.UnloadPrefabContents(root);

                        if (i % 25 == 0)
                            EditorUtility.DisplayProgressBar("TMP Font 適用中 (Prefabs)", path, (float)i / Mathf.Max(1, prefabGuids.Length));
                    }
                    EditorUtility.ClearProgressBar();
                }

                // ---- Scenes ----
                var sceneSearch = SceneOrPrefabSearchFolders(SceneFolders);
                if (sceneSearch.Length > 0)
                {
                    var sceneGuids = AssetDatabase.FindAssets("t:Scene", sceneSearch);
                    for (int i = 0; i < sceneGuids.Length; i++)
                    {
                        var scenePath = AssetDatabase.GUIDToAssetPath(sceneGuids[i]);
                        var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
                        bool changed = false;

                        foreach (var go in scene.GetRootGameObjects())
                            if (AssignInHierarchy(go, font, assignAll)) changed = true;

                        if (changed)
                        {
                            EditorSceneManager.MarkSceneDirty(scene);
                            EditorSceneManager.SaveScene(scene);
                        }

                        if (i % 5 == 0)
                            EditorUtility.DisplayProgressBar("TMP Font 適用中 (Scenes)", scenePath, (float)i / Mathf.Max(1, sceneGuids.Length));
                    }
                    EditorUtility.ClearProgressBar();
                }

                EditorUtility.DisplayDialog("完了",
                    $"一括適用が完了しました：{font.name}\n対象: {string.Join(", ", SceneFolders.Concat(PrefabFolders))}",
                    "OK");
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
                EditorUtility.ClearProgressBar();
            }
        }

        private static string[] SceneOrPrefabSearchFolders(string[] candidates)
            => candidates.Where(AssetDatabase.IsValidFolder).ToArray();

        // ヒエラルキー内の TextMeshProUGUI / TextMeshPro を置換
        private static bool AssignInHierarchy(GameObject root, TMP_FontAsset font, bool assignAll)
        {
            bool changed = false;

            var uiTexts = root.GetComponentsInChildren<TextMeshProUGUI>(true);
            foreach (var t in uiTexts)
            {
                if (ShouldReplace(t.font, assignAll))
                {
                    Undo.RecordObject(t, "Assign TMP Font");
                    t.font = font;
                    if (t.fontSharedMaterial == null) t.fontSharedMaterial = font.material;
                    EditorUtility.SetDirty(t);
                    changed = true;
                }
            }

            var worldTexts = root.GetComponentsInChildren<TextMeshPro>(true);
            foreach (var t in worldTexts)
            {
                if (ShouldReplace(t.font, assignAll))
                {
                    Undo.RecordObject(t, "Assign TMP Font");
                    t.font = font;
                    if (t.fontSharedMaterial == null) t.fontSharedMaterial = font.material;
                    EditorUtility.SetDirty(t);
                    changed = true;
                }
            }

            return changed;
        }

        private static bool ShouldReplace(TMP_FontAsset current, bool assignAll)
            => assignAll || current == null;
    }
}
