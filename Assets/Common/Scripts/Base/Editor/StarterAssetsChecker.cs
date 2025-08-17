using UnityEditor;
using UnityEngine;

namespace TinyShrine.Base.Views
{
    [InitializeOnLoad]
    public static class StarterAssetsChecker
    {
        // EditorPrefs keys
        const string PrefSuppress = "StarterAssetsPrompt.Suppress"; // true なら起動時に出さない
        const string PrefShownOnce = "StarterAssetsPrompt.ShownOnce"; // 初回だけ出す制御用

        // Asset Store (or Docs) URL — 必要に応じて差し替え
        const string StoreUrl = "https://assetstore.unity.com/packages/essentials/starter-assets-character-controllers-urp-267961";

        // よく使われる“存在確認の目印”。
        // 名前は環境により微差があるので複数ワードで OR 検索します。
        static readonly string[] Queries =
        {
        "t:Prefab PlayerCapsule", // Prefab名の候補
        "t:AnimatorController StarterAssetsThirdPerson", // Animator Controller名の候補
    };

        static StarterAssetsChecker()
        {
            // 起動時チェック。1フレーム遅延してから実行（Assembly読み込み直後を回避）
            EditorApplication.update += DelayedCheck;
        }

        static void DelayedCheck()
        {
            EditorApplication.update -= DelayedCheck;

            // 抑止中 or 既に一度出したなら何もしない（メニューからはいつでも手動実行可）
            if (EditorPrefs.GetBool(PrefSuppress, false)) return;

            // “初回だけ自動表示”したい場合はこれを使用
            if (EditorPrefs.GetBool(PrefShownOnce, false)) return;

            if (!HasStarterAssets())
            {
                ShowPromptDialog();
                EditorPrefs.SetBool(PrefShownOnce, true);
            }
        }

        [MenuItem("Tools/Project/Check Starter Assets…")]
        public static void CheckNow()
        {
            if (HasStarterAssets())
            {
                EditorUtility.DisplayDialog("Starter Assets",
                    "Starter Assets が見つかりました。セットアップ済みです。", "OK");
            }
            else
            {
                ShowPromptDialog();
            }
        }

        [MenuItem("Tools/Project/Starter Assets Prompt/Enable On Startup")]
        public static void EnableStartupPrompt()
        {
            EditorPrefs.SetBool(PrefSuppress, false);
            EditorUtility.DisplayDialog("Starter Assets Prompt",
                "起動時の確認ダイアログを有効にしました。", "OK");
        }

        [MenuItem("Tools/Project/Starter Assets Prompt/Disable On Startup")]
        public static void DisableStartupPrompt()
        {
            EditorPrefs.SetBool(PrefSuppress, true);
            EditorUtility.DisplayDialog("Starter Assets Prompt",
                "起動時の確認ダイアログを無効にしました。", "OK");
        }

        static bool HasStarterAssets()
        {
            // いずれかの“目印”がヒットすれば導入済みとみなす
            foreach (var q in Queries)
            {
                var guids = AssetDatabase.FindAssets(q);
                foreach (var guid in guids)
                {
                    var path = AssetDatabase.GUIDToAssetPath(guid);

                    // "StarterAssets" をパスに含んでいれば導入済みとみなす
                    if (path.Contains("Starter Assets"))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        static void ShowPromptDialog()
        {
            int choice = EditorUtility.DisplayDialogComplex(
                "Starter Assets not found",
                "このプロジェクトは Starter Assets: Character Controllers | URPを前提としています。\n" +
                "Asset Store からインポートしますか？",
                "Asset Store を開く",     // 0
                "閉じる",                 // 1
                "今後は表示しない"        // 2
            );

            if (choice == 0)
            {
                Application.OpenURL(StoreUrl);
            }
            else if (choice == 2)
            {
                EditorPrefs.SetBool(PrefSuppress, true);
                EditorUtility.DisplayDialog("Starter Assets Prompt",
                    "起動時の確認ダイアログは今後表示されません（Tools メニューから再実行可）。", "OK");
            }
        }
    }
}
