using UnityEditor;
using UnityEngine;
using System.Linq;

namespace TinyShrine.Base.Editor.Setup
{
    public static class VrmReimportUtility
    {
        /// <summary>
        /// プロジェクト内の全てのVRMファイルを再インポートする
        /// </summary>
        public static void ReimportAllVrms()
        {
            string[] vrmGuids = AssetDatabase.FindAssets("t:DefaultAsset")
                .Where(guid => AssetDatabase.GUIDToAssetPath(guid).EndsWith(".vrm"))
                .ToArray();

            if (vrmGuids.Length == 0)
            {
                Debug.Log("VRMファイルが見つかりませんでした。");
                return;
            }

            Debug.Log($"{vrmGuids.Length}個のVRMファイルの再インポートを開始します...");

            for (int i = 0; i < vrmGuids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(vrmGuids[i]);

                EditorUtility.DisplayProgressBar(
                    "VRM 再インポート中",
                    $"処理中: {System.IO.Path.GetFileName(path)}",
                    (float)i / vrmGuids.Length);

                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                Debug.Log($"再インポート完了: {path}");
            }

            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
            Debug.Log("全てのVRMファイルの再インポートが完了しました。");
        }
    }
}
