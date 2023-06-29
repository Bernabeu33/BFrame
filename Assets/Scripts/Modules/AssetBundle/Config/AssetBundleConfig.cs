using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;

namespace AssetBundles
{
    public class AssetBundleConfig
    {
        private static string _rootPath = string.Empty;
        public const string AssetBundlesFolderName = "AssetBundles/";
        private static int mIsEditorMode = 1; // 1为编辑器模式，0为真机
        private const string kIsEditorMode = "IsEditorMode";
        
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

        public static bool IsEditorMode
        {
            get
            {
                if (mIsEditorMode == 1)
                {
                    if (!EditorPrefs.HasKey(kIsEditorMode))
                    {
                        EditorPrefs.SetBool(kIsEditorMode, true);
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
        
        public static string GetPersistentDataPath(string assetPath = null)
        {
            string outputPath = Path.Combine(Application.persistentDataPath, AssetBundleConfig.AssetBundlesFolderName);
            if (!string.IsNullOrEmpty(assetPath))
            {
                outputPath = Path.Combine(outputPath, assetPath);
            }
#if UNITY_EDITOR_WIN
            return GameUtility.FormatToSysFilePath(outputPath);
#else
            return outputPath;
#endif
        }
    } 
}

