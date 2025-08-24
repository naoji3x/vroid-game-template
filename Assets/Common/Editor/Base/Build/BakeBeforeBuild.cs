using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

public class BakeBeforeBuild : IPreprocessBuildWithReport
{
    public int callbackOrder => 0;
    public void OnPreprocessBuild(BuildReport report)
    {
        // 同期ベイクでビルド前に確定
        Lightmapping.Bake();
    }
}
