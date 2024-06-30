/****************************************************
    文件：YooUpdate.cs
    作者：Y
    邮箱: 916111418@qq.com
    日期：#CreateTime#
    功能：Nothing
*****************************************************/

using System;
using System.Collections;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;
using YooAsset;

public class YooUpdate : MonoBehaviour
{
    private ResourcePackage _package;

    public EPlayMode PlayMode = EPlayMode.EditorSimulateMode;
    public string PackageName = "DefaultPackage";
    public EDefaultBuildPipeline BuildPipeline = EDefaultBuildPipeline.BuiltinBuildPipeline;

    private IEnumerator Start()
    {
        // yield return InitPackage(PackageName, BuildPipeline, PlayMode);
        yield return YooManger.Instance.InitPackage(PlayMode, PackageName, BuildPipeline.ToString());
        YooAssets.SetDefaultPackage(YooManger.Instance.GetPackage(PackageName));
        yield return LoadCube();
    }
    private IEnumerator LoadCube()
    {
        var cubeHandle = YooAssets.LoadAssetAsync<GameObject>("Cube");
        yield return cubeHandle;
        cubeHandle.InstantiateSync();
        cubeHandle.Release();
    }
}