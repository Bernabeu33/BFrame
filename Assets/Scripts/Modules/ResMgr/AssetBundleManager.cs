using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using BFrame;

namespace AssetBundles
{
    public enum GetResourceFullPathType
    {
        /// <summary>
        /// 无资源
        /// </summary>
        Invalid,

        /// <summary>
        /// 安装包内
        /// </summary>
        InApp,

        /// <summary>
        /// 热更新目录
        /// </summary>
        InDocument,
    }
    public class AssetBundleManager : MonoSingle<AssetBundleManager>
    {
        public IEnumerator Initialize()
        {
            PathMgr.InitResourcePath();
            var start = DateTime.Now;
            yield return DownloadManager.Instance.Start();
        }
    }
}

