---
mode: 'agent'
description: 'プロジェクトのSetup Guideスクリプトを生成する。'
---

あなたはこのリポジトリの「セットアップ」のスクリプトを作成する、熟練のプログラマです。以下を順番に実行して、スクリプトを作成して下さい。

## 前提

- Unity は 6000.0 LTS 以降

## コードの出力先

Assets/Common/Scripts/Base/Editor/Setup

## 作成するコード

### ProjectSetupGuide.cs

プロジェクト初回起動時に以下の手順を表示する。２回目以降は表示しない。

```
   このプロジェクトの初期セットアップ手順：

   1. Starter Assets: Character Controllers | URP を Asset Store からダウンロードしてインストール
     メニュー『Tools → Project → 1. Download Starter Assets: Character Controllers | URP』でサイトを開く

   2. TextMesh Pro を初期化：Project Settings → TextMesh Pro で『Import TMP Essentials』
     メニュー『Tools → Project → 2. Import TextMesh Pro Essentials』を開く

   3. 文字がうまく表示されない場合：
     メニュー『Tools → Project → 3. Assign Default Font (Null Only) in Scenes+Prefabs』を実行

   4. Vroidがうまく表示されない/エラーになる場合：
     メニュー『Tools → Project → 4. Reimport Vrms』を実行

   この手順はメニュー『Tools → Project → 1. Show Setup Guide』からいつでも表示できます。
```

以下のメニューを作成し、それぞれの処理を実行する。

1. Tools/Project/1. Download Starter Assets: Character Controllers | URP
   https://assetstore.unity.com/packages/essentials/starter-assets-character-controllers-urp-267961 を開く。
2. Tools/Project/2. Import TextMesh Pro Essentials
   Project Settings の TextMesh Pro ページを開く
3. Tools/Project/3. Assign Default Font (Null Only) in Scenes+Prefabs
   TmpFontReassignUtility.AssignDefaultFont()を実行する。
   - 対象ディレクトリは Assets 以下の `Scenes` と `Prefabs` フォルダとする。
   - デフォルトフォントは、`Assets/Common/Fonts/MPLUS2-Medium SDF.asset`とする。
4. Tools/Project/4. Reimport Vrms
   VrmReimportUtility.ReimportAllVrms()を実行する。

### VrmReimportUtility.cs

ReimportAllVrms()メソッドで、プロジェクト内の全ての VRM ファイルを再インポートする。

### TmpFontReassignUtility.cs

AssignDefaultFont()メソッドで、

- 対象ディレクトリ以下の全ての Prefab と Scene ファイルを検索し、TextMeshProUGUI コンポーネントのフォントが null の場合に、デフォルトフォントを設定する。
- 対象ディレクトリは Assets 以下の `Scenes` と `Prefabs` フォルダとする（引数で指定）。
- デフォルトフォントは、`Assets/Common/Fonts/MPLUS2-Medium SDF.asset`とする（引数で指定）。
