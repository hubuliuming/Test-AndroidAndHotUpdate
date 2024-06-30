/****************************************************
    文件：CheckUpdate.cs
    作者：Y
    邮箱: 916111418@qq.com
    日期：#CreateTime#
    功能：Nothing
*****************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using YFramework.Extension;

public class CheckUpdate : MonoBehaviour
{
    [Serializable]
    private class DownLoadInfo
    {
        public List<string> Keys;
    }

    private DownLoadInfo _downLoadInfo;
    private readonly string DOWNLOADKEY = "DOWNLOADKEY";
    
    private List<object> _catalogsKeys;

    private void Start()
    {
        //服务器更新
        //更新目录  把要更新的目录保存下来
            //服务器目录更新
            //本地目录更新
        //下载对应资源文件
        //下载完成后把保存的数据删除
        StartCoroutine(Check());
    }

    private IEnumerator Check()
    {
        _downLoadInfo = new DownLoadInfo();
        _catalogsKeys = new List<object>();
        yield return CheckAssetUpdate();
        yield return DownLoadAsset();
        //生成资源
        yield return LoadCube();
        //yield return LoadScene();
        //加载热更场景
    }

    private IEnumerator LoadCube()
    {
        var dllHandle = Addressables.LoadAssetsAsync<TextAsset>("HotUpdate.dll",null);
        yield return dllHandle;
        foreach (var s in dllHandle.Result)
        {
            var assembly = Assembly.Load(s.bytes);
        }
// var type = assembly.GetType("Hello");
        var cubeHandle = Addressables.LoadAssetAsync<GameObject>("Cube");
        yield return cubeHandle;
        if (cubeHandle.Status == AsyncOperationStatus.Succeeded)
        {
            Debug.Log("加载cube成功");
            yield return new WaitForSeconds(1);
            var cube = Instantiate(cubeHandle.Result);
            //这里的shader会丢失需要重新复制
            cube.ReSetShader();
            cube.transform.position = Vector3.zero;
        }
        else
        {
            Debug.Log("加载cube失败");
        }
        var cubeHandle1 = Addressables.LoadAssetAsync<GameObject>("Cube 1");
        yield return cubeHandle1;
        if (cubeHandle1.Status == AsyncOperationStatus.Succeeded)
        {
            Debug.Log("加载cube1成功");
            var cube = Instantiate(cubeHandle1.Result);
            //这里的shader会丢失需要重新复制
            //cube.ReSetShader();
            cube.transform.position = Vector3.one * 3;
        }
        else
        {
            Debug.Log("加载cube1失败");
        }
    }
    
    private IEnumerator LoadScene()
    {
        var sceneHandle = Addressables.LoadSceneAsync("Enter");
        yield return sceneHandle;
        if (sceneHandle.Status == AsyncOperationStatus.Succeeded)
        {
            Debug.Log("加载Scene成功");
        }
        else
        {
            Debug.Log("加载Scene失败");
        }
    }

    private IEnumerator CheckAssetUpdate()
    {
        var catalogHandle = Addressables.CheckForCatalogUpdates(false);
        yield return catalogHandle;
        if (catalogHandle.Status == AsyncOperationStatus.Succeeded)
        {
            Debug.Log("检查目录成功");
            Debug.Log("目录长度 ："+catalogHandle.Result.Count);
            //校对本地存储的目录信息
            if (catalogHandle.Result.Count > 0)
            {
                _downLoadInfo.Keys = catalogHandle.Result;
                var keyStr = JsonUtility.ToJson(_downLoadInfo);
                PlayerPrefs.SetString(DOWNLOADKEY,keyStr);
            }
            else
            {
                if (PlayerPrefs.HasKey(DOWNLOADKEY))
                {
                    Debug.Log("服务器有未更新完的资源");
                    var keys = PlayerPrefs.GetString(DOWNLOADKEY);
                    _downLoadInfo = JsonUtility.FromJson<DownLoadInfo>(keys);
                }
            }

            if (_downLoadInfo.Keys != null && _downLoadInfo.Keys.Count > 0)
            {
                var locatorHandle = Addressables.UpdateCatalogs(_downLoadInfo.Keys, false);
                yield return locatorHandle;
                if (locatorHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    _catalogsKeys.Clear();
                    foreach (var locator in locatorHandle.Result)
                    {
                        Debug.Log(locator.Keys);
                        _catalogsKeys.AddRange(locator.Keys);
                    }
                    Debug.Log("更新目录资源长度 ：" + _catalogsKeys.Count);
                }
                else
                {
                    Debug.Log("更新目录资源失败");
                }
                Addressables.Release(locatorHandle);
            }
        }
        else
        {
            Debug.Log("更新目录失败");
        }
 
        Addressables.Release(catalogHandle);
    }

    private IEnumerator DownLoadAsset()
    {
        var sizeAsync = Addressables.GetDownloadSizeAsync((IEnumerable)_catalogsKeys);
        yield return sizeAsync;
        if (sizeAsync.Result <= 0)
        {
            Debug.Log("下载的资源长度为0");
            yield break;
        }
        if (sizeAsync.Status == AsyncOperationStatus.Succeeded)
        {
            Debug.Log("需要下载的资源大小为：" + sizeAsync.Result);
            var downHandle = Addressables.DownloadDependenciesAsync((IEnumerable)_catalogsKeys,Addressables.MergeMode.Union);
            yield return downHandle;
            if (downHandle.Status == AsyncOperationStatus.Succeeded)
            {
                Debug.Log("下载资源成功");
                PlayerPrefs.DeleteKey(DOWNLOADKEY);
            }
            else
            {
                Debug.Log("下载资源失败");
            }
            Addressables.Release(downHandle);
        }
    }

  
    
    // private IEnumerator CheckAssetUpdate()
    // {
    //     _severCatalogsKeys = new List<object>();
    //     //检查目录更新
    //     var checkForCatalogUpdates = Addressables.CheckForCatalogUpdates(false);
    //     yield return checkForCatalogUpdates;
    //     if (checkForCatalogUpdates.Status == AsyncOperationStatus.Succeeded)
    //     {
    //         if (checkForCatalogUpdates.Result.Count > 0)
    //         { 
    //             var updateCatalogs = Addressables.UpdateCatalogs(checkForCatalogUpdates.Result,false);
    //             yield return updateCatalogs;
    //             Addressables.Release(checkForCatalogUpdates);
    //             if (updateCatalogs.Status == AsyncOperationStatus.Succeeded)
    //             {
    //                 Debug.Log("更新目录成功");
    //                 foreach (var catalog in updateCatalogs.Result)
    //                 {
    //                     _severCatalogsKeys.AddRange(catalog.Keys);
    //                 }
    //             }
    //             Addressables.Release(updateCatalogs);
    //         }
    //         else
    //         {
    //             Debug.Log("没有目录更新");
    //         }
    //     }
    //     else
    //     {
    //         Debug.Log("更新资源失败");
    //     }

    // }
}