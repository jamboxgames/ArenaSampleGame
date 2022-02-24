using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using Jambox.Common.TinyJson;

public enum ConfigMessage
{
	OutOfCurrencyTitle = 0,
	OutOfCurrencyMessage = 1,
	OutOfCurrencyButton,
	InviteMessageAndroid,
	InviteMessageIOS,
	ClaimMessageTitle,
	ClaimMessageBody,
	ClaimMessageButton = 7
}
[Serializable]
public class MsgData
{
	public string MessageKey;
	public string MessageBody;
}

public class CustomizeMessageEditor : EditorWindow
{
	private static CustomizeMessageEditor myWindow;

	bool showAddress = false;
	private Dictionary<ConfigMessage, string> MessageData = new Dictionary<ConfigMessage, string>();
	private List<ConfigMessage> address = new List<ConfigMessage>();
	List<string> data = new List<string>();
	private string className;
	[MenuItem("JamBox/Custom Message Editor")]
	private static void AddCustomEvent()
	{
		myWindow = EditorWindow.GetWindow<CustomizeMessageEditor>();
	}
	int i = 0;
	void OnGUI()
	{
        showAddress = EditorGUILayout.Foldout(showAddress, "Custom Editable Messages");
		if (showAddress)
		{
			//EditorGUILayout.LabelField("Add custom message", "Use plus button");
			createAfield(i);

			if (GUILayout.Button("+", GUILayout.Width(40), GUILayout.Height(40)))
			{
				Debug.Log("+ Button Click hit >>>>>");
				++i;
			}
		}
		if (GUILayout.Button("Save"))
		{
			int TotalSize = address.Count;
			MsgData[] _data = new MsgData[TotalSize];
			Debug.Log("TotalSize of data is : " + TotalSize);
			Dictionary<string, string> valueData = new Dictionary<string, string>();
			for (int i = 0; i < TotalSize; i++)
            {
				if(valueData.ContainsKey(address[i].ToString()))
                {
					Debug.LogError("Duplicate entry of " + address[i].ToString() + " exists. Please add every message data once only.");
					return;
                }
				valueData.Add(address[i].ToString(), data[i]);
				_data[i] = new MsgData();
				_data[i].MessageKey = address[i].ToString();
				_data[i].MessageBody = data[i];
			}
			Debug.Log(" data Length :  " + _data.Length);
			string json = CustomJsonHelper.ToJson(_data, true);
			string filePath = "Assets/Resources/ConfigMessage.json";
			if (File.Exists(filePath))
			{
				File.Delete(filePath);
			}
			Debug.Log("The Json Data : " + json);
			File.WriteAllText(filePath, json);
			myWindow.Close();
		}
	}

	public void createAfield (int k)
    {
		if (i >= ConfigMessage.GetNames(typeof(ConfigMessage)).Length)
        {
			Debug.LogError("You already have created maximum required nuber of fields. Just fill their message data.");
			i = ConfigMessage.GetNames(typeof(ConfigMessage)).Length - 1;
			return;
        }
		for (int j = 0; j <= k; j++)
		{
			//Debug.Log("createAfield : i : " + i);
			ConfigMessage mymessage = (ConfigMessage)i;
			if (address.Count > j)
			{
				address[j] = (ConfigMessage)EditorGUILayout.EnumPopup("Message Type", address[j]);
			}
			else
			{
				address.Add(mymessage);
				address[j] = (ConfigMessage)EditorGUILayout.EnumPopup("Message Type", address[j]);
			}
			if (data.Count > i)
			{
				data[j] = EditorGUILayout.TextField(data[j]);
			}
			else
			{
				data.Add("");
				data[j] = EditorGUILayout.TextField(data[j]);
			}
		}
	}
}
