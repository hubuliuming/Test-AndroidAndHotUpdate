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
        yield return InitPackage(PackageName, BuildPipeline, PlayMode);
        yield return LoadCube();
    }
    
    private IEnumerator InitPackage(string packageName, EDefaultBuildPipeline buildPipeline, EPlayMode playMode)
    {
        YooAssets.Initialize();
        // 创建资源包裹类
        var package = YooAssets.TryGetPackage(packageName);
        if (package == null)
            package = YooAssets.CreatePackage(packageName);

        YooAssets.SetDefaultPackage(package);
        // 编辑器下的模拟模式
        InitializationOperation initializationOperation = null;
        if (playMode == EPlayMode.EditorSimulateMode)
        {
            var createParameters = new EditorSimulateModeParameters();
            createParameters.SimulateManifestFilePath = EditorSimulateModeHelper.SimulateBuild(buildPipeline, packageName);
            initializationOperation = package.InitializeAsync(createParameters);
        }

        // 单机运行模式
        if (playMode == EPlayMode.OfflinePlayMode)
        {
            var createParameters = new OfflinePlayModeParameters();
            //createParameters.DecryptionServices = new FileStreamDecryption();
            initializationOperation = package.InitializeAsync(createParameters);
        }
        
        // 联机运行模式
        // if (playMode == EPlayMode.HostPlayMode)
        // {
        //     string defaultHostServer = GetHostServerURL();
        //     string fallbackHostServer = GetHostServerURL();
        //     var createParameters = new HostPlayModeParameters();
        //     createParameters.DecryptionServices = new FileStreamDecryption();
        //     createParameters.BuildinQueryServices = new GameQueryServices();
        //     createParameters.RemoteServices = new RemoteServices(defaultHostServer, fallbackHostServer);
        //     initializationOperation = package.InitializeAsync(createParameters);
        // }
        //
        // // WebGL运行模式
        // if (playMode == EPlayMode.WebPlayMode)
        // {
        //     string defaultHostServer = GetHostServerURL();
        //     string fallbackHostServer = GetHostServerURL();
        //     var createParameters = new WebPlayModeParameters();
        //     createParameters.DecryptionServices = new FileStreamDecryption();
        //     createParameters.BuildinQueryServices = new GameQueryServices();
        //     createParameters.RemoteServices = new RemoteServices(defaultHostServer, fallbackHostServer);
        //     initializationOperation = package.InitializeAsync(createParameters);
        // }
        
        yield return initializationOperation;
        // // 如果初始化失败弹出提示界面
        if (initializationOperation.Status != EOperationStatus.Succeed)
        {
            Debug.LogWarning($"{initializationOperation.Error}");
        }
        else
        {
            var version = initializationOperation.PackageVersion;
            Debug.Log($"Init resource package version : {version}");
        }
    }

    /// <summary>
    /// 获取资源服务器地址
    /// </summary>
    private string GetHostServerURL()
    {
        //string hostServerIP = "http://10.0.2.2"; //安卓模拟器地址
        string hostServerIP = "http://127.0.0.1";
        string appVersion = "v1.0";

#if UNITY_EDITOR
        if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.Android)
            return $"{hostServerIP}/CDN/Android/{appVersion}";
        else if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.iOS)
            return $"{hostServerIP}/CDN/IPhone/{appVersion}";
        else if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.WebGL)
            return $"{hostServerIP}/CDN/WebGL/{appVersion}";
        else
            return $"{hostServerIP}/CDN/PC/{appVersion}";
#else
        if (Application.platform == RuntimePlatform.Android)
            return $"{hostServerIP}/CDN/Android/{appVersion}";
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
            return $"{hostServerIP}/CDN/IPhone/{appVersion}";
        else if (Application.platform == RuntimePlatform.WebGLPlayer)
            return $"{hostServerIP}/CDN/WebGL/{appVersion}";
        else
            return $"{hostServerIP}/CDN/PC/{appVersion}";
#endif
    }

    /// <summary>
    /// 远端资源地址查询服务类
    /// </summary>
    private class RemoteServices : IRemoteServices
    {
        private readonly string _defaultHostServer;
        private readonly string _fallbackHostServer;

        public RemoteServices(string defaultHostServer, string fallbackHostServer)
        {
            _defaultHostServer = defaultHostServer;
            _fallbackHostServer = fallbackHostServer;
        }

        string IRemoteServices.GetRemoteMainURL(string fileName)
        {
            return $"{_defaultHostServer}/{fileName}";
        }

        string IRemoteServices.GetRemoteFallbackURL(string fileName)
        {
            return $"{_fallbackHostServer}/{fileName}";
        }
    }


    /// <summary>
    /// 资源文件流加载解密类
    /// </summary>
    // private class FileStreamDecryption : IDecryptionServices
    // {
    //     /// <summary>
    //     /// 同步方式获取解密的资源包对象
    //     /// 注意：加载流对象在资源包对象释放的时候会自动释放
    //     /// </summary>
    //     AssetBundle IDecryptionServices.LoadAssetBundle(DecryptFileInfo fileInfo, out Stream managedStream)
    //     {
    //         BundleStream bundleStream = new BundleStream(fileInfo.FileLoadPath, FileMode.Open, FileAccess.Read, FileShare.Read);
    //         managedStream = bundleStream;
    //         return AssetBundle.LoadFromStream(bundleStream, fileInfo.ConentCRC, GetManagedReadBufferSize());
    //     }
    //
    //     /// <summary>
    //     /// 异步方式获取解密的资源包对象
    //     /// 注意：加载流对象在资源包对象释放的时候会自动释放
    //     /// </summary>
    //     AssetBundleCreateRequest IDecryptionServices.LoadAssetBundleAsync(DecryptFileInfo fileInfo, out Stream managedStream)
    //     {
    //         BundleStream bundleStream = new BundleStream(fileInfo.FileLoadPath, FileMode.Open, FileAccess.Read, FileShare.Read);
    //         managedStream = bundleStream;
    //         return AssetBundle.LoadFromStreamAsync(bundleStream, fileInfo.ConentCRC, GetManagedReadBufferSize());
    //     }
    //
    //     private static uint GetManagedReadBufferSize()
    //     {
    //         return 1024;
    //     }
    // }
    private IEnumerator LoadCube()
    {
        var cubeHandle = YooAssets.LoadAssetAsync<GameObject>("Cube");
        yield return cubeHandle;
        cubeHandle.InstantiateSync();
        cubeHandle.Release();
    }
}