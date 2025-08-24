# CONTRIBUTING.md

## リリースの手順

```bash
# 1) リリース準備（ローカル）
git switch -c release/v1.4.0
# CHANGELOG.md を手で編集（Unreleased→[v1.4.0] - YYYY-MM-DD）
npm run release:prepare -- v1.4.0   # ← コミットだけ作る（タグは切らない）

# 2) release ブランチを **明示的に** push（HEAD を使わない）
git push -u origin release/v1.4.0   # ← upstream 設定も同時に

# 3) PR が作成される（release/v1.4.0 → main）→ GitHub上でマージ

# 4) main に切り替えてタグを作成＆push（Actions 起動）
git switch main
git pull --ff-only                  # fast-forward マージしか許さない
npm run release:tag -- v1.4.0       # ← タグ作成＆ push（スクリプトが実施）
```
