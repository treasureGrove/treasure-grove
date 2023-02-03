using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class showValuesOnScreenSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool showValueEnabled = true;

	public bool addExtraString;
	public string extraString;
	public bool addExtraStringAtStart;

	[Space]
	[Header ("Stats Settings")]
	[Space]

	public string statNameToShow;
	public bool showStatValueOnStart;

	[Space]
	[Header ("Inventory Settings")]
	[Space]

	public string inventoryObjectNameToShow;
	public bool showInventoryObjectAmountOnStart;

	[Space]
	[Header ("HUD Settings")]
	[Space]

	public GameObject valueTextPanel;
	public Text valueText;

	public GameObject playerGameObject;

	bool panelActive;

	void Start ()
	{
		if (!showValueEnabled) {
			if (valueTextPanel.activeSelf) {
				valueTextPanel.SetActive (false);
			}
		}

		if (showStatValueOnStart || showInventoryObjectAmountOnStart) {
			StartCoroutine (showValueOnStartCoroutine ());
		}
	}

	public void showInventoryObjectAmountByName ()
	{
		if (inventoryObjectNameToShow != "") {
			float newValue = GKC_Utils.getInventoryObjectAmountByName (inventoryObjectNameToShow, playerGameObject);

			if (newValue > -1) {
				updateValueAmount (newValue);
			}
		}
	}

	public void showStatByName ()
	{
		if (statNameToShow != "") {
			float newValue = GKC_Utils.getStateValueByName (statNameToShow, playerGameObject);

			if (newValue > -1) {
				updateValueAmount (newValue);
			}
		}
	}

	public void updateValueAmount (float newValue)
	{
		if (!showValueEnabled) {
			return;
		}

		if (!panelActive) {
			if (!valueTextPanel.activeSelf) {
				valueTextPanel.SetActive (true);
			}

			panelActive = true;
		}

		string valueString = newValue.ToString ();

		if (addExtraString) {
			if (addExtraStringAtStart) {
				valueString = extraString + valueString;
			} else {
				valueString += extraString;
			}
		}

		valueText.text = valueString;
	}

	IEnumerator showValueOnStartCoroutine ()
	{
		yield return new WaitForSeconds (0.3f);

		if (showInventoryObjectAmountOnStart) {
			showInventoryObjectAmountByName ();
		}

		if (showStatValueOnStart) {
			showStatByName ();
		}
	}
}
