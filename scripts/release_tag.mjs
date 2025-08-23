import { spawn } from 'node:child_process';
import { access } from 'node:fs/promises';
import { constants } from 'node:fs';
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

function captureCommand(cmd, args) {
  return new Promise((resolvePromise, reject) => {
    let stdout = '';
    const child = spawn(cmd, args, {
      stdio: ['ignore', 'pipe', 'inherit'],
      shell: process.platform === 'win32',
    });
    child.stdout.setEncoding('utf8');
    child.stdout.on('data', (chunk) => {
      stdout += chunk;
    });
    child.on('close', (code) => {
      if (code === 0) resolvePromise(stdout);
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
    return false;
  }
}

async function main() {
  const tag = process.env.TAG || process.argv[2];
  if (!tag) {
    // eslint-disable-next-line no-console
    console.error(
      'Usage: TAG=vX.Y.Z npm run release:tag  (or: node scripts/release_tag.mjs vX.Y.Z)'
    );
    throw new Error('TAG parameter is required');
  }

  const branch = (await captureCommand('git', ['rev-parse', '--abbrev-ref', 'HEAD'])).trim();
  if (branch !== 'main') {
    // eslint-disable-next-line no-console
    console.error(`現在ブランチ '${branch}' です。main で実行してください。`);
    throw new Error('Not on main branch');
  }

  try {
    await runCommand('git', ['pull', '--ff-only']);
    await runCommand(process.execPath, [resolve('scripts/changelog_extract.mjs'), tag]);

    if (!(await fileExists('dist/RELEASE_BODY.md'))) {
      throw new Error('dist/RELEASE_BODY.md がありません。抽出に失敗しています。');
    }

    await runCommand('git', ['tag', '-a', tag, '-F', 'dist/RELEASE_BODY.md']);
    await runCommand('git', ['push', 'origin', tag]);

    // eslint-disable-next-line no-console
    console.log(`Annotated tag ${tag} pushed to origin. GitHub Actions が Release を作成します。`);
  } catch (error) {
    // eslint-disable-next-line no-console
    console.error(`Error: ${error.message}`);
    throw error;
  }
}

main().catch(() => {
  // 上でログ済み
  process.exitCode = 1;
});
