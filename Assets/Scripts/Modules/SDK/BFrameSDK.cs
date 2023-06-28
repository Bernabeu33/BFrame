using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

/// <summary>
/// SDK Manager, 用来初始化各种SDK
/// </summary>
public class BFrameSDK : MonoSingle<BFrameSDK>
{
    public bool initOver = false;

    void Awake()
    {
        Debug.Log("BFrameSdk awake");
        DontDestroyOnLoad(gameObject);
    }
    public void InitSDK()
    {
       // TODO 做sdk初始化工作
       initOver = true;
    }
    
    
}
