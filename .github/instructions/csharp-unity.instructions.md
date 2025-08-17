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
- 例外はコントロールフローに使用しない。Try パターン / Result 型を活用
- シリアライズ:
  - `[SerializeField] private` を基本とする
  - リネームは `[FormerlySerializedAs]` を利用
- VContainer:
  - コンポジションルートで依存解決
  - `new` の散在は禁止し、DI で注入
