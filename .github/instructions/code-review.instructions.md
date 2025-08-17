---
applyTo: "**"
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
