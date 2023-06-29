using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif

namespace AssetBundles
{
    public class AssetBundleConfig
    {
        private static string _rootPath = string.Empty;
        public const string AssetBundlesFolderName = "AssetBundles/";
        public const string AssetsFolderName = "AssetsPackage";
        public const string CommonMapPattren = ",";
        public const string AssetsPathMapFileName = "AssetsMap.bytes";
        public const string AssetBundleSuffix = ".assetbundle";
        
        //AssetBundle跟路径
        public static string AssetBundleRootPath
        {
            get
            {
                return _rootPath;
            }
            set
            {
                _rootPath = value;
            }
        }
        
#if UNITY_EDITOR
        private static int mIsEditorMode = 1; // 1为编辑器模式，0为真机
        private const string kIsEditorMode = "IsEditorMode";
        public static bool IsEditorMode
        {
            get
            {
                if (mIsEditorMode == 1)
                {
                    if (!EditorPrefs.HasKey(kIsEditorMode))
                    {
                        EditorPrefs.SetBool(kIsEditorMode, false);
                    }
                    mIsEditorMode = EditorPrefs.GetBool(kIsEditorMode, true) ? 1 : 0;
                }
                return mIsEditorMode == 1;
            }
            set
            {
                int newValue = value ? 1 : 0;
                if (newValue != mIsEditorMode)
                {
                    mIsEditorMode = newValue;
                    EditorPrefs.SetBool(kIsEditorMode, value);
                }
            }
        }
#endif
        
        public static string GetPersistentDataPath(string assetPath = null)
        {
            string outputPath = System.IO.Path.Combine(Application.persistentDataPath, AssetBundlesFolderName);
            if (!string.IsNullOrEmpty(assetPath))
            {
                outputPath = System.IO.Path.Combine(outputPath, assetPath);
            }
#if UNITY_EDITOR_WIN
            return GameUtility.FormatToSysFilePath(outputPath);
#else
            return outputPath;
#endif
        }
     

        public static string PackagePathToAssetsPath(string assetPath)
        {
            return "Assets/" + AssetsFolderName + "/" + assetPath;
        }
        
        public static string AssetsPathToPackagePath(string assetPath)
        {
            // Assets/AssetsPackage/lua
            string path = "Assets/" + AssetsFolderName + "/";
            if (assetPath.StartsWith(path))
            {
                // lua
                return assetPath.Substring(path.Length);
            }
            else
            {
                Debug.LogWarning(assetPath + " is not a package path!");
                return assetPath;
            }
        }
    } 
}

