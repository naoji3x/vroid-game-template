---
applyTo: '**'
---

# General project instructions

- リポジトリの標準命名規則を遵守する
  - PascalCase、スペース・アンダースコア・ハイフン禁止
  - コード（クラス名等）とアセット名を一致させる
- Kind（Prefab / Scene / Material 等 Unity が拡張子やアイコンで区別できるもの）は名前に含めない
- Variant や Animator Override Controller も名前に含めない（拡張子・フォルダで判別）
- ドメインを説明する単語（Config / Profile / Settings 等）は名前に含める
- フォルダ構成で種類を整理し、命名に余計な情報を入れない

# Workflow guidelines

- すべてのコードはバージョン管理下で共有可能な状態に保つ
- テスト（NUnit）は必ず `Tests/PlayMode` または `Tests/EditMode` に配置
- アセット参照は Addressables / Smart Addresser を通して一元管理
- プラットフォーム依存コードは `#if UNITY_IOS` / `#if UNITY_ANDROID` で明確化
