import { spawn } from 'node:child_process';
import { constants } from 'node:fs';
import { access, readFile } from 'node:fs/promises';
import { resolve } from 'node:path';

function runCommand(cmd, args) {
  return new Promise((resolvePromise, reject) => {
    const child = spawn(cmd, args, {
      stdio: 'inherit',
      shell: process.platform === 'win32',
    });
    child.on('close', (code) => {
      if (code === 0) resolvePromise();
      else reject(new Error(`Command failed: ${cmd} ${args.join(' ')} (exit code: ${code})`));
    });
    child.on('error', (error) => {
      reject(new Error(`Failed to start command: ${cmd} ${args.join(' ')} - ${error.message}`));
    });
  });
}

async function fileExists(pathLike) {
  try {
    await access(pathLike, constants.F_OK);
    return true;
  } catch (e) {
    if (e) {
      // ignore
    }
    return false; // 存在しない
  }
}

async function readText(filePath) {
  return readFile(resolve(filePath), 'utf8');
}

async function getAppVersion() {
  const yamlContent = await readText('ProjectSettings/ProjectSettings.asset');
  const versionMatch = yamlContent.match(/^\s*bundleVersion:\s*"?([^\r\n"]+)"?\s*$/m);
  return versionMatch ? versionMatch[1].trim() : null;
}

async function main() {
  const tag = process.env.TAG || process.argv[2];
  if (!tag) {
    // eslint-disable-next-line no-console
    console.error(
      'Usage: TAG=vX.Y.Z npm run release:prepare  (or: node scripts/release_prepare.mjs vX.Y.Z)'
    );
    throw new Error('TAG parameter is required');
  }

  try {
    // Unity アプリバージョンと指定されたタグの整合性チェック
    const appVersion = await getAppVersion();
    if (!appVersion) {
      throw new Error('bundleVersion (PlayerSettings → Version) が見つかりません');
    }

    // タグからバージョン部分を抽出 (v0.1.0 → 0.1.0)
    const tagVersion = tag.startsWith('v') ? tag.slice(1) : tag;
    if (appVersion !== tagVersion) {
      // eslint-disable-next-line no-console
      console.error(`バージョン不一致:`);
      // eslint-disable-next-line no-console
      console.error(`  Unity アプリバージョン: ${appVersion}`);
      // eslint-disable-next-line no-console
      console.error(`  指定されたタグ: ${tag} (${tagVersion})`);
      throw new Error('Unity アプリバージョンとタグバージョンが一致しません');
    }

    // eslint-disable-next-line no-console
    console.log(`バージョン確認: Unity アプリ ${appVersion} = タグ ${tag} ✓`);

    await runCommand(process.execPath, [resolve('scripts/changelog_extract.mjs'), tag]);

    if (!(await fileExists('dist/RELEASE_BODY.md'))) {
      throw new Error('dist/RELEASE_BODY.md がありません。抽出に失敗しています。');
    }

    await runCommand('git', ['add', 'CHANGELOG.md', 'ProjectSettings/ProjectSettings.asset']);
    const releaseBodyRaw = await readFile('dist/RELEASE_BODY.md', 'utf8');
    const releaseBody = releaseBodyRaw.trim();
    // -m と -F を同時指定できないため、タイトルと本文を -m 2 回で渡す
    await runCommand('git', ['commit', '-m', `chore(release): ${tag}`, '-m', releaseBody]);

    // eslint-disable-next-line no-console
    console.log(`Prepared release commit for ${tag}.`);
    // eslint-disable-next-line no-console
    console.log('Next steps:');
    // eslint-disable-next-line no-console
    console.log('  git push -u origin HEAD   # ブランチをリモートへ');
    // eslint-disable-next-line no-console
    console.log(`  # その後 GitHub で PR を作成（release/${tag} -> main）`);
  } catch (error) {
    // eslint-disable-next-line no-console
    console.error(`Error: ${error.message}`);
    throw error;
  }
}

main().catch(() => {
  process.exitCode = 1;
});
