# Developer Log

## TODO

- [x] CHANGELOG.mdの作成。
- [ ] バージョンタグをv*.*.\*に変更。
- [ ] リリースのタイトルとアセット名にUnityのバージョンを記載。
- [ ] Unityに設定したバージョン情報からv*.*.\*を取得。

## TextMeshProの設定

- [Unityのフォント追加方法・TextMeshProを日本語で使う方法](https://yurinchi2525.com/2023011howtoaddtextmeshpro/)
- [[Unity] Text Mesh Proで日本語を表示する方法](https://zenn.dev/kametani256/articles/63c083ab318136)

サイズが大きくなるので、Atlas Resolution は4096x4096にしておく。

## Packageのインストール

次に各種パッケージのインポート。GUIは面倒なのでコマンドラインからインストール。

```bash
npm i -g openupm-cli # 先ずはopenupm-cliのインストール
openupm add com.cysharp.unitask # unitaskのインストール
openupm add jp.hadashikick.vcontainer # vcontainerのインストール
openupm add com.coffee.ui-effect # UIEffectのインストール
openupm add com.github-glitchenzo.nugetforunity # NuGet for Unityのインストール
```

- UIEffectは[こちら](https://anogame.net/unity-oss-uieffect/)
- localizationはPackage Managerからインストール。参考情報は[こちら](https://xrdnk.hateblo.jp/entry/2021/11/26/090000)。
- LitMotionはPackages/manifest.jsonに以下を追加。

```json
{
  "dependencies": {
    "com.annulusgames.lit-motion": "https://github.com/annulusgames/LitMotion.git?path=src/LitMotion/Assets/LitMotion"
  }
}
```

- SmartAddresserはPackages/manifest.jsonに以下を追加。

```json
{
  "dependencies": {
    "jp.co.cyberagent.smartaddresser": "https://github.com/CyberAgentGameEntertainment/SmartAddresser.git?path=/Assets/SmartAddresser"
  }
}
```

> Smart Addresserをインストールする場合は、Unity RegistryからAddressables（必要に応じてAddressables for Android）をインストールして下さい。

- R3はNuGetForUnityからインストール。UnityのメニューからNuGet > Manage NuGet Packagesを選択し、検索ボックスに「R3」と入力してインストール。

## .gitignore, .vscodeの設定

[こちら](https://github.com/github/gitignore/blob/main/Unity.gitignore)を参考に.gitignoreを作成。また、以下を追加。

```gitignore
# Assets/Packages フォルダ（NuGetForUnityのPackageを保存するフォルダ）をgit管理から除外
/[Aa]ssets/[Pp]ackages/

# misc
.DS_Store
*.pem
```

.vscode/settings.jsonに以下を追加。

```json
    "editor.formatOnSave": true
```

UniVRMをインストール。インストールはgithubから。バージョンはv0.129.3として、manifest.jsonに以下を追加。

```json
{
  "dependencies": {
    "com.vrmc.gltf": "https://github.com/vrm-c/UniVRM.git?path=/Assets/UniGLTF#v0.129.3",
    "com.vrmc.vrm": "https://github.com/vrm-c/UniVRM.git?path=/Assets/VRM10#v0.129.3"
  }
}
```
