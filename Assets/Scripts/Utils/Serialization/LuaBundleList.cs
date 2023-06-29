using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class LuaBundleList {
	public List<string> items;

    public LuaBundleList () {
        this.items = new List<string> ();
    }
}
