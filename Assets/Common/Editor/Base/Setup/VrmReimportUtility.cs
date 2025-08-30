using System.IO;
using UnityEditor;
using UnityEngine;

namespace TinyShrine.Base.Editor.Setup
{
    public static class VrmReimportUtility
    {
        /// <summary>
        /// プロジェクト内の全てのVRMファイルを再インポートする
        /// </summary>
        public static void ReimportAllVrms()
        {
            if (!FindAnyVrm(out string[] paths))
            {
                EditorUtility.DisplayDialog("VRMの再インポート", "Assets 内に .vrm が見つかりませんでした。", "OK");
                return;
            }

            try
            {
                int total = paths.Length;
                for (int i = 0; i < total; i++)
                {
                    string p = ToProjectRelative(paths[i]);
                    EditorUtility.DisplayProgressBar("Reimporting VRM", p, (float)i / total);

                    // UniVRM の Importer を再実行（同期・強制）
                    AssetDatabase.ImportAsset(
                        p,
                        ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport
                    );

                    // 念のため生成物も最新化（同フォルダに生成される Prefab/Materials など）
                    // → ImportAssetだけで十分なはずですが、必要に応じて追加の処理を入れられます。
                }

                AssetDatabase.SaveAssets();
                EditorUtility.DisplayDialog("完了", $".vrm を {total} 件、再インポートしました。", "OK");
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        // --- helpers ---
        private static bool FindAnyVrm(out string[] fullPaths)
        {
            // Assets 以下を再帰で走査して *.vrm を収集
            System.Collections.Generic.List<string> list = new System.Collections.Generic.List<string>();
            string root = Application.dataPath; // 絶対パス（…/Project/Assets）
            CollectVrmFiles(root, list);
            fullPaths = list.ToArray();
            return fullPaths.Length > 0;
        }

        private static void CollectVrmFiles(string dir, System.Collections.Generic.List<string> list)
        {
            // .vrm は .glb 互換ですが拡張子で判定
            string[] files = Directory.GetFiles(dir, "*.vrm", SearchOption.TopDirectoryOnly);
            list.AddRange(files);

            string[] dirs = Directory.GetDirectories(dir);
            foreach (string d in dirs)
            {
                // 隠し/パッケージ/Library をスキップ（Assets 内だけ走査）
                if (Path.GetFileName(d).StartsWith('.'))
                {
                    continue;
                }

                CollectVrmFiles(d, list);
            }
        }

        private static string ToProjectRelative(string absolute)
        {
            // …/Project/ の手前まで削って "Assets/…" 形式に
            string proj = Path.GetDirectoryName(Application.dataPath).Replace("\\", "/");
            return absolute.Replace("\\", "/").Replace(proj + "/", string.Empty);
        }
    }
}
