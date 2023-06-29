using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class AssetVersionList {
	public List<AssetVersionItem> items;

    public AssetVersionList () {
        this.items = new List<AssetVersionItem> ();
    }
}