# CONTRIBUTING.md

準備（releaseブランチで）

```bash
git switch -c release/v1.4.0
# ← CHANGELOG.md を手で編集（Unreleased→[v1.4.0] - YYYY-MM-DD）
npm run release:prepare -- v1.4.0   # コミットだけ作る（タグは作らない）
git push -u origin HEAD             # PR を作成（release/v1.4.0 → main）
```

確定（PRがmainにマージされたら）

```bash
git switch main
git pull --ff-only
npm run release:tag -- v1.4.0       # 注釈付きタグ作成＆push → ActionsがRelease生成
```
