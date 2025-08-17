using System.IO;
using UnityEditor;
using UnityEngine;

namespace TinyShrine.Base.Editor
{
    [InitializeOnLoad]
    public static class VrmReimportUtility
    {
        // 起動時に一度だけ案内＋実行（オフにもできる）
        const string PrefAutoOffer = "VRMReimportUtility.AutoOffer";   // 自動提示ON/OFF
        const string PrefOfferedOnce = "VRMReimportUtility.OfferedOnce"; // 一回だけ

        static VrmReimportUtility()
        {
            // デフォルトは自動提示ON
            if (!EditorPrefs.HasKey(PrefAutoOffer)) EditorPrefs.SetBool(PrefAutoOffer, true);
            EditorApplication.update += DelayedOffer;
        }

        static void DelayedOffer()
        {
            EditorApplication.update -= DelayedOffer;
            if (!EditorPrefs.GetBool(PrefAutoOffer, true)) return;
            if (EditorPrefs.GetBool(PrefOfferedOnce, false)) return;

            var hasVrm = FindAnyVrm(out _);
            if (!hasVrm) return;

            var ok = EditorUtility.DisplayDialog(
                "VRMの再インポート",
                "クローン直後などで VRM が正しく表示されない場合、.vrm を一括再インポートすると解決します。\n\n" +
                "今すぐ実行しますか？（後でも Tools → VRM → Reimport All .vrm から実行できます）",
                "今すぐ実行", "あとで");
            if (ok) ReimportAllVrms();

            EditorPrefs.SetBool(PrefOfferedOnce, true);
        }

        [MenuItem("Tools/VRM/Reimport All .vrm")]
        public static void ReimportAllVrms()
        {
            if (!FindAnyVrm(out var paths))
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
                    AssetDatabase.ImportAsset(p,
                        ImportAssetOptions.ForceUpdate |
                        ImportAssetOptions.ForceSynchronousImport);

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

        [MenuItem("Tools/VRM/Auto Offer On Startup/Enable")]
        public static void EnableAutoOffer()
        {
            EditorPrefs.SetBool(PrefAutoOffer, true);
            EditorPrefs.SetBool(PrefOfferedOnce, false);
            EditorUtility.DisplayDialog("VRM Reimport", "次回起動時に再インポートの案内を表示します。", "OK");
        }

        [MenuItem("Tools/VRM/Auto Offer On Startup/Disable")]
        public static void DisableAutoOffer()
        {
            EditorPrefs.SetBool(PrefAutoOffer, false);
            EditorUtility.DisplayDialog("VRM Reimport", "起動時の案内を無効にしました。手動で実行してください。", "OK");
        }

        // --- helpers ---
        static bool FindAnyVrm(out string[] fullPaths)
        {
            // Assets 以下を再帰で走査して *.vrm を収集
            var list = new System.Collections.Generic.List<string>();
            var root = Application.dataPath; // 絶対パス（…/Project/Assets）
            CollectVrmFiles(root, list);
            fullPaths = list.ToArray();
            return fullPaths.Length > 0;
        }

        static void CollectVrmFiles(string dir, System.Collections.Generic.List<string> list)
        {
            // .vrm は .glb 互換ですが拡張子で判定
            var files = Directory.GetFiles(dir, "*.vrm", SearchOption.TopDirectoryOnly);
            list.AddRange(files);

            var dirs = Directory.GetDirectories(dir);
            foreach (var d in dirs)
            {
                // 隠し/パッケージ/Library をスキップ（Assets 内だけ走査）
                if (Path.GetFileName(d).StartsWith(".")) continue;
                CollectVrmFiles(d, list);
            }
        }

        static string ToProjectRelative(string absolute)
        {
            // …/Project/ の手前まで削って "Assets/…" 形式に
            var proj = Path.GetDirectoryName(Application.dataPath).Replace("\\", "/");
            return absolute.Replace("\\", "/").Replace(proj + "/", "");
        }
    }
}
