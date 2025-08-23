import { promises as fs } from 'node:fs';
import { resolve } from 'node:path';

const tag = process.env.TAG || process.argv[2];
async function main() {
  if (!tag) {
    throw new Error(
      'Usage: TAG=vX.Y.Z npm run release:extract  (or: node scripts/changelog_extract.mjs vX.Y.Z)'
    );
  }
  const text = await fs.readFile(resolve('CHANGELOG.md'), 'utf8');
  const esc = (s) => s.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
  const h = String.raw`##\s*(?:\[\s*)?${esc(tag)}(?:\s*\])?(?:\s*-\s*\d{4}-\d{2}-\d{2})?`;
  const headerRe = new RegExp(`^${h}\\s*$`, 'm');
  const start = text.search(headerRe);
  if (start === -1) {
    throw new Error(`CHANGELOG.md に見出しが見つかりません: ${tag}`);
  }

  const nextHeaderRe = /^##\s+/gm;
  nextHeaderRe.lastIndex = start + 2;
  const m = nextHeaderRe.exec(text);
  const end = m ? m.index : text.length;

  const section = text.slice(start, end).trim();
  await fs.mkdir('dist', { recursive: true });
  await fs.writeFile('dist/RELEASE_NOTES.md', section, 'utf8');
  const lines = section.split(/\r?\n/);
  const body = lines.length > 1 ? lines.slice(1).join('\n').trim() : '';
  await fs.writeFile('dist/RELEASE_BODY.md', `${body}\n`, 'utf8');
}

main().catch(() => {
  process.exitCode = 1;
});
