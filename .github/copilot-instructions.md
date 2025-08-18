# Copilot Instructions

- Unity: 6000.0 LTS / .NET Standard 2.1
- Packages: Addressables, Input System, Cinemachine, LitMotion, Smart Addresser, UniTask, UI Effect, VContainer, VRM 1.0, R3
- Targets: iOS / Android (IL2CPP), Standalone

## Asset naming

- すべて **PascalCase**（スペース・アンダースコア・ハイフン禁止）
- **コード（クラス名等）と同名に揃える**
- Kind（Prefab / Scene / Material など **Unity が拡張子やアイコンで区別できるもの**）は **名前に含めない**
  - 例: `EnemyGoblin.prefab`, `MainMenu.unity`, `WaterSurface.mat`
- ドメインを説明する単語（Config / Profile / Settings など）は名前に含める
  - 例: `InventoryConfig.asset`, `LocalizationProfile.asset`
- Variant / Animator Override Controller なども名前に含めない（拡張子・フォルダで判別）
- Input System:
  - アセット名: `PlayerInputActions.inputactions`
  - 生成クラス名: `PlayerInputActions`（手動で一致させる）

## Unity patterns

- MonoBehaviour の Update 系は最小化。イベント / コルーチン / Job / ECS / UniTask を優先
- `GetComponent` / `FindObjectOfType` は開始時にキャッシュ。ループ内で呼ばない
- GC 割当を抑制（StringBuilder / ArrayPool、LINQ はホットパス禁止）
- 例外はコントロールフローに使わない。失敗は Try パターン / Result 型で返す
- シリアライズ: `[SerializeField] private` 基本、リネームは `[FormerlySerializedAs]`
- プラットフォーム依存は `#if UNITY_IOS` / `UNITY_ANDROID` で分離

## Packages usage

- Addressables: 参照の取得/Release を対に。シーン遷移で未解放を残さない
- VContainer: コンポジションルートで依存解決。MonoBehaviour に DI、`new` の散在禁止
- UniTask: メインスレッド制約に注意。キャンセル/タイムアウト必須、`Forget()`はログ付き
- Input System: Action ベース。Enable/Disable のライフサイクルを明示
- Cinemachine: カメラ制御は仮想カメラに集約。直接 Transform 操作しない
- LitMotion / UI Effect: ホットパスでの割当に注意（再利用/Pooling）
- Smart Addresser: アセット参照はアドレス化名を定数/設定で一元管理
- VRM 1.0: ローディングと破棄の責務を明確化（マテリアル/Animator のクリーンアップ）
- R3:
  - 購読は MonoBehaviour のライフサイクルに紐付け、`IDisposable` を必ず Dispose
  - Unity API 呼び出しはメインスレッドへマーシャリング
  - 入力スパムは `Throttle` / `Debounce` 等で制御し、Subscribe に重い処理を書かない
  - `OnError` は必ずロギングし、無視や握り潰しをしない

## Async policy

- **基本は UniTask**：単発の待機・ロード・シーン遷移・アニメ/演出待ち等は `async/await` + UniTask を使用
- **R3 は要所で**：
  - 入力/UI のイベント監視やストリーム合成
  - 複数非同期ソースの統合／キャンセル／タイムアウト制御
  - 公開 API は必要に応じて `IObservable<T>` を返す（過剰ラップ禁止）
  - `Subject<T>` を公開しない。`CompositeDisposable` で確実に破棄
- UniTask と R3 を無理に混在させない。用途で明確に選択

## Testing & QA

- テスト: NUnit。EditMode = `Tests/EditMode`、PlayMode = `Tests/PlayMode`
- public API ごとに最低 1 テスト
- 非同期はタイムアウト/キャンセルを検証
- パフォーマンス: Profiler で GC Alloc ゼロを目標（フレーム境界で確認）

## Build & CI

- コマンドラインビルド: `Tools/CI/Build.cs`
- ログ: レベル分離（Dev=詳細、本番=Warning 以上）。PII を出力しない
- Addressables のロード/Release を徹底（リーク厳禁）

## PR & Review

- 小さなスコープで PR 作成（1PR=1 目的）
- レビューでは: UniTask/R3 の使い分け・GC Alloc・エラー処理・プラットフォーム分岐・Addressables 解放を重点確認
