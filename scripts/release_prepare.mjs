import { spawn } from 'node:child_process';
import { constants } from 'node:fs';
import { access } from 'node:fs/promises';
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
    await runCommand(process.execPath, [resolve('scripts/changelog_extract.mjs'), tag]);

    if (!(await fileExists('dist/RELEASE_BODY.md'))) {
      throw new Error('dist/RELEASE_BODY.md がありません。抽出に失敗しています。');
    }

    await runCommand('git', ['add', 'CHANGELOG.md']);
    await runCommand('git', [
      'commit',
      '-m',
      `chore(release): ${tag}`,
      '-m',
      'dist/RELEASE_BODY.md',
    ]);

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
