using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using System.Collections.Generic;
using System.Net;

public enum DownloadableTheme
{
	None,
	Theme1,
	Theme2
}
public class DownloadThemeEditor : EditorWindow
{
	private static DownloadThemeEditor myWindow;
	private string DownloadThemeURLT1 = "https://slack-files.com/T01HKBXE142-F035YNNHRB6-11a2cc8944";
	private string DownloadThemeURLT2 = "";
	private DownloadableTheme SelectedTheme;

	[MenuItem("JamBox/Download Theme")]
	private static void AddCustomEvent()
	{
		myWindow = EditorWindow.GetWindow<DownloadThemeEditor>();
	}

	int i = 0;
	void OnGUI()
	{
		SelectedTheme = (DownloadableTheme)EditorGUILayout.EnumPopup("Message Type", DownloadableTheme.Theme1);
		if (GUILayout.Button("Download"))
		{
			if(SelectedTheme == DownloadableTheme.Theme1)
            {
				Debug.Log("SelectedTheme is Theme1 >>>>");
				using (WebClient client = new WebClient())
				{
					string path = Path.Combine(Application.persistentDataPath, "Theme1.zip");
					client.DownloadFile(DownloadThemeURLT1, path);
					//if (data == null)
					//{
					//	Debug.Log("Download Failed >>>>");
					//}
					//else
					//{
					//	string savePath = string.Format("{0}/{1}.unitypackage", Application.persistentDataPath, DownloadableTheme.Theme1.ToString());
					//	if(File.Exists(savePath))
					//                   {
					//		Debug.Log("File Exists Deleting .....");
					//		File.Delete(savePath);
					//                   }
					//	System.IO.File.WriteAllBytes(savePath, data);
					//	Debug.Log("File saved >>>>" + Application.persistentDataPath.ToString());
					//	AssetDatabase.ImportPackage(savePath, true);
					//}
				}
			}
		}
	}



}
