---
applyTo: '**/*.cs'
---

# C# / Unity coding guidelines

## 設計指針

- ファイル / クラスは単一責務
- public API は最小限に

## 命名規則

- Types / Namespaces / Properties / Methods = PascalCase（private 含む）
- Fields (private 含む) = camelCase（先頭アンダースコア禁止）
- 必要に応じて `this.` を付与してメンバー参照を明確化

## 型推論（var）の方針

- 右辺で型が**一目で明確**な場合のみ `var` を使う
  - 例: `var customer = new Customer();`、明示キャスト済み、LINQ/匿名型
- **不明瞭になる場合は明示型**を使う
  - 戻り値が推測しづらいメソッド、読み手が型を知る必要がある箇所
- Unity API 周り（`GetComponent`, `Find*`, `Resources.Load` など）や **公開/Serialize 対象**は **明示型**を原則
- **数値リテラル**は誤読防止のため明示型（`int/float/double` の混在時や演算含む式）
- `default` リテラルは可読性が下がる場合は `default(T)` を使用
- 変数名で補えない場合は `var` を避け、**型の可読性＞省記**を優先

## MonoBehaviour

- Update 系は最小限。イベント / コルーチン / Job / ECS / UniTask を優先
- `GetComponent` / `FindObjectOfType` はキャッシュして使用。ループ内で呼ばない

## GC / パフォーマンス

- GC 割当を抑制
  - StringBuilder / ArrayPool の活用
  - LINQ はホットパスで禁止

## 非同期処理

- UniTask を利用し、キャンセル / タイムアウトを必ず考慮
- `Forget()` の場合はログ出力を必ず伴わせる

## R3 (Reactive)

- **使い所**: 入力/UI イベントのストリーム化、複数非同期ソースの統合、キャンセル/タイムアウトの複雑制御
- **API 方針**: 外部公開は `IObservable<T>`、`Subject<T>` は内部専用（公開禁止）
- **ライフサイクル**: 購読は MonoBehaviour に紐付け、`IDisposable` を保持して `OnDisable/OnDestroy` で `Dispose`（`CompositeDisposable` 推奨）
- **スレッド**: Unity API 前にメインスレッドへマーシャリング（`ObserveOn` 等）
- **品質/性能**: 入力スパムは `Throttle/Debounce` 等で制御。高コストストリームは `Share/Publish` で共有し副作用の重複を防止
- **エラー**: `OnError` を握り潰さない（必ずロギング/通知）。リトライは一時エラーに限定し、指数バックオフ等を併用
- UniTask との橋渡しは必要時のみ（乱用しない）

## 例外 / エラー処理

- 例外はコントロールフローに使用しない
- Try パターン / Result 型を活用する

## シリアライズ

- `[SerializeField] private` を基本とする
- リネームは `[FormerlySerializedAs]` を利用

## VContainer

- コンポジションルートで依存解決
- `new` の散在は禁止し、DI で注入
