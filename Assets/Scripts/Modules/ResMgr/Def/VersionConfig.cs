using System.Collections.Generic;
using UnityEngine;

namespace BFrame
{
    [System.Serializable]
    public class VersionItem
    {
        /// <summary>
        /// 包版本
        /// </summary>
        public string version;
        /// <summary>
        /// AssetsVersion md5
        /// </summary>
        public string hash;
        /// <summary>
        /// 资源版本
        /// </summary>
        public string resVersion;
    }
    
    [System.Serializable]
    public class PlatformItem
    {
        public string Platform;
        public List<VersionItem> Versions;

        public PlatformItem()
        {
            Versions = new List<VersionItem>();
        }

        public void SetItem(string version, string hash, string resV)
        {
            foreach (var verItem in Versions)
            {
                if (verItem.version == version)
                {
                    verItem.hash = hash;
                    verItem.resVersion = resV;
                    return;
                }
            }
            var item = new VersionItem();
            item.version = version;
            item.hash = hash;
            item.resVersion = resV;
            Versions.Add(item);
            
        }
    }

    [System.Serializable]
    public class VersionConfig
    {
        public List<PlatformItem> Platforms;
        public VersionConfig()
        {
            Platforms = new List<PlatformItem>();
        }

        public void SetItem(string platformName, string versions, string hash, string resV)
        {
            PlatformItem platformItem = null;
            foreach (var item in Platforms)
            {
                if (item.Platform == platformName)
                {
                    platformItem = item;
                    break;
                }
            }
            if (platformItem == null)
            {
                platformItem = new PlatformItem();
                platformItem.Platform = platformName;
                Platforms.Add(platformItem);
            }
        
            string[] vers = versions.Split( ',');
            for (int k = 0; k < vers.Length; k++)
            {
                platformItem.SetItem(vers[k], hash, resV);
            }
        }

        public VersionItem GetItem(string versions)
        {
            string platformName = PathMgr.GetBuildPlatformName();
            foreach (var platformItem in Platforms)
            {
                if (platformItem.Platform == platformName)
                {
                    foreach (var versionItem in platformItem.Versions)
                    {
                        if (versionItem.version == versions)
                        {
                            return versionItem;
                        }
                    }
                    break;
                }
            }
            return null;
        }
    }
    
}