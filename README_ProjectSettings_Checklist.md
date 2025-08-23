# ProjectSettings Checklist

## Unity ProjectSettings 公開前 安全チェックリスト ✅

Unity プロジェクトの `ProjectSettings` を **Public リポジトリ** に含める際、  
不要な秘密情報や環境依存設定を含めないためのチェックリストです。

---

### 1. 機密情報の有無を確認

- [ ] **APIキーやシークレットキーが含まれていないか**
  - AdMob, Google API, AWS, Firebase, Unity Cloud Build など
  - 特に以下のファイルを確認：
    - `ProjectSettings/ProjectSettings.asset`
    - `ProjectSettings/EditorSettings.asset`
    - `ProjectSettings/UnityConnectSettings.asset`
- [ ] 外部サービスの接続情報（URL・ID・パスワード）がないか

---

### 2. 個人やローカル環境依存の情報を除外

- [ ] `ProjectSettings/EditorUserSettings.asset` を除外  
       → 個人のエディタ設定（レイアウト、開いているシーンなど）が含まれる
- [ ] 自分のPCのユーザ名やローカルパスが書かれていないか

---

### 3. 他社資産や非公開設定の確認

- [ ] 社内・他者から提供された素材やプラグインのライセンス違反がないか
- [ ] 未発表のプラットフォーム設定（例：新ハード用ビルド設定）が含まれていないか

---

### 4. Unity サービス関連設定の見直し

- [ ] Unity Analytics / Unity Cloud Build の **Organization ID / Project ID** を確認
- [ ] Unity Collaborate / Cloud Build 用のビルドターゲット設定に社内情報がないか

---

### 5. 公開前の最終確認

- [ ] `.gitignore` に以下を追加（推奨）

```gitignore
/ProjectSettings/EditorUserSettings.asset
```

- [ ] 公開前に git diff --cached で変更内容を目視チェック
- [ ] 公開後はリポジトリを clone して問題なく動くか確認

### 6. ライセンス・配布コンプライアンス詳細チェック（重要）🔍

#### 6.1 依存関係（UPM / サブモジュール）

- [ ] `Packages/manifest.json` に記載の各パッケージのライセンスを確認（MIT / Apache-2.0 / Unity EULA 等）
- [ ] 必要に応じて `ThirdPartyNotices.md` or `NOTICE` をリポジトリ直下に用意（依存ライセンスと著作権表示）
- [ ] Unity Asset Store 経由のパッケージは **再配布不可** → プロジェクトに **同梱しない**（入手手順をREADMEに記載）

#### 6.2 TextMesh Pro（TMP）まわり

- [ ] `Assets/TextMesh Pro` の **Essentials（Shaders / Sprites / Resources）を公開しない**  
       → 利用者に **Window → TextMeshPro → Import TMP Essentials** を案内
- [ ] 公開するフォントは **再配布可能ライセンス**（例：SIL OFL、Apache-2.0 など）か確認
- [ ] フォントの **ライセンス全文**（`LICENSE_OF_FONT.txt` など）を同じフォルダに同梱
- [ ] 必要に応じて **Reserved Font Name**（OFLの指定名）を侵害していないか確認

#### 6.3 アセット種別ごとの再配布可否

- 画像/アイコン
  - [ ] ライセンス（CC BY / CC0 / 商用可否 / 帰属表示の要否）を確認
  - [ ] CC BY の場合は **作者名・ライセンス・URL** を `ThirdPartyNotices.md` に明記
- 音源/BGM/効果音
  - [ ] **CC BY-NC（非商用）** や **有償サイトの再配布禁止**に該当しないか
  - [ ] 帰属表示が必要なら `ThirdPartyNotices.md` に記載
- フォント
  - [ ] ライセンス（OFL/Apache 等）と **改変・再配布条件**を確認
  - [ ] ライセンス本文を同梱（同フォルダ推奨）
- コードスニペット/ライブラリ
  - [ ] 取り込み元のライセンス（MIT / Apache-2.0 / GPL 等）を確認
  - [ ] **GPL 系**はプロジェクト全体のライセンスに影響するため要注意（避けるか分離）
- Unity Asset Store のアセット
  - [ ] **Asset Store EULA により再配布不可** → リポジトリに含めない
  - [ ] 利用者向けに「各自でAsset Storeから入手」手順をREADMEに記載

#### 6.4 ライセンス表記・ファイル整備

- [ ] リポジトリの **トップにプロジェクトの LICENSE** を配置（MIT/Apache等、選定済み）
- [ ] **`ThirdPartyNotices.md`**（または `NOTICE`）に、第三者素材の  
       **名称 / 作者 / ライセンス / 参照URL / 必要な表記** を一覧化
- [ ] バイナリアセット（フォント・画像・音源）には、可能なら **同ディレクトリに LICENSE ファイル** を同梱
- [ ] 商標・ブランドロゴを含む場合、**商標ガイドライン**に抵触しないか確認（再配布権限が必要）

#### 6.5 自動チェック（任意）

- [ ] 大容量のバイナリは LFS 管理（`.gitattributes`）
- [ ] `git grep` などでライセンス上気になる語をスキャン  
       例：`git grep -niE "copyright|license|by-nc"`
- [ ] CI で `LICENSE` / `NOTICE` の存在チェック（任意）

#### 6.6 README への追記（利用者向け）

- [ ] 「**初回セットアップ**」に以下を明記
  - `Window → TextMeshPro → Import TMP Essentials`
  - Asset Store 由来アセットは **各自入手** が必要である旨
- [ ] 再配布禁止アセットを含まないこと、第三者ライセンスの表記場所（`ThirdPartyNotices.md`）を明示

### 推奨運用

- 機密情報はコードや ProjectSettings に直書きせず、ScriptableObject や .env ファイルに分離して管理
- .env や \*.secret.asset は .gitignore に登録
- チーム内共有は Unity Version Control や暗号化ストレージを使用
