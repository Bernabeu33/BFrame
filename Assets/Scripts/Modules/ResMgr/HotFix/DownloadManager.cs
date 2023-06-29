using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AssetBundles;
using System.IO;

namespace BFrame
{
    public class DownloadManager : Single<DownloadManager>
    {
        private const string DownCheckLoadVersion = "DownCheckLoadVersion";
        private string VersionMark
        {
            get
            {
                return $"{PathMgr.AppDataPath}{DownCheckLoadVersion}.{Settings.Version}";
            }
        }
        public IEnumerator Start()
        {
#if UNITY_EDITOR
            if (AssetBundleConfig.IsEditorMode)
            {
                yield break;
            }
#endif
            var data = new Dictionary<string, object>();
            yield return LoadLocalCfg();
        }

        /// <summary>
        /// 读取本地版本资源配置文件
        /// </summary>
        /// <returns></returns>
        IEnumerator LoadLocalCfg()
        {
            // 本地版本信息路径 VersionCfg.json.1.10.5
            var localVersionCfgPath = $"{PathMgr.VersionConfigPath}.{Settings.Version}";
            // www读取需要加前缀 如Android -->jar:file:// 
            var resourceFullPathType = PathMgr.GetResourceFullPath(
                localVersionCfgPath, true, out var localVersionCfgFullPath);
            var isInApp = resourceFullPathType == GetResourceFullPathType.InApp; // 如果是安装包内的资源，就不需要检验文件
            //如果读取的是安装包内的资源，尝试清理之前的热更资源
            if (isInApp)
            {
                ///为了只在新版本升级时，删除老的无用的资源，防止数据无限增长，做了一个版本标记
                if (!GetVersionMark())
                {
                    TryDeleteHotfixResources();
                    GameUtility.SafeWriteAllText(VersionMark, "");
                }
            }
            yield return null;
        }
        
        private bool GetVersionMark()
        {
            // VersionMark : Users/mac/Library/Application Support/DefaultCompany/BFrame1/DownCheckLoadVersion.1.0.0
            return  File.Exists(VersionMark);
        }
        
        /// <summary>
        /// 尝试清理无用的热更资源
        /// </summary>
        private void TryDeleteHotfixResources()
        {
            var path = AssetBundleConfig.GetPersistentDataPath();
            GameUtility.SafeDeleteDir(path);
        }
    }
}


