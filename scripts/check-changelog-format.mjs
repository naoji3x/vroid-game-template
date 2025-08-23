import { readFile, access } from 'node:fs/promises';
import { constants } from 'node:fs';

/**
 * CHANGELOG.mdの書式チェック
 * Keep a Changelog準拠のリリース見出し形式をチェック
 */

const CHANGELOG_PATH = './CHANGELOG.md';

// リリース見出しの正規表現: ## [vX.Y.Z] - YYYY-MM-DD
const RELEASE_HEADING_REGEX = /^## \[v(\d+)\.(\d+)\.(\d+)\] - (\d{4})-(\d{2})-(\d{2})$/;

// Unreleasedセクションの正規表現
const UNRELEASED_REGEX = /^## \[Unreleased\]$/;

async function checkChangelogFormat() {
  try {
    await access(CHANGELOG_PATH, constants.F_OK);
    // eslint-disable-next-line no-unused-vars
  } catch (err) {
    throw new Error(`${CHANGELOG_PATH} が見つかりません`);
  }

  const content = await readFile(CHANGELOG_PATH, 'utf8');
  const lines = content.split('\n');

  const errors = [];

  lines.forEach((line, index) => {
    const lineNumber = index + 1;

    // レベル2見出しのみチェック
    if (line.startsWith('## ')) {
      // [Unreleased]は除外
      if (UNRELEASED_REGEX.test(line)) {
        return;
      }

      // リリース見出しの書式チェック
      if (!RELEASE_HEADING_REGEX.test(line)) {
        errors.push({
          line: lineNumber,
          content: line,
          message: 'リリース見出しは "## [vX.Y.Z] - YYYY-MM-DD" 形式である必要があります',
        });
      } else {
        // 日付の妥当性チェック
        const match = line.match(RELEASE_HEADING_REGEX);
        if (match) {
          const [, , , , year, month, day] = match;
          const date = new Date(year, month - 1, day);

          if (
            date.getFullYear() !== Number(year) ||
            date.getMonth() !== Number(month) - 1 ||
            date.getDate() !== Number(day)
          ) {
            errors.push({
              line: lineNumber,
              content: line,
              message: '無効な日付です',
            });
          }
        }
      }
    }
  });

  if (errors.length > 0) {
    // eslint-disable-next-line no-console
    console.error('❌ CHANGELOG.md書式エラー:');
    errors.forEach((error) => {
      // eslint-disable-next-line no-console
      console.error(`  Line ${error.line}: ${error.message}`);
      // eslint-disable-next-line no-console
      console.error(`    ${error.content}`);
    });
    throw new Error('CHANGELOG.md書式エラーが見つかりました');
  }

  // eslint-disable-next-line no-console
  console.log('✅ CHANGELOG.md書式チェック: OK');
}

checkChangelogFormat().catch(() => {
  process.exitCode = 1;
});
