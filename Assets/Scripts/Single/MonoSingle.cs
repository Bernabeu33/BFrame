using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MonoSingle<T>: MonoBehaviour where T: MonoSingle<T>
{
    private static T _instnace = null;

    public static T Instance
    {
        get
        {
            if (_instnace == null)
            {
                _instnace = FindObjectOfType(typeof(T)) as T;
                if (_instnace == null)
                {
                    GameObject go = new GameObject(typeof(T).Name);
                    _instnace = go.AddComponent<T>();
                    GameObject parent = GameObject.Find("Boot");
                    if (parent == null)
                    {
                        parent = new GameObject("Boot");
                        DontDestroyOnLoad(parent);
                    }
                    if (parent != null)
                    {
                        go.transform.parent = parent.transform;
                    }
                }
            }
            return _instnace;
        }
    }

    private void Awake()
    {
        if (_instnace == null)
        {
            _instnace = this as T;
        }
        DontDestroyOnLoad(this.gameObject);
        Init();
    }
    
    protected virtual void Init()
    {

    }
}
