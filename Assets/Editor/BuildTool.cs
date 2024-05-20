/****************************************************
    文件：BuildToos.cs
    作者：Y
    邮箱: 916111418@qq.com
    日期：#CreateTime#
    功能：Nothing
*****************************************************/

using System.IO;
using HybridCLR.Editor;
using UnityEditor;
using UnityEngine;

public class BuildTool
{
    private static readonly string AAResourcesPath = "AddressableResources/Dlls/";
    //把Hybrid热更程序集复制到AA包里热更
    [MenuItem("BuildToos/HyBridCLRCopy")]
    private static void HyBridCLRCopy()
    {
        var path = Application.dataPath +"/../"+ SettingsUtil.HotUpdateDllsRootOutputDir +"/"+ EditorUserBuildSettings.activeBuildTarget;
        if (!Directory.Exists(path))
        {
            Debug.LogError("不存在HyBridCLR热更打包路径：" +path);
            return;
        }
        DirectoryInfo directoryInfo = new DirectoryInfo(path);

        foreach (FileInfo info in directoryInfo.GetFiles())
        {
            if (info.Name.EndsWith(".dll"))
            {
                foreach (var s in SettingsUtil.HotUpdateAssemblyFilesExcludePreserved)
                {
                    if (info.Name.Equals(s))
                    {
                        info.CopyTo(Application.dataPath + "/" + AAResourcesPath + info.Name + ".bytes",true);
                        Debug.Log("Copy Success :" + info.Name);
                    }
                }
            }
        }
        AssetDatabase.Refresh();
 
    }
}