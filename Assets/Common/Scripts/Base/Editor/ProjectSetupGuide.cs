using System.Linq;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class ProjectSetupGuide
{
    // 表示制御
    private const string PrefSuppressed = "ProjectSetupGuide.Suppressed";
    private const string PrefShownOnce = "ProjectSetupGuide.ShownOnce";

    // 必要に応じて差し替え（URP版 Starter Assets）
    private const string StarterAssetsStoreUrl =
    "https://assetstore.unity.com/packages/essentials/starter-assets-character-controllers-urp-267961";


    // よく使われる“存在確認の目印”。
    // 名前は環境により微差があるので複数ワードで OR 検索します。
    static readonly string[] StarterAssetsQueries =
    {
    "t:Prefab PlayerCapsule", // Prefab名の候補
    "t:AnimatorController StarterAssetsThirdPerson", // Animator Controller名の候補
    };
    static readonly string[] TextMeshProQueries =
    {
    "t:TMP_FontAsset LiberationSans SDF",
    };

    static bool HasAssets(string[] queries, string assetName)
    {
        // いずれかの“目印”がヒットすれば導入済みとみなす
        foreach (var q in queries)
        {
            if (AssetDatabase.FindAssets(q)
                .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
                .Any(path => path.Contains(assetName)))
            {
                return true;
            }
        }
        return false;
    }

    // 起動時に一度だけガイドを表示
    static ProjectSetupGuide()
    {
        EditorApplication.update += DelayedShow;
    }

    private static void DelayedShow()
    {
        EditorApplication.update -= DelayedShow;

        if (EditorPrefs.GetBool(PrefSuppressed, false)) return;
        if (EditorPrefs.GetBool(PrefShownOnce, false)) return;

        ShowGuideDialog();
        EditorPrefs.SetBool(PrefShownOnce, true);
    }

    // メニューから再表示
    [MenuItem("Tools/Project/Show Setup Guide")]
    public static void ShowGuideDialog()
    {
        var hasStarterAssets = HasAssets(StarterAssetsQueries, "Starter Assets");
        var hasTextMeshPro = HasAssets(TextMeshProQueries, "TextMesh Pro");

        // 1段目：総合ガイド
        int choice = EditorUtility.DisplayDialogComplex(
            "Project Setup",
            (hasStarterAssets ? "✅ Starter Assets は導入済みです。" : "⚠️ Starter Assets は未導入です。") + "\n" +
            (hasTextMeshPro ? "✅ TextMesh Pro は導入済みです。" : "⚠️ TextMesh Pro は未導入です。") + "\n" +
            (hasStarterAssets && hasTextMeshPro ? "初期セットアップは完了しています。" : "以下に従って初期セットアップを完了させて下さい。") + "\n\n" +

            "このプロジェクトの初期セットアップ手順：\n\n" +
            "1) Starter Assets: Character Controllers | URP を Asset Store からダウンロードしてインストール\n" +
            "2) TextMesh Pro を初期化：Project Settings → TextMesh Pro で『Import TMP Essentials』\n" +
            "3) 文字がうまく表示されない場合：メニュー『Tools → TMP → Assign Default Font (Null Only) in Scenes+Prefabs』を実行\n\n" +
            "下のボタンから各手順に移動できます。",
            "2) TMP 初期化へ",
            "1) Starter Assets を開く",
            "閉じる");

        if (choice == 0)
        {
            OpenTmpProjectSettingsOrImport();
            AskFontAssignHint();
        }
        else if (choice == 1)
        {
            OpenStarterAssetsStore();
            // 追加案内：必要なら次の手順も促す
            AskTmpStep();
        }
        // choice == 2 → 閉じる
    }

    [MenuItem("Tools/Project/Setup Guide/Don’t show on startup")]
    public static void SuppressOnStartup() =>
        EditorPrefs.SetBool(PrefSuppressed, true);

    [MenuItem("Tools/Project/Setup Guide/Show on startup")]
    public static void EnableOnStartup()
    {
        EditorPrefs.SetBool(PrefSuppressed, false);
        EditorPrefs.SetBool(PrefShownOnce, false); // 次回起動で再表示
        EditorUtility.DisplayDialog("Setup Guide", "次回起動時にガイドを表示します。", "OK");
    }

    // ---- 手順アクション ----
    private static void OpenStarterAssetsStore()
    {
        Application.OpenURL(StarterAssetsStoreUrl);
        EditorUtility.DisplayDialog(
            "Starter Assets",
            "Asset Store のページを開きました。\n「Add to My Assets」→「Open in Unity」→ Package Manager から Import してください。",
            "OK");
    }

    private static void OpenTmpProjectSettingsOrImport()
    {
        // Project Settings の TextMesh Pro ページを開く（Unity 2021+）
        SettingsService.OpenProjectSettings("Project/TextMesh Pro");
    }

    private static void AskTmpStep()
    {
        int sel = EditorUtility.DisplayDialogComplex(
            "次の手順：TMP 初期化",
            "続けて TextMesh Pro の初期化を行いますか？\n" +
            "Project Settings → TextMesh Pro で『Import TMP Essentials』をクリックしてください。",
            "TMP 初期化へ",
            "あとで",
            "ガイドをもう表示しない");

        if (sel == 0)
        {
            OpenTmpProjectSettingsOrImport();
            AskFontAssignHint();
        }
        else if (sel == 2)
        {
            EditorPrefs.SetBool(PrefSuppressed, true);
            EditorUtility.DisplayDialog("Setup Guide", "起動時のガイド表示をオフにしました。Tools/Project/Show Setup Guide から再表示できます。", "OK");
        }
    }

    private static void AskFontAssignHint()
    {
        EditorUtility.DisplayDialog(
            "フォントが表示されない場合",
            "TextMesh Pro の文字が表示されない場合は、\n" +
            "メニューから『Tools → TMP → Assign Default Font (Null Only) in Scenes+Prefabs』を実行してください。\n" +
            "Fontが未設定の TextMeshPro コンポーネントに一括で割り当てます。",
            "OK");
    }
}
