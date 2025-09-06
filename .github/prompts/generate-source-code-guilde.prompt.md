---
mode: 'agent'
description: 'プロジェクトのSource Code Guideのドキュメントを生成する。'
---

あなたはこのリポジトリの「ソースコードガイド」のドキュメントを作成する、熟練のプログラマです。以下を順番に実行して、ドキュメントを作成して下さい。

## 前提

- Unity は 6000.0 LTS 以降
- C# は .NET Standard 2.1 以降

## ドキュメントの出力先

docs/SourceCodeGuide.md

## 参照するコード

- Assets/TinyShrine/VRoid/Runtime/Features/Agents/Controllers/NavMeshAgentController.c
- Assets/TinyShrine/VRoid/Runtime/Features/Agents/Views/AnimationEventBridge.cs
- Assets/TinyShrine/VRoid/Runtime/Features/Agents/Views/AnimationEventReceiver.cs
- Assets/TinyShrine/VRoid/Runtime/Features/Agents/Views/AnimatorBridge.cs

## 作成するドキュメント

### SourceCodeGuide.md

- プロジェクト概要
- アーキテクチャ
  - NavMeshVRoidArmatureの場合: 以下のコンポーネントをアタッチしています。
    - NavMeshAgentController
    - AnimationEventBridge
  - ThirdPersonVRoidArmatureの場合: 以下のコンポーネントをアタッチしています。
    - AnimatorBridge
    - AnimationEventBridge
- 各コンポーネントの役割
- 使用方法
- コーディング規約
  - 命名規則
  - フォルダ構成
  - コメントスタイル
  - Lint/Formatter
- 開発フロー
  - ブランチ戦略
  - コミットメッセージ規約
  - プルリクエストの手順
