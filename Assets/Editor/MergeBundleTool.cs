using System.Collections.Generic;
using System.IO;
using AssetBundles;
using BFrame;
using UnityEditor;
using UnityEngine;

namespace MergeEditor
{
    
    public class MergeBundleTool
    {
        // 生成AssetVersions_1bbeb5d7c82bdc36c838ff7efb2a7f33 里面是ab包信息例如
        // {
        //      "name": "lua.assetbundle",
        //      "hash": "bbaf938beab0c06f0ccf40aea4bff5f3",
        //      "size": 1326,
        //      "file": "lua_bbaf938beab0c06f0ccf40aea4bff5f3.assetbundle"
        // }
         public static void GenerageVersionList()
        {
            List<string> mPaths = new List<string> ();
            List<string> mFiles = new List<string> ();
            // 进行 AssetsVersion 并 在此进行
            string osStreamPath = PathUtil.GetOSStreamAssetPath ();
            var pathBundleDir = osStreamPath + AssetBundleConfig.AssetBundlesFolderName;
            PathUtil.TraverseR (pathBundleDir, ref mFiles, ref mPaths);

            AssetVersionList versionList = new AssetVersionList ();
            string bundleName;
            for (int i = 0; i < mFiles.Count; i++) {
                string file = mFiles[i];
                if (file.EndsWith (".meta") || file.Contains (".DS_Store") || file.EndsWith(".manifest")) continue;
                bundleName = file.Replace(pathBundleDir, string.Empty);
                FileInfo fileInfo = new FileInfo (file);
                AssetVersionItem item = new AssetVersionItem ();
                item.name = bundleName;
                item.size = (int)fileInfo.Length;
                if (bundleName.EndsWith("AssetBundles"))
                {
                    item.hash = CryptoUtil.md5file (file);;
                    item.file = bundleName + "_" + item.hash;
                    var fileDestPath = file + "_" + item.hash;
                    if (!File.Exists(fileDestPath))
                    {
                        File.Move(file, fileDestPath);
                    }
                }
                else
                {
                    item.hash = trimHash(bundleName);
                    item.name = item.name.Replace("_" + item.hash, string.Empty);
                    item.file = bundleName;
                }
                versionList.items.Add (item);
            }
            string json = JsonUtility.ToJson (versionList);
            string versionMd5 = CryptoUtil.md5text(json);
            string md5Sufix = "_" + versionMd5;
            // Users/mac/Desktop/WorkSpace/BFrame/Assets/StreamingAssets/AssetBundles/AssetVersions_52f30c97cbacaf796aed3ba46af9737b.json
            string outPath = pathBundleDir + PathMgr.VersionListFileName.Replace(".json", md5Sufix + ".json");
            SaveFile(json, outPath);
            GenerateResConfig(versionMd5);
        }

         public static string trimHash(string filePah)
         {
             var path = filePah;
             if (filePah.EndsWith(AssetBundleConfig.AssetBundleSuffix))
             {
                 path = filePah.Replace(AssetBundleConfig.AssetBundleSuffix, string.Empty);
             }

             string hash = null;
             string[] arr = path.Split('_');
             if (arr.Length > 1)
             {
                 hash = arr[arr.Length - 1];
             }

             return hash;
         }

         /// <summary>
         /// 生成版本信息 VersionCfg
         /// </summary>
         /// <param name="versionMd5"></param>
         public static void GenerateResConfig(string versionMd5)
         {
             string path = Path.Combine(Application.streamingAssetsPath, PathMgr.VersionConfigPath);
             string text = "{}";
             if (File.Exists(path))
             {
                 text = File.ReadAllText(path);
             }
             VersionConfig versionConfig = JsonUtility.FromJson<VersionConfig>(text);
             string platform = PathMgr.GetBuildPlatformName();
             string updateVersions = BuilderCfg.ResConfig;
             if (updateVersions == "")
             {
                 updateVersions = Settings.Version;
             }

             versionConfig.SetItem(platform, updateVersions, versionMd5, BuilderCfg.ResVersion);
             string json = JsonUtility.ToJson(versionConfig);
             SaveFile(json, path);
             SaveFile(json, path+"."+Settings.Version);
         }

         public static void SaveFile(string str, string outPath)
         {
             if (File.Exists (outPath)) {
                 File.Delete (outPath);
             }
             Debug.Log(outPath);
             FileStream fs = new FileStream (outPath, FileMode.CreateNew);
             StreamWriter sw = new StreamWriter (fs);
             sw.Write (str);
             sw.Close ();
             fs.Close ();
             AssetDatabase.Refresh ();
         }
    }
}