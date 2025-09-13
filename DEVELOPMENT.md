# VRoidゲーム開発ガイド

このガイドでは、VRoidキャラクターを使ったゲームを作るためのテンプレートプロジェクトについて説明します。初心者の方でも分かりやすいように、順を追って解説していきます。

## 📋 目次

1. [📖 このプロジェクトについて](#-このプロジェクトについて)
2. [🏗️ システムの仕組み](#️-システムの仕組み)
3. [🚀 使い方](#-使い方)
4. [🔄 データの流れ](#-データの流れ)
5. [🧩 コンポーネントの詳細](#-コンポーネントの詳細)
6. [📝 コードの書き方](#-コードの書き方)

---

## 📖 このプロジェクトについて

このプロジェクトは、VRoidで作ったキャラクターをUnityゲームで動かすためのテンプレートです。VRoidキャラクターの操作、アニメーション、効果音の再生などが簡単にできるようになっています。

2つの主要な操作方法をサポートしています：

1. **自動移動キャラクター（NavMeshVRoidArmature）**: コンピューターが自動でキャラクターを動かします。目的地を設定すると、キャラクターが自分で道を見つけて移動します。
2. **手動操作キャラクター（ThirdPersonVRoidArmature）**: プレイヤーがキーボードやコントローラーでキャラクターを直接操作できます。

### 開発環境

| ソフトウェア | バージョン | 用途                 |
| ------------ | ---------- | -------------------- |
| Unity        | 6000.0 LTS | ゲーム開発エンジン   |
| Node.js      | 20.x以上   | 開発ツール・自動化   |
| .NET SDK     | 8.x以上    | C#コンパイル・テスト |
| Git          | 最新版     | バージョン管理       |

### 環境変数の設定（macOS）

macOSでの開発に必要な環境変数を`~/.zshrc`に設定してください。

#### .NET SDK関連の環境変数(dotnet@8をHomebrewでインストールした場合)

```bash
# ~/.zshrc に以下を追加
export DOTNET_ROOT="/opt/homebrew/opt/dotnet@8/libexec"
export PATH="/opt/homebrew/opt/dotnet@8/bin:$PATH"
export DOTNET_MSBUILD_SDK_RESOLVER_CLI_DIR="$DOTNET_ROOT"
```

#### 設定の反映

```bash
# 設定を反映
source ~/.zshrc

# 設定確認
dotnet --version
```

### できること

- **キャラクター制御**: NavMeshを使った自動移動
- **アニメーションイベント**: アニメーションに合わせたイベント処理
- **効果音システム**: 歩く音や着地音などの自動再生
- **アニメーション同期**: 複数のアニメーター間での状態共有

### フォルダ構成

```text
Assets/
├── TinyShrine/
│   ├── Core/
│   │   ├── Editor/          # エディタ拡張
│   │   └── Runtime/         # ランタイムコード
│   └── VRoid/
│       ├── Editor/          # VRoid関連エディタ拡張
│       ├── Runtime/         # VRoid関連ランタイム
│       │   └── Features/    # 機能別分類
│       │       ├── Agents/      # エージェント制御
│       │       │   ├── Controllers/  # 制御ロジック
│       │       │   └── Views/        # UI・表示関連
│       │       └── Cameras/     # カメラ制御
│       └── Sample/          # サンプルアセット・シーン
│           ├── Scenes/          # サンプルシーン
│           ├── Prefabs/         # サンプルPrefab
│           ├── Materials/       # サンプルマテリアル
│           └── Audio/           # サンプル音声
└── Scenes/                  # シーンファイル
```

---

## 🏗️ システムの仕組み

### キャラクターの構成パターン

#### 自動移動キャラクター（NavMeshVRoidArmature）

コンピューターが自動でキャラクターを動かすシステムです。以下のコンポーネントが付いています：

- **NavMeshAgentController**: 目的地への自動移動を担当
- **AnimationEventBridge**: アニメーションと効果音を連携

```text
NavMeshVRoidArmature（自動移動キャラクター）
├── NavMeshAgent（Unityの標準機能）
├── AudioSource（音声再生用）
├── NavMeshAgentController（移動制御）
├── AnimationEventBridge（音声連携）
└── Model
    └── VRoidキャラクター（子オブジェクト）
        ├── Animator（アニメーション制御）
        └── AnimationEventReceiver（自動で追加される）
```

#### 手動操作キャラクター（ThirdPersonVRoidArmature）

プレイヤーがキーボードやコントローラーで操作するシステムです。以下のコンポーネントが付いています：

- **AnimatorBridge**: アニメーションの状態を同期
- **AnimationEventBridge**: アニメーションと効果音を連携

```text
ThirdPersonVRoidArmature（手動操作キャラクター）
├── Animator（プレイヤー操作用）
├── AudioSource（音声再生用）
├── AnimatorBridge（アニメーション同期）
├── AnimationEventBridge（音声連携）
└── Model
    └── VRoidキャラクター（子オブジェクト）
        ├── Animator（VRoidキャラクター用）
        └── AnimationEventReceiver（自動で追加される）
```

---

## 🚀 使い方

### 自動移動キャラクター（NavMeshVRoidArmature）の使い方

#### 最初の設定

1. `Assets/TinyShrine/VRoid/Sample/Scenes/NavMeshSample`シーンをコピーして使用してください。
2. 必要に応じてHierarchyウィンドウで`SampleGirl`の名前を変更してください。
3. `SampleGirl/Model`にVRoidキャラクターが配置されているので、自分のVRoidキャラクターに置き換えてください。

#### 移動先の変更

移動先は`SampleGirl`に付いている`NavMeshAgentController`コンポーネントの`Goal`で設定されています。好きな場所に変更できます。

#### 地形の変更

1. 必要に応じて`Ground`オブジェクトを置き換えてください。
2. `Ground`にNavMeshが設定されています。置き換える場合はNavMesh Surfaceコンポーネントを追加してBakeしてください。

#### カメラの設定

1. VRoidキャラクターを自動で追いかける`FreeLook Camera`が設定されています。
2. SampleGirlを複製して使う場合は、`Cinemachine Camera`コンポーネントの`Tracking Target`を変更してください。
3. 画面に十字線が表示される場合は、Main Cameraの`Cinemachine Brain`コンポーネントの`Show Camera Frustum`をオフにしてください。

### 手動操作キャラクター（ThirdPersonVRoidArmature）の使い方

#### 最初の設定

1. `Assets/TinyShrine/VRoid/Sample/Scenes/ThirdPersonSample`シーンをコピーして使用してください。
2. 必要に応じてHierarchyウィンドウで`SampleGirl`の名前を変更してください。
3. `SampleGirl/Model`にVRoidキャラクターが配置されているので、自分のVRoidキャラクターに置き換えてください。

#### 地形の変更

必要に応じて`Ground`オブジェクトを置き換えてください。

#### カメラの設定

1. VRoidキャラクターを自動で追いかける`FreeLook Camera`が設定されています。
2. SampleGirlを複製して使う場合は、`Cinemachine Camera`コンポーネントの`Tracking Target`を変更してください。
3. 画面に十字線が表示される場合は、Main Cameraの`Cinemachine Brain`コンポーネントの`Show Camera Frustum`をオフにしてください。

---

## 🔄 データの流れ

### 自動移動キャラクターでのデータの流れ

1. **移動制御**: NavMeshAgentController → NavMeshAgent → キャラクターの位置更新
2. **アニメーション同期**: NavMeshAgentの速度 → Animatorのパラメータ更新
3. **イベント処理**: アニメーションイベント → AnimationEventReceiver → UnityEvents → 効果音再生

### 手動操作キャラクターでのデータの流れ

1. **入力処理**: プレイヤー入力 → 親Animatorのパラメータ更新
2. **パラメータ同期**: 親Animator → AnimatorBridge → VRoidキャラクターのAnimator
3. **イベント処理**: アニメーションイベント → AnimationEventReceiver → UnityEvents → 効果音再生

---

## 🧩 コンポーネントの詳細

### AnimationEventBridge

**役割**: VRoidキャラクターのアニメーションに合わせて効果音を再生するコンポーネント

**どこに付ける**: 自動移動キャラクター（NavMeshVRoidArmature）、手動操作キャラクター（ThirdPersonVRoidArmature）

**できること**:

- 子オブジェクトのAnimatorを自動で見つける
- AnimationEventReceiverを自動でセットアップ
- 効果音を自動で再生
- 音量を個別に調整

**設定項目**:

- `footstepSound`: 足音のファイル（AudioClip）
- `landSound`: 着地音のファイル（AudioClip）
- `footstepVolume`: 足音の音量（0.0〜1.0）
- `landVolume`: 着地音の音量（0.0〜1.0）

### NavMeshAgentController

**役割**: NavMeshAgentを使ってVRoidキャラクターを自動移動させるコンポーネント

**どこに付ける**: 自動移動キャラクター（NavMeshVRoidArmature）

**できること**:

- NavMeshを使った経路探索と移動
- アニメーションパラメータの自動更新
- 目的地への到達判定と移動停止
- Root Motionを無効化して位置制御

**設定項目**:

- `goal`: 移動先のGameObject
- `moveOnStart`: 開始時に自動移動するかどうか
- `arriveThreshold`: 到達と判定する距離
- `animatorController`: VRoidキャラクター用AnimatorController
- `speedParam`: 移動速度パラメータ名
- `motionSpeedParam`: モーション速度パラメータ名

### AnimatorBridge

**役割**: 親のAnimatorから子のAnimatorへアニメーションパラメータをコピーするコンポーネント

**どこに付ける**: 手動操作キャラクター（ThirdPersonVRoidArmature）

**できること**:

- 親子Animator間のパラメータ同期
- VRoidキャラクター用AnimatorControllerの自動設定
- リアルタイムでのパラメータ転送

**同期するパラメータ**:

- `Speed`: 移動速度（数値）
- `MotionSpeed`: モーション速度（数値）
- `Grounded`: 地面に接しているか（真偽値）
- `Jump`: ジャンプ中か（真偽値）
- `FreeFall`: 落下中か（真偽値）

### AnimationEventReceiver

**役割**: アニメーションイベントを受信してUnityEventで配信するコンポーネント。AnimationEventBridgeが自動でセットアップします。

**どこに付ける**: VRoidキャラクターのGameObject（自動で配置される）

**できること**:

- アニメーションからのイベント受信
- UnityEventによる柔軟なイベント配信

**使い方**:

親オブジェクトに付いているAnimationEventBridgeが自動でセットアップするので、特に設定は不要です。

---

## 📝 コードの書き方

### 名前の付け方

#### C# コードの命名ルール

このプロジェクトでは統一された命名ルールを使っています：

- **クラス名**: 最初が大文字 - `AnimationEventReceiver`, `NavMeshAgentController`
- **メソッド名**: 最初が大文字 - `MoveTo()`, `OnFootstep()`
- **プロパティ名**: 最初が大文字 - `OnFootstepEvent`, `TargetAnimator`
- **フィールド名**: 最初が小文字 - `targetAnimator`, `speedHash`
- **パラメータ名**: 最初が小文字 - `worldPos`, `evt`
- **定数**: 最初が大文字 - `DefaultSpeed`, `MaxDistance`

#### ファイル名の付け方

- **すべて最初が大文字**（スペース・アンダースコア・ハイフンは使わない）
- **コード（クラス名等）と同じ名前にする**
- **種類（Prefab / Scene / Material など）は名前に含めない**（Unityが自動で区別するため）

良い例：

- `EnemyGoblin.prefab`（❌ `EnemyGoblinPrefab.prefab`）
- `MainMenu.unity`（❌ `MainMenuScene.unity`）
- `WaterSurface.mat`（❌ `WaterSurfaceMaterial.mat`）
- `InventoryConfig.asset`（✅ 設定の説明は含めてもOK）

### コメントスタイル

#### XML ドキュメントコメント

すべてのpublicメンバーには XML ドキュメントコメントを必須とします：

```csharp
/// <summary>
/// アニメーションイベントを受信してUnityEventで配信するコンポーネント。
/// Animatorコンポーネントと同じGameObjectに配置され、アニメーションから送信される
/// AnimationEventをUnityEventとして外部に通知します。
/// </summary>
public class AnimationEventReceiver : MonoBehaviour
{
    /// <summary>
    /// Gets 足音アニメーションイベント発生時に呼び出されるUnityEvent。
    /// 外部コンポーネントがこのイベントを購読して音声再生等の処理を実行できます。
    /// </summary>
    public UnityEvent<AnimationEvent> OnFootstepEvent { get; } = new();

    /// <summary>
    /// 足音アニメーションイベントの受信メソッド。
    /// アニメーション内のAnimationEventから呼び出され、対応するUnityEventを発火します。
    /// </summary>
    /// <param name="evt">アニメーションイベントの詳細情報</param>
    public void OnFootstep(AnimationEvent evt) => OnFootstepEvent.Invoke(evt);
}
```

#### インラインコメント

複雑なロジックには説明的なコメントを追加：

```csharp
// sqrMagnitudeを使用してsqrt計算を回避し、0との比較で最適化
var speed = agent.velocity.sqrMagnitude > 0 ? agent.velocity.magnitude : 0f;

// pathPendingが false = 経路計算完了
// remainingDistanceは経路が無い時に Mathf.Infinity になるため hasPath もチェック
if (agent.hasPath && agent.remainingDistance <= Mathf.Max(agent.stoppingDistance, arriveThreshold))
{
    arrived = true;
    agent.isStopped = true;
}
```

### Lint/Formatter

#### EditorConfig 設定

プロジェクトでは統一されたコードスタイルのため `.editorconfig` を使用：

```editorconfig
# C# files
[*.cs]
indent_size = 4
charset = utf-8
end_of_line = lf
insert_final_newline = true
trim_trailing_whitespace = true
max_line_length = 120
```

#### StyleCop / Roslyn Analyzers

コード品質維持のため以下のルールを適用：

- **Performance**: エラーレベル（最優先）
- **Security**: エラーレベル
- **Usage**: エラーレベル
- **Design**: 警告レベル
- **Style**: CSharpier に委譲（基本オフ）

```editorconfig
# 性能系を最優先で厳格（Error）
dotnet_analyzer_diagnostic.category-Performance.severity = error

# セキュリティ/使用法も厳しめ
dotnet_analyzer_diagnostic.category-Security.severity = error
dotnet_analyzer_diagnostic.category-Usage.severity = error

# C# 9.0+ target-typed new expressions をサポート
dotnet_diagnostic.SA1000.severity = none
```

#### CSharpier

自動コードフォーマッターとして CSharpier を使用：

```bash
# フォーマット実行
npm run format:cs

# フォーマットチェック
npm run format:check
```

---

このガイドを参考に、VRoidキャラクターを使った楽しいゲームを作ってください。分からないことがあったら、プロジェクトのIssueやディスカッションで質問してくださいね。
