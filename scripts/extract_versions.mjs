import { readFile } from 'node:fs/promises';
import { resolve } from 'node:path';

async function readText(filePath) {
  return readFile(resolve(filePath), 'utf8');
}

async function getUnityEditorVersion() {
  const txt = await readText('ProjectSettings/ProjectVersion.txt');
  const match = txt.match(/m_EditorVersion(?:WithRevision)?:\s*([^\s()]+)/);
  if (!match) {
    throw new Error('Unity Editor version not found in ProjectVersion.txt');
  }
  return match[1];
}

async function getAppVersion() {
  const yamlContent = await readText('ProjectSettings/ProjectSettings.asset');
  const versionMatch = yamlContent.match(/^\s*bundleVersion:\s*"?([^\r\n"]+)"?\s*$/m);
  return versionMatch ? versionMatch[1].trim() : null;
}

async function main() {
  try {
    const unityVersion = await getUnityEditorVersion();
    const appVersion = await getAppVersion();

    if (!appVersion) {
      throw new Error('bundleVersion (PlayerSettings → Version) が見つかりません');
    }

    // eslint-disable-next-line no-console
    console.log(`UNITY_VERSION=${unityVersion}`);
    // eslint-disable-next-line no-console
    console.log(`APP_VERSION=${appVersion}`);
    // 仕様通り UNITY_VERSION と APP_VERSION のみ出力
  } catch (error) {
    // eslint-disable-next-line no-console
    console.error(`::error::${error.message}`);
    throw error;
  }
}

// Execute main function and handle errors
main().catch(() => {
  process.exitCode = 1;
});
