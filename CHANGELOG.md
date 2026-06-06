# Changelog

All notable changes to this project will be documented in this file.

The format is based on Keep a Changelog, and this project adheres to Semantic Versioning.

## [Unreleased]

### Added

- 音声認識機能の追加
- 音声合成機能の追加
- リップシンク機能の追加

### Changed

### Fixed

### Removed

### Deprecated

### Security

### Build

### Docs

## [v1.1.6] - 2026-06-06

### Build

- Unity 6.3LTS に更新

## [v1.1.5] - 2026-06-06

### Build

- checkout, setup-node, action-gh-releaseをバージョンアップしnode24に対応

## [v1.1.4] - 2026-06-06

### Build

- github actionsのnodeのバージョンを22 -> 24へ変更

## [v1.1.3] - 2026-06-06

### Fixed

- vscodeでエラーとなるため、global.jsonを書き換え。ローカルcにインストールされているdotnetを9.0.200以上に更新する必要があります。

## [v1.1.2] - 2025-10-25

### Fixed

- lefthookの設定でC#のフォーマッターとリンターの設定を汎用的なものに修正

## [v1.1.1] - 2025-10-12

### Build

- Unity 6000.0.58f2 LTS に更新
- Packageを最新に更新

## [v1.1.0] - 2025-10-04

### Added

VRoidとのチャット機能を追加（LLM for Unityを使ったローカル生成AIでのチャット）。

- サンプルシーンは Assets/TinyShrine/VRoid/Sample/Scenes/ChatSample.unity
- コードは Assets/TinyShrine/VRoid/Runtime/Features/Chats/にあります。

### Changed

ファイル名の大文字小文字の見直し。Vrm → VRM、Vroid → VRoid。

- Assets/TinyShrine/VRoid/Editor/Tools/VrmReimportUtility.cs
- Assets/TinyShrine/VRoid/Runtime/Features/Agents/AnimatorControllers/VroidLocomotion.overrideController
- Assets/TinyShrine/VRoid/Runtime/Features/Agents/AnimatorControllers/VroidLocomotionBridge.controller

## [v1.0.0] - 2025-09-06

### Added

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
