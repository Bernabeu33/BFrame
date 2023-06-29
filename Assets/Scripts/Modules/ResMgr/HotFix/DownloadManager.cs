using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AssetBundles;

namespace BFrame
{
    public class DownloadManager : Single<DownloadManager>
    {
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
            var resourceFullPathType = PathMgr.GetResourceFullPath(
                localVersionCfgPath, true, out var localVersionCfgFullPath);
            yield return null;
        }
    }
}


