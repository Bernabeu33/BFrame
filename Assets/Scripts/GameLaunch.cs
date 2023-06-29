using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using AssetBundles;
using UnityEngine;
using UnityEditor;

public class GameLaunch : MonoBehaviour
{
    private void Awake()
    {
        // 基础设置
    }

    private void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        string platform = GetPlatForm();
        
        string rootPath = Path.Combine(System.Environment.CurrentDirectory, AssetBundleConfig.AssetBundlesFolderName);
        // 打出的ab路径 /Users/mac/Desktop/WorkSpace/BFrame/AssetBundles/Android/AssetBundles
        string AssetBundleRootPath = Path.Combine(rootPath, platform + "/AssetBundles");
        AssetBundleConfig.AssetBundleRootPath = AssetBundleRootPath;
        
        StartCoroutine(StartUp());
    }

    IEnumerator StartUp()
    {
        // 读取项目基础配置
        StartCoroutine(ReadConfig());
        yield return null;
        
        // 初始化sdk
        BFrameSDK.Instance.InitSDK();
        yield return new WaitUntil(()=> BFrameSDK.Instance.initOver);
        
        // 热更
        yield return AssetBundleManager.Instance.Initialize();
        
    }

    IEnumerator ReadConfig()
    {
        yield return 0;
    }

    private string GetPlatForm()
    {
        string platform = string.Empty;
#if !UNITY_EDITOR
        #if UNITY_IPHONE
        platform = "iOS";
        #elif UNITY_WEBGL
        platform = "WebGL";
        #else
        platform = "Android";
        #endif
#else
        switch (EditorUserBuildSettings.activeBuildTarget)
        {
            case BuildTarget.Android:
                platform = "Android";
                break;
            case BuildTarget.iOS:
                platform = "iOS";
                break;
            case BuildTarget.WebGL:
                platform = "WebGL";
                break;
            default:
                throw new Exception("Error buildTarget!!!");
        }
#endif
        return platform;
    }
    


}
