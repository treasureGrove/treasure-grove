using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class screenActionInfo
{
	public string screenActionName;
	public GameObject screenActionsGameObject;
	public bool isMainGameScreenActionPanel;
	public bool mainGameScreenActionPanelActive;

	[Space]

	public GameObject mainScreenActionGameObject;

	[Space]

	public bool hasSecondaryActionPanel;
	public GameObject secondaryActionPanelGameObject;

	[Space]

	public GameObject screenActionsGamepadGameObject;
	public GameObject secondaryActionPanelGamepadGameObject;
}

[System.Serializable]
public class touchPanelsInfo
{
	public string Name;
	public GameObject touchPanel;
}

[System.Serializable]
public class touchPanelsSchemesInfo
{
	public string Name;
	public bool currentTouchPanelScheme;
	public List<GameObject> enabledPanels = new List<GameObject> ();
	public bool isMainGameTouchPanel;
	public bool mainGameTouchPanelActive;
}