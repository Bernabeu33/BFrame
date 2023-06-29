using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using AssetBundles;
using AssetBundleBrowser;
using System.IO;

public class BFrameMenu
{
    private const string kSimulateMode = "BFrame/GameMode/Simulate Mode";
    private const string kEditorMode = "BFrame/GameMode/Editor Mode";
    private const string kNode = "BFrame/";


    [MenuItem(kSimulateMode, false)]
    public static void ToggleSimulateMode()
    {
        AssetBundleConfig.IsEditorMode = false;
        Menu.SetChecked(kSimulateMode, true);
        Menu.SetChecked(kEditorMode, false);
        Debug.Log("切换运行模式到:Simulate真机");
    }
    
    [MenuItem(kEditorMode, false)]
    public static void ToggleEditorMode()
    {
        AssetBundleConfig.IsEditorMode = true;
        Menu.SetChecked(kEditorMode, true);
        Menu.SetChecked(kSimulateMode, false);
        Debug.Log("切换运行模式到:Editor");
    }

    [MenuItem("BFrame/Build/bundle/LZ4 Append Hash")]
    public static void LZ4AppendHash()
    {
        BuildBundle(AssetBundleBuildTab.CompressOptions.ChunkBasedCompression, true);
    }

   
    
    private static void BuildBundle(AssetBundleBuildTab.CompressOptions compressOption, bool appendHash = false)
    {
        // 清空打包资源生成路径下的旧资源
        var path = Path.Combine(Application.streamingAssetsPath, "AssetBundles");
        GameUtility.SafeDeleteDir(path);
        // copy lua资源
        CopyLua();
        
        // 借助AssetBundleBrowser 打包
        AssetBundleBuildTab buildTab = new AssetBundleBuildTab();
        buildTab.RealEnable();
        buildTab.ResetState();
        buildTab.SetAppendHash(appendHash);
        buildTab.SetCompressType(compressOption);
        buildTab.ExecuteMethod();
    }
    
    [MenuItem(kNode + "Res/CopyLua")]
    public static void CopyLua()
    {
        // 将Assets/LuaScripts下所有文件copy到Assets/AssetsPackage下的Lua文件下
        string destination = Path.Combine(Application.dataPath, AssetBundleConfig.AssetsFolderName);
        // 目标路径--> /Users/mac/Desktop/WorkSpace/BFrame/Assets/AssetsPackage/Lua
        destination = Path.Combine(destination, "Lua");
        // 原路径 --> /Users/mac/Desktop/WorkSpace/BFrame/Assets/LuaScripts
        string source = Path.Combine(Application.dataPath, "LuaScripts");
        GameUtility.SafeDeleteDir(destination);
        
        FileUtil.CopyFileOrDirectoryFollowSymlinks(source, destination);
        
        var notLuaFiles = GameUtility.GetSpecifyFilesInFolder(destination, new string[] { ".lua", ".pb" }, true);
        if (notLuaFiles != null && notLuaFiles.Length > 0)
        {
            for (int i = 0; i < notLuaFiles.Length; i++)
            {
                GameUtility.SafeDeleteFile(notLuaFiles[i]);
            }
        }

        var luaFiles = GameUtility.GetSpecifyFilesInFolder(destination, new string[] { ".lua", ".pb" }, false);
        if (luaFiles != null && luaFiles.Length > 0)
        {
            for (int i = 0; i < luaFiles.Length; i++)
            {
                GameUtility.SafeRenameFile(luaFiles[i], luaFiles[i] + ".bytes");
            }
        }
        
        AssetDatabase.Refresh();
        Debug.Log("Copy lua files over");
    }
    
   
}
