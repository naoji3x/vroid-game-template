---
applyTo: '**'
---

# Code review checklist

- 命名規則に従っているか（PascalCase / camelCase、一貫性のあるアセット名）
- public API は最小限かつ責務が明確か
- 例外の扱いが適切か（コントロールフローで使っていないか）
- GC 割当を最小化しているか（Profiler で GC Alloc の監視）
- 非同期処理にキャンセル / タイムアウトが考慮されているか
- Addressables 参照は Release を忘れていないか
- VContainer / DI の責務が正しく分離されているか
- プラットフォーム依存コードが明示的に分岐されているか
- ログに PII（個人情報）が含まれていないか
- テストが追加されているか（特に public API に対応したテスト）
- シーン遷移やリソース破棄でメモリリークを残していないか
- R3（Reactive）:
  - 用途の妥当性：UniTask で済む箇所に R3 を使っていないか（“要所使い”になっているか）
  - API 露出：`Subject<T>` を公開していないか／公開側は `IObservable<T>` になっているか
  - ライフサイクル：`IDisposable` を保持し、`CompositeDisposable` で確実に `Dispose` しているか（`OnDisable/OnDestroy` で解放）
  - スレッド：Unity API 直前にメインスレッドへマーシャリングしているか
  - 性能：高コストなストリームを毎回生成していないか／副作用の二重実行を `Share/Publish` 等で防いでいるか
  - 可読性：オペレーター合成が過剰・ネスト深すぎになっていないか（分割・命名で意図が追えるか）
  - 入力スパム対策：`Throttle`/`Debounce`/`Buffer` 等で制御しているか（Subscribe 側が重くなっていないか）
  - エラー方針：`OnError` を握り潰していないか／リトライは一時エラーに限定しバックオフ等を併用しているか
  - ブリッジ乱用：UniTask ↔ R3 の相互変換を必要最小限に留めているか
  - テスト：Observable の完了/エラー/キャンセル、Dispose の検証が含まれているか
