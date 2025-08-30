using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace TinyShrine.Base.Editor.Build
{
    public class BakeBeforeBuild : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            // 同期ベイクでビルド前に確定
            Lightmapping.Bake();
        }
    }
}
