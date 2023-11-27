//#define USE_BUILD_TOOLS

using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using System.Threading;

#if USE_BUILD_TOOLS
public class BuildConfig : Editor, IPreprocessBuildWithReport, IPostprocessBuildWithReport
{
    public int callbackOrder => 0;

    public static string OutPath;

    private static Build.Build build;

    //打包前处理
    public void OnPreprocessBuild(BuildReport report)
    {
        bool isReturn = false;
        bool isSuccess = false;
        build = new Build.Build(Application.dataPath, OutPath,
                (isSucc) =>
                {
                    Debug.LogFormat("OnPath succ = {0}", isSucc);
                    isReturn = true;
                    isSuccess = isSucc;
                },
                (isSucc) =>
                {
                    if (isSucc) 
                    {
                        build.Stop();
                    }
                    Debug.LogFormat("Restore succ = {0}", isSucc);
                });
        EditorUtility.DisplayCancelableProgressBar("BuildConfig", "配置中...", 0.3f);
        float pro = 0.3f;
        while (!isReturn)
        {
            Thread.Sleep(10);
            pro += 0.1f;
            EditorUtility.DisplayCancelableProgressBar("BuildConfig", "配置中...", pro);
        }
        if (isSuccess)
        {
            AssetDatabase.Refresh();
        }
    }
    //打包后处理
    public void OnPostprocessBuild(BuildReport report)
    {
        build.Restore();
    }
}
#elif USE_JENKINS_TOOLS
public static class BuildConfig
{
    public static string OutPath;
    public static void EndBuild()
    {
        bool isReturn = false;
        bool isSuccess = false;
        Build.Build build = null;
        build = new Build.Build(OutPath,
                (isSucc) =>
                {
                    isSuccess = isSucc;
                    isReturn = true;
                    Debug.LogFormat("Restore succ = {0}", isSucc);
                });
        EditorUtility.DisplayCancelableProgressBar("BuildConfig", "配置中...", 0.3f);
        float pro = 0.98f;
        while (!isReturn)
        {
            Thread.Sleep(10);
            pro += 0.001f;
            EditorUtility.DisplayCancelableProgressBar("BuildConfig", "配置中...", pro);
        }
        if (isSuccess) 
        {
            build.Restore();
            build.Stop();
        }
        EditorUtility.ClearProgressBar();
    }
}
#endif



