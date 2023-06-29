using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using AssetBundles;

public class BFrameMenu
{
    private const string kSimulateMode = "BFrame/GameMode/Simulate Mode";
    private const string kEditorMode = "BFrame/GameMode/Editor Mode";
    

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
    
   
}
