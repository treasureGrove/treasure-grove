using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class openInitialPopUpWindow : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool showInitialPopWindowEnabled = true;

	public bool checkForPresetsActive;


	void Awake ()
	{
		checkOpenInitialPopUpWindow ();
	}

	void checkOpenInitialPopUpWindow ()
	{
		#if UNITY_EDITOR

		if (showInitialPopWindowEnabled) {
			string relativePath = "Assets/Game Kit Controller/Pop Up Window Folder";

			if (!Directory.Exists (relativePath)) {
				UnityEditor.EditorApplication.isPaused = true;

				Directory.CreateDirectory (relativePath);

				initialPopUpWindow mainWindow = (initialPopUpWindow)EditorWindow.GetWindow (typeof(initialPopUpWindow));

				mainWindow.Init ();

				UnityEditor.EditorApplication.isPlaying = false;

				mainWindow.checkForPresetsActive = checkForPresetsActive;
			}
		}
		#endif
	}

	public void setShowInitialPopWindowEnabledState (bool state)
	{
		showInitialPopWindowEnabled = state;

		if (!showInitialPopWindowEnabled) {
			checkForPresetsActive = false;
		}
	}
}
