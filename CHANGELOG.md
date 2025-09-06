# Changelog

All notable changes to this project will be documented in this file.

The format is based on Keep a Changelog, and this project adheres to Semantic Versioning.

## [Unreleased]

### Added

### Changed

### Fixed

### Removed

### Deprecated

### Security

### Build

### Docs

> このファイルはプロジェクトの各リリースごとに更新してください。

## [v1.0.0] - 2025-09-06

### Added

1.0リリース

VRoidアバターを扱うUnity向けのオープンソース（MITライセンス）テンプレート:

- VRoidで作成した自分だけの3DキャラクターをUnity上に表示する
- キャラクターをキーボードやマウスで自由に操作する
- 目標地点までキャラクターを歩かせたり走らせたりする

また、以下のような時間のかかる作業を自動化・簡素化:

- 各種パッケージの導入やTextMesh Proによる日本語表示など、面倒なセットアップを自動化
- Unity Asset Storeのアセットなど、ライセンス上同梱できない素材も、手順に従って簡単に導入可能
- ソースコードはMITライセンス、同梱アセットは無料素材のみを使用し、誰でも安心して利用可能
- C#のフォーマッター、リンターの設定、コミット時に自動実行
- GitHub Actionsでのリリース自動化

様々なUnityのプロジェクトの雛形として、また、VRoidキャラクターを使ったゲーム開発の出発点として利用可能です。

## [v0.1.5] - 2025-08-31

### Added

- C#のフォーマッターの追加
- C#のリンターの追加

- ProjectSetupGuideのボタンを日本語に変更

### Build

- C#のフォーマッター、リンターをpre-commitに追加

### Docs

- CONTRIBUTING.mdの追加

## [v0.1.4] - 2025-08-24

### Added

- Recorder packageの追加

### Docs

- サンプルのスクリーンショットとTL;DRの文章の追加

## [v0.1.3] - 2025-08-24

### Fixed

- 初期設定アセットのディレクトリの修正、セットアップガイドの表示フラグの修正

## [v0.1.2] - 2025-08-24

### Added

- Appバージョンチェックの追加、archive名の変更

## [v0.1.1] - 2025-08-24

### Added

αリリース

- プロジェクト初期化
- Unity 6000.0 LTS / .NET Standard 2.1 対応
- Addressables, Input System, Cinemachine, LitMotion, Smart Addresser, UniTask, UI Effect,
  VContainer, VRM 1.0, R3 パッケージ導入
- テンプレート構成（Assets/Common, Scripts, Scenes, Settings など）
- Input System: `InputSystem_Actions.inputactions` 追加
- サンプルシーン・サンプルスクリプト追加
- エディタ拡張（Common/Editor/Base/Setup/TmpFontReassignUtility.cs など）
- README, DEVLOG, CHANGELOG、README_ProjectSettings_Checklist、ライセンスファイル追加
- nodeを追加して、CHANGELOGの自動更新やリリースタグ付けをスクリプト化
- プルリクエストの自動生成を追加
