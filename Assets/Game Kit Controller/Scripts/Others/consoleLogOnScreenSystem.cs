using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class consoleLogOnScreenSystem : MonoBehaviour
{
	public Text consoleLogText;
	public int maxTextLength = 3000;
	public int textLenghtToRemoveOnMaxAmount = 2500;

	static string myLog = "";
	private string output;

	void OnEnable ()
	{
		Application.logMessageReceived += Log;
	}

	void OnDisable ()
	{
		Application.logMessageReceived -= Log;
	}

	public void Log (string logString, string stackTrace, LogType type)
	{
		output = logString;

		myLog = output + "\n" + myLog;

		if (myLog.Length > maxTextLength) {
			myLog = myLog.Substring (0, textLenghtToRemoveOnMaxAmount);
		}
	}

	void OnGUI ()
	{
		consoleLogText.text = myLog;
	}
}
