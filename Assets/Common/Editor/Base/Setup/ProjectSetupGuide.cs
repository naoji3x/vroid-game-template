using UnityEditor;
using UnityEngine;
using System.IO;

namespace TinyShrine.Base.Editor.Setup
{
    [InitializeOnLoad]
    public static class ProjectSetupGuide
    {
        private static readonly string GuideShownFilePath = "ProjectSettings/ProjectSetupGuide.shown";

        static ProjectSetupGuide()
        {
            // 初回のみ表示（1フレーム待ってから表示）
            if (!IsGuideShown())
            {
                EditorApplication.update += ShowGuideDelayed;
            }
        }

        private static bool IsGuideShown()
        {
            return File.Exists(GuideShownFilePath);
        }

        private static void SetGuideShown()
        {
            // プロジェクト単位での設定保存
            string directory = Path.GetDirectoryName(GuideShownFilePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            File.WriteAllText(GuideShownFilePath, System.DateTime.Now.ToString());
        }

        private static void ShowGuideDelayed()
        {
            EditorApplication.update -= ShowGuideDelayed;
            ShowGuide();
            SetGuideShown();
        }

        [MenuItem("Tools/Project/1. Show Setup Guide")]
        public static void ShowSetupGuide()
        {
            ShowGuide();
        }

        [MenuItem("Tools/Project/Reset Setup Guide State")]
        public static void ResetSetupGuideState()
        {
            if (File.Exists(GuideShownFilePath))
            {
                File.Delete(GuideShownFilePath);
                Debug.Log("Setup Guide state has been reset. The guide will show again on next project load.");
            }
            else
            {
                Debug.Log("Setup Guide state was already reset.");
            }
        }

        public static void DownloadStarterAssets()
        {
            Application.OpenURL("https://assetstore.unity.com/packages/essentials/starter-assets-character-controllers-urp-267961");
        }

        public static void ImportTextMeshProEssentials()
        {
            SettingsService.OpenProjectSettings("Project/TextMesh Pro");
        }

        public static void AssignDefaultFont()
        {
            string[] targetDirectories = { "Assets/Common/Sample/Scenes", "Assets/Common/Sample/Prefabs", "Assets/Common/Runtime/Prefabs" };
            string defaultFontPath = "Assets/Common/Runtime/Fonts/MPLUS2-Medium SDF.asset";
            TmpFontReassignUtility.AssignDefaultFont(targetDirectories, defaultFontPath);
        }

        public static void ReimportVrms()
        {
            VrmReimportUtility.ReimportAllVrms();
        }

        private static void ShowGuide()
        {
            ProjectSetupGuideWindow.ShowWindow();
        }
    }

    public class ProjectSetupGuideWindow : EditorWindow
    {
        private Vector2 scrollPosition;

        public static void ShowWindow()
        {
            ProjectSetupGuideWindow window = GetWindow<ProjectSetupGuideWindow>(true, "プロジェクトセットアップガイド", true);
            window.minSize = new Vector2(600, 500);
            window.maxSize = new Vector2(800, 700);
        }

        private void OnGUI()
        {
            EditorGUILayout.Space(10);

            // タイトル
            GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 16,
                alignment = TextAnchor.MiddleCenter
            };
            EditorGUILayout.LabelField("このプロジェクトの初期セットアップ手順", titleStyle);

            EditorGUILayout.Space(10);

            // スクロール可能なエリア
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            DrawSetupStep("1", "Starter Assets のダウンロード",
                "Starter Assets: Character Controllers | URP を Asset Store からダウンロードしてインストールしてください。",
                "Download Starter Assets: Character Controllers | URP",
                () => ProjectSetupGuide.DownloadStarterAssets());

            DrawSetupStep("2", "TextMesh Pro の初期化",
                "Project Settings → TextMesh Pro で『Import TMP Essentials』をクリックしてください。",
                "Import TextMesh Pro Essentials",
                () => ProjectSetupGuide.ImportTextMeshProEssentials());

            DrawSetupStep("3", "フォントの設定",
                "文字がうまく表示されない場合に実行してください。\nnullフォントにデフォルトフォントを自動設定します。",
                "Assign Default Font (Null Only) in Scenes+Prefabs",
                () => ProjectSetupGuide.AssignDefaultFont());

            DrawSetupStep("4", "VRM の再インポート",
                "プロジェクト内の全VRMファイルを再インポートしてVRoidの表示を修正します。\n※VRoidがオレンジになる場合は再度実行してください。",
                "Reimport Vrms",
                () => ProjectSetupGuide.ReimportVrms());

            EditorGUILayout.EndScrollView();

            EditorGUILayout.Space(10);

            // フッター
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUIStyle footerStyle = new GUIStyle(EditorStyles.miniLabel)
            {
                alignment = TextAnchor.MiddleCenter
            };
            // ラベルを大きく・複数行で表示する
            GUIStyle footerLargeStyle = new GUIStyle(EditorStyles.wordWrappedLabel)
            {
                fontSize = 12,
                alignment = TextAnchor.MiddleCenter
            };
            EditorGUILayout.LabelField(
                "この手順は Tools → Project → 1. Show Setup Guide からいつでも表示できます",
                footerLargeStyle,
                GUILayout.ExpandWidth(true),
                GUILayout.MaxWidth(700)
            );
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(10);

            // 閉じるボタン
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("閉じる", GUILayout.Width(100), GUILayout.Height(30)))
            {
                Close();
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(10);
        }

        private void DrawSetupStep(string stepNumber, string title, string description, string menuName, System.Action menuAction)
        {
            EditorGUILayout.BeginVertical("box");

            // ステップタイトル
            GUIStyle stepStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 14
            };
            EditorGUILayout.LabelField($"ステップ {stepNumber}: {title}", stepStyle);

            EditorGUILayout.Space(5);

            // 説明文
            GUIStyle descStyle = new GUIStyle(EditorStyles.wordWrappedLabel);
            EditorGUILayout.LabelField(description, descStyle);

            EditorGUILayout.Space(5);

            // メニューボタン
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(menuName, GUILayout.Height(25)))
            {
                menuAction?.Invoke();
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(10);
        }
    }
}
