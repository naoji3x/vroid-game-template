using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TinyShrine.Core.Editor.Tools
{
    public static class TMProFontReassignUtility
    {
        /// <summary>
        /// 指定されたディレクトリ内のPrefabとSceneファイルでTextMeshProUGUIのフォントがnullの場合にデフォルトフォントを設定する
        /// </summary>
        /// <param name="targetDirectories">対象ディレクトリパス配列.</param>
        /// <param name="defaultFontPath">デフォルトフォントのパス.</param>
        public static void AssignDefaultFont(string[] targetDirectories, string defaultFontPath)
        {
            // デフォルトフォントをロード
            TMP_FontAsset defaultFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(defaultFontPath);
            if (defaultFont == null)
            {
                Debug.LogError($"デフォルトフォントが見つかりません: {defaultFontPath}");
                return;
            }

            int totalProcessed = 0;
            int totalAssigned = 0;

            foreach (string directory in targetDirectories)
            {
                if (!AssetDatabase.IsValidFolder(directory))
                {
                    Debug.LogWarning($"ディレクトリが存在しません: {directory}");
                    continue;
                }

                // Prefabファイルを処理
                (int prefabProcessed, int prefabAssigned) = ProcessPrefabs(directory, defaultFont);
                totalProcessed += prefabProcessed;
                totalAssigned += prefabAssigned;

                // Sceneファイルを処理
                (int sceneProcessed, int sceneAssigned) = ProcessScenes(directory, defaultFont);
                totalProcessed += sceneProcessed;
                totalAssigned += sceneAssigned;
            }

            AssetDatabase.SaveAssets();
            Debug.Log(
                $"処理完了: {totalProcessed}個のファイルを処理し、{totalAssigned}個のTextMeshProUGUIにフォントを設定しました。"
            );
        }

        private static (int processed, int assigned) ProcessPrefabs(string directory, TMP_FontAsset defaultFont)
        {
            string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] { directory });
            int processed = 0;
            int assigned = 0;

            foreach (string guid in prefabGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

                if (prefab == null)
                {
                    continue;
                }

                bool modified = false;
                TextMeshProUGUI[] tmpComponents = prefab.GetComponentsInChildren<TextMeshProUGUI>(true);

                foreach (TextMeshProUGUI tmp in tmpComponents)
                {
                    if (tmp.font == null)
                    {
                        tmp.font = defaultFont;
                        modified = true;
                        assigned++;
                        Debug.Log($"Prefab: {path} の {tmp.name} にフォントを設定しました。");
                    }
                }

                if (modified)
                {
                    EditorUtility.SetDirty(prefab);
                }

                processed++;
            }

            return (processed, assigned);
        }

        private static (int Processed, int Assigned) ProcessScenes(string directory, TMP_FontAsset defaultFont)
        {
            string[] sceneGuids = AssetDatabase.FindAssets("t:Scene", new[] { directory });
            int processed = 0;
            int assigned = 0;

            // 現在のシーンを保存
            string currentScenePath = SceneManager.GetActiveScene().path;
            bool needsRestore = !string.IsNullOrEmpty(currentScenePath);

            foreach (string guid in sceneGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);

                // シーンを開く
                UnityEditor.SceneManagement.EditorSceneManager.OpenScene(path);

                bool modified = false;
                TextMeshProUGUI[] tmpComponents = Object.FindObjectsByType<TextMeshProUGUI>(FindObjectsSortMode.None);

                foreach (TextMeshProUGUI tmp in tmpComponents)
                {
                    if (tmp.font == null)
                    {
                        tmp.font = defaultFont;
                        modified = true;
                        assigned++;
                        Debug.Log($"Scene: {path} の {tmp.name} にフォントを設定しました。");
                    }
                }

                if (modified)
                {
                    UnityEditor.SceneManagement.EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
                }

                processed++;
            }

            // 元のシーンに戻す
            if (needsRestore)
            {
                UnityEditor.SceneManagement.EditorSceneManager.OpenScene(currentScenePath);
            }

            return (processed, assigned);
        }
    }
}
