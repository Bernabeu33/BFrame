using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
using AssetBundles;

namespace BFrame
{
    public class PathMgr
    {
        /// <summary>
        /// 控制版本热更的配置文件
        /// </summary>
        public static string VersionConfigPath = "VersionCfg.json";

        
        /// <summary>
        /// Bundles/Android/ etc... no prefix for streamingAssets
        /// </summary>
        public static string BundlesPathRelative { get; private set; }
        
        
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
        /// Initialize the path of AssetBundles store place ( Maybe in PersitentDataPath or StreamingAssetsPath )
        /// </summary>
        /// <returns></returns>
        public static void InitResourcePath()
        {
            // AssetBundles
            BundlesPathRelative = AssetBundleConfig.AssetBundlesFolderName;
            Debug.Log(BundlesPathRelative);
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
            string docUrl;
            
            // 再从安装包StreamingAssets内读取
            
            return GetResourceFullPathType.InApp;
        }

    }
}

