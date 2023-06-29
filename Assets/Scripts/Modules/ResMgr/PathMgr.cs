using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
using AssetBundles;
using System;

namespace BFrame
{
    public class PathMgr
    {
        
        public static bool ReadStreamFromEditor = true;
        
        /**路径说明
         * Editor下模拟下载资源：
         *     AppData:C:\xxx\xxx\Appdata
         *     StreamAsset:C:\KSFramrwork\Product
         * 真机：
         *     AppData:Android\data\com.xxx.xxx\files\
         *     StreamAsset:apk内
         */
        private static string editorProductFullPath;
        
        /// <summary>
        /// 控制版本热更的配置文件
        /// </summary>
        public static string VersionConfigPath = "VersionCfg.json";
        
        /// <summary>
        /// 记录所有ab的各种信息，包括md5名字，bundle名字，size
        /// </summary>
        public static string VersionListFileName = "AssetVersions.json";

        
        /// <summary>
        /// Bundles/Android/ etc... no prefix for streamingAssets
        /// </summary>
        public static string BundlesPathRelative { get; private set; }
        
        /// <summary>
        /// file://+Application.persistentDataPath
        /// </summary>
        public static string AppDataPathWithProtocol;
        
        
        /// <summary>
        /// On Windows, file protocol has a strange rule that has one more slash
        /// </summary>
        /// <returns>string, file protocol string</returns>
        public static string GetFileProtocol
        {
            get
            {
                string fileProtocol = "file://";
                if (Application.platform == RuntimePlatform.WindowsEditor ||
                    Application.platform == RuntimePlatform.WindowsPlayer)
                {
                    fileProtocol = "file:///";
                }else if (Application.platform == RuntimePlatform.Android)
                {
                    fileProtocol = "jar:file://";
                }
                
                return fileProtocol;
            }
        }
        
        private static string appDataPath = null;
        /// <summary>
        /// app的数据目录，有读写权限，实际是Application.persistentDataPath，以/结尾
        /// </summary>
        public static string AppDataPath
        {
            get
            {
                if (appDataPath == null) appDataPath = Application.persistentDataPath + "/";
                return appDataPath;
            }
        }

        /// <summary>
        /// 安装包内的路径，移动平台为只读权限，针对Application.streamingAssetsPath进行多平台处理，以/结尾
        /// </summary>
        public static string AppBasePath { get; private set; }
        
        /// <summary>
        /// WWW的读取需要file://前缀
        /// </summary>
        public static string AppBasePathWithProtocol { get; private set; }
        
        /// <summary>
        /// Product Folder Full Path , Default: C:\KSFramework\Product
        /// </summary>
        public static string EditorProductFullPath
        {
            get
            {
                if (string.IsNullOrEmpty(editorProductFullPath))
                {
                    editorProductFullPath = Path.GetFullPath(string.Format("AssetBundles/{0}/AssetBundles", GetBuildPlatformName()));
                    //editorProductFullPath = AssetBundleConfig.AssetBundleRootPath;
                }
                return editorProductFullPath;
            }
        }

        /// <summary>
        /// Initialize the path of AssetBundles store place ( Maybe in PersitentDataPath or StreamingAssetsPath )
        /// </summary>
        /// <returns></returns>
        public static void InitResourcePath()
        {
            string editorProductPath = EditorProductFullPath;
            // AssetBundles/
            BundlesPathRelative = AssetBundleConfig.AssetBundlesFolderName;
            string fileProtocol = GetFileProtocol;
            // 沙河路径 --> file:///Users/mac/Library/Application Support/DefaultCompany/BFrame1/
            AppDataPathWithProtocol = fileProtocol + AppDataPath;
            switch (Application.platform)
            {
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.OSXEditor:
                case RuntimePlatform.LinuxEditor:
                {
                    if (ReadStreamFromEditor)
                    {
                        AppBasePath = Application.streamingAssetsPath + "/";
                        AppBasePathWithProtocol = fileProtocol + AppBasePath;
                    }
                    else
                    {
                        AppBasePath = editorProductPath + "/";
                        AppBasePathWithProtocol = fileProtocol + AppBasePath;
                    }
                }
                    break;
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.OSXPlayer:
                {
                    string path = Application.streamingAssetsPath.Replace('\\', '/');
                    AppBasePath = path + "/";
                    AppBasePathWithProtocol = fileProtocol + AppBasePath;
                }
                    break;
                case RuntimePlatform.Android:
                {
                    //文档：https://docs.unity3d.com/Manual/StreamingAssets.html
                    //注意，StramingAsset在Android平台是在apk中，无法通过File读取请使用LoadAssetsSync，如果要同步读取ab请使用GetAbFullPath
                    //NOTE 我见到一些项目的做法是把apk包内的资源放到Assets的上层res内，读取时使用 jar:file://+Application.dataPath + "!/assets/res/"，editor上则需要/../res/
                    AppBasePath = Application.dataPath + "!/assets/";
                    AppBasePathWithProtocol = fileProtocol + AppBasePath;
                }
                    break;
                case RuntimePlatform.IPhonePlayer:
                {
                    // MacOSX下，带空格的文件夹，空格字符需要转义成%20
                    // only iPhone need to Escape the fucking Url!!! other platform works without it!!!
                    AppBasePath = System.Uri.EscapeUriString(Application.dataPath + "/Raw/");
                    AppBasePathWithProtocol = fileProtocol + AppBasePath;
                }
                    break;
                default:
                {
                    //Debuger.Assert(false);
                }
                    break;
                
            }
        }
        
        /// <summary>
        /// 根据相对路径，获取到完整路径，優先从下載資源目录找，没有就读本地資源目錄 
        /// 根路径：Product
        /// </summary>
        /// <param name="url">相对路径</param>
        /// <param name="withFileProtocol"></param>
        /// <param name="fullPath">完整路径</param>
        /// <param name="raiseError">文件不存在打印Error</param>
        /// <returns></returns>
        public static GetResourceFullPathType GetResourceFullPath(string url, bool withFileProtocol,
            out string fullPath, bool raiseError = true)
        {
            if (string.IsNullOrEmpty(url))
            {
                Debug.LogErrorFormat("尝试获取一个空的资源路径！url = {0}", url);
                fullPath = null;
                return GetResourceFullPathType.Invalid;
            }

            fullPath = string.Empty;
            
            // 先从沙河路径读取
            // mac编辑器下路径如：file:///Users/mac/Library/Application Support/DefaultCompany/BFrame1/VersionCfg.json.1.0.0
            string docUrl; 
            bool hasDocUrl = TryGetAppDataUrl(url, withFileProtocol, out docUrl);
            if (hasDocUrl)
            {
                fullPath = docUrl;
                return GetResourceFullPathType.InDocument;
            }
            
            // 再从安装包StreamingAssets内读取
            string inAppUrl;
            // mac编辑器路径如：file:///Users/mac/Desktop/WorkSpace/BFrame/Assets/StreamingAssets/VersionCfg.json.1.0.0
            bool hasInAppUrl = TryGetInAppStreamingUrl(url, withFileProtocol, out inAppUrl);
            if (!hasInAppUrl) // 连本地资源都没有，直接失败吧 ？？ 
            {
                if (raiseError) 
                    UnityEngine.Debug.LogError($"[Not Found] StreamingAssetsPath Url Resource: {url} ,fullPath:{inAppUrl}");
                fullPath = null;
                return GetResourceFullPathType.Invalid;
            }
            fullPath = inAppUrl; // 直接使用本地資源！
            return GetResourceFullPathType.InApp;
        }
        
          
        /// <summary>
        /// 可读写的目录
        /// </summary>
        /// <param name="url"></param>
        /// <param name="withFileProtocol">是否带有file://前缀</param>
        /// <param name="newUrl"></param>
        /// <returns></returns>
        public static bool TryGetAppDataUrl(string url, bool withFileProtocol, out string newUrl)
        {
            newUrl = (withFileProtocol ? AppDataPathWithProtocol : AppDataPath) + url;
            //newUrl = Path.GetFullPath(newUrl);
            return File.Exists(Path.GetFullPath(AppDataPath + url));
        }
        
        /// <summary>
        /// StreamingAssets目录
        /// </summary>
        /// <param name="url"></param>
        /// <param name="withFileProtocol">是否带有file://前缀</param>
        /// <param name="newUrl"></param>
        /// <returns></returns>
        public static bool TryGetInAppStreamingUrl(string url, bool withFileProtocol, out string newUrl)
        {
            newUrl = (withFileProtocol ? AppBasePathWithProtocol : AppBasePath) + url;
            //newUrl = Path.GetFullPath(newUrl);
            
            if (Application.isEditor)
            {
                // Editor进行文件检查
                if (!File.Exists(Path.GetFullPath(AppBasePath + url)))
                {
                    return false;
                }
            }
            else if(Application.platform == RuntimePlatform.Android) // 注意，StreamingAssetsPath在Android平台時，壓縮在apk里面，不要使用File做文件檢查
            {
                // TODO : 特别注意 android文件路径判断需要java
                return AndroidNativeHelper.IsAssetExists(url);
            }else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                if (!File.Exists(Path.GetFullPath(AppBasePath + url)))
                {
                    return false;
                }
            }

            // Windows/Editor平台下，进行大小敏感判断
            if (Application.isEditor)
            {
                var result = FileExistsWithDifferentCase(AppBasePath + url);
                if (!result)
                {
                    UnityEngine.Debug.LogErrorFormat("[大小写敏感]发现一个资源 {0}，大小写出现问题，在Windows可以读取，手机不行，请改表修改！", url);
                }
            }

            return true;
        }
        
        private static string _unityEditorEditorUserBuildSettingsActiveBuildTarget;
        public static string UnityEditor_EditorUserBuildSettings_activeBuildTarget
        {
            get
            {
                if (Application.isPlaying && !string.IsNullOrEmpty(_unityEditorEditorUserBuildSettingsActiveBuildTarget))
                {
                    return _unityEditorEditorUserBuildSettingsActiveBuildTarget;
                }

                var assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
                foreach (var a in assemblies)
                {
                    if (a.GetName().Name == "UnityEditor")
                    {
                        Type lockType = a.GetType("UnityEditor.EditorUserBuildSettings");
                        //var retObj = lockType.GetMethod(staticMethodName,
                        //    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public)
                        //    .Invoke(null, args);
                        //return retObj;
                        var p = lockType.GetProperty("activeBuildTarget");

                        var em = p.GetGetMethod().Invoke(null, new object[] { }).ToString();
                        _unityEditorEditorUserBuildSettingsActiveBuildTarget = em;
                        return em;
                    }
                }

                return null;
            }
        }
        
        public static string GetBuildPlatformName()
        {
            string buildPlatformName = "Windows"; // default

            if (Application.isEditor)
            {
                var buildTarget = UnityEditor_EditorUserBuildSettings_activeBuildTarget;
                //UnityEditor.EditorUserBuildSettings.activeBuildTarget;
                switch (buildTarget)
                {
                    case "StandaloneOSXIntel":
                    case "StandaloneOSXIntel64":
                    case "StandaloneOSXUniversal":
                    case "StandaloneOSX":
                        buildPlatformName = "MacOS";
                        break;
                    case "StandaloneWindows": // UnityEditor.BuildTarget.StandaloneWindows:
                    case "StandaloneWindows64": // UnityEditor.BuildTarget.StandaloneWindows64:
                        buildPlatformName = "Windows";
                        break;
                    case "Android": // UnityEditor.BuildTarget.Android:
                        buildPlatformName = "Android";
                        break;
                    case "iPhone": // UnityEditor.BuildTarget.iPhone:
                    case "iOS":
                        buildPlatformName = "iOS";
                        break;
                    default:
                        break;
                }
            }
            else
            {
                buildPlatformName = GetRunningPlatformName();
            }
            
            return buildPlatformName;
        }
        
        public static string GetRunningPlatformName()
        {
            string buildPlatformName = "Windows";
            switch (Application.platform)
            {
                case RuntimePlatform.OSXPlayer:
                    buildPlatformName = "MacOS";
                    break;
                case RuntimePlatform.Android:
                    buildPlatformName = "Android";
                    break;
                case RuntimePlatform.IPhonePlayer:
                    buildPlatformName = "iOS";
                    break;
                case RuntimePlatform.WindowsPlayer:
#if !UNITY_5_4_OR_NEWER
                    case RuntimePlatform.WindowsWebPlayer:
#endif
                    buildPlatformName = "Windows";
                    break;
                default:
                    // Debuger.Assert(false);
                    break;
            }

            return buildPlatformName;
        }
        
        /// <summary>
        /// 大小写敏感地进行文件判断, Windows Only
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private static bool FileExistsWithDifferentCase(string filePath)
        {
            if (File.Exists(filePath))
            {
                string directory = Path.GetDirectoryName(filePath);
                string fileTitle = Path.GetFileName(filePath);
                string[] files = Directory.GetFiles(directory, fileTitle);
                var realFilePath = files[0].Replace("\\", "/");
                filePath = filePath.Replace("\\", "/");
                filePath = filePath.Replace("//", "/");

                return String.CompareOrdinal(realFilePath, filePath) == 0;
            }

            return false;
        }
        
    }
    
}

