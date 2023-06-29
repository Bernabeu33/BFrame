using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

public static class PathUtil {

	/// <summary>
	/// 获取StreamingAssets的子路径
	/// </summary>
	public static string GetOSStreamAssetPath () {
		return Application.streamingAssetsPath + "/";
	}

	/// <summary>
	/// 获取StreamingAssets的路径
	/// </summary>
	public static string GetStreamAssetPath () {
		return Application.streamingAssetsPath + "/";
	}

	/// <summary>
	/// 获取工具的路径
	/// </summary>
	public static string GetToolsPath () {
		return Application.dataPath.ToLower ().Replace ("assets", "") + "Tools";
	}
	
	/// <summary>
	/// 遍历目录及其子目录
	/// </summary>
	/// <return>返回路径和文件列表</return>
	public static void TraverseR (string path, ref List<string> files, ref List<string> paths) {
		string[] names = Directory.GetFiles (path);
		string[] dirs = Directory.GetDirectories (path);
		foreach (string name in names) {
			string ext = Path.GetExtension (name);
			if (!ext.Equals (".meta")) {
				files.Add (name.Replace ('\\', '/'));
			}
		}
		foreach (string dir in dirs) {
			paths.Add (dir.Replace ('\\', '/'));
			TraverseR (dir, ref files, ref paths);
		}
	}

}