---
applyTo: "**/*.cs"
---

# C# / Unity coding guidelines

- 命名規則:
  - Types / Namespaces / Properties = PascalCase
  - Methods = camelCase
  - Fields (private 含む) = camelCase（先頭アンダースコア禁止）
  - 必要に応じて `this.` を付与してメンバー参照を明確化
- ファイル / クラスは単一責務。public API は最小限に
- MonoBehaviour:
  - Update 系は最小限。イベント / コルーチン / Job / ECS / UniTask を優先
  - `GetComponent` / `FindObjectOfType` はキャッシュして使用。ループ内で呼ばない
- GC 割当を抑制
  - StringBuilder / ArrayPool の活用
  - LINQ はホットパスで禁止
- 非同期処理:
  - UniTask を利用し、キャンセル / タイムアウトを必ず考慮
  - `Forget()` の場合はログ出力を必ず伴わせる
- R3 (Reactive):
  - 使い所: 入力/UI イベントのストリーム化、複数非同期ソースの統合、キャンセル/タイムアウトの複雑制御
  - API 方針: 外部公開は `IObservable<T>`、`Subject<T>` は内部専用（公開禁止）
  - ライフサイクル: 購読は MonoBehaviour に紐付け、`IDisposable` を保持して `OnDisable/OnDestroy` で `Dispose`（`CompositeDisposable` 推奨）
  - スレッド: Unity API 前にメインスレッドへマーシャリング（`ObserveOn` 等）
  - 品質/性能: 入力スパムは `Throttle/Debounce` 等で制御。高コストストリームは `Share/Publish` で共有し副作用の重複を防止
  - エラー: `OnError` を握り潰さない（必ずロギング/通知）。リトライは一時エラーに限定し、指数バックオフ等を併用
  - UniTask との橋渡しは必要時のみ（乱用しない）
- 例外はコントロールフローに使用しない。Try パターン / Result 型を活用
- シリアライズ:
  - `[SerializeField] private` を基本とする
  - リネームは `[FormerlySerializedAs]` を利用
- VContainer:
  - コンポジションルートで依存解決
  - `new` の散在は禁止し、DI で注入
