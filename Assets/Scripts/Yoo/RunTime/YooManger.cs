/****************************************************
    文件：YooManger.cs
    作者：Y
    邮箱: 916111418@qq.com
    日期：#CreateTime#
    功能：Nothing
*****************************************************/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YooAsset;

public class YooManger
{
    private static YooManger _instance;

    public static YooManger Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new YooManger();
            }

            return _instance;
        }
    }

    private Dictionary<string, ResourcePackage> _packageDic = new Dictionary<string, ResourcePackage>();


    public ResourcePackage GetPackage(string packageName)
    {
        _packageDic.TryGetValue(packageName, out var package);
        return package;
    }

    public bool AddPackage(ResourcePackage package)
    {
        return _packageDic.TryAdd(package.PackageName, package);
    }

    public IEnumerator InitPackage(EPlayMode playMode, string packageName, string buildPipeline)
    {
        var initPackage = new InitializePackage().InitPackage(playMode, packageName, buildPipeline);
        yield return initPackage;
    }
}