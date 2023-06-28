using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Single<T> where T:class, new()
{
    private static T _instnace = null;

    public static T Instance
    {
        get
        {
            if (_instnace == null)
            {
                _instnace = Activator.CreateInstance<T>();
                if (_instnace != null)
                {
                    (_instnace as Single<T>).Init();
                }
            }
            return _instnace;
        }
    }

    public virtual void Init()
    {
        
    }
}
