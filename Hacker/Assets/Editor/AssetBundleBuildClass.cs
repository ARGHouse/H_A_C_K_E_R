namespace HACKER
{
	using UnityEngine;
	using UnityEditor;
	using System.Diagnostics;
	using System;
	public static class AssetBundleBuildClass
	{
		/// for win/Linux/Mac and cleanup created prefabs.
		/// To note: this will build _all_ of your asset bundles. 
		/// If you're adding custom sounds, best case is to create a temporary scene then export.
		[MenuItem("Assets/Build And Clean All Assets")]
		public static void BuildAndCleanAll()
		{
			BuildLinuxAssetBundles();
			BuildWindowsAssetBundles();
			BuildMacAssetBundles();
		}

		/// Will create asset bundles for linux
		[MenuItem("Assets/Build Linux Assets")]
		public static void BuildLinuxAssetBundles()
		{
			string assetPath = Application.streamingAssetsPath + "/Linux";
			bool pathExists = System.IO.Directory.Exists(assetPath);
			if (!pathExists)
				throw new Exception(assetPath + " doesn't exist.");

			BuildAssetBundles(Application.streamingAssetsPath + "/Linux/", BuildTarget.StandaloneLinux64);
		}

		/// Will create asset bundles for windows
		[MenuItem("Assets/Build Windows Assets")]
		public static void BuildWindowsAssetBundles()
		{
			string assetPath = Application.streamingAssetsPath + "/Windows";
			bool pathExists = System.IO.Directory.Exists(assetPath);
			if (!pathExists)
				throw new Exception(assetPath + " doesn't exist");

			BuildAssetBundles(Application.streamingAssetsPath + "/Windows", BuildTarget.StandaloneWindows64);
		}

		/// Will create asset bundles for Mac
		[MenuItem("Assets/Build Mac Assets")]
		public static void BuildMacAssetBundles()
		{
			string assetPath = Application.streamingAssetsPath + "/Mac";
			bool pathExists = System.IO.Directory.Exists(assetPath);
			if (!pathExists)
				throw new Exception(assetPath + " doesn't exist.");
			BuildAssetBundles(Application.streamingAssetsPath + "/Mac", BuildTarget.StandaloneOSXIntel64);
		}

		public static void BuildAssetBundles(string pathIN, BuildTarget target)
		{
			BuildPipeline.BuildAssetBundles(pathIN, BuildAssetBundleOptions.None, target);
		}
	}
}