using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class inventorySlotOptionsButtons : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public int buttonHeight;

	public int extraHeight;

	public int panelBackgroundOffset;

	[Space]
	[Header ("UI Components Settings")]
	[Space]

	public GameObject useButton;
	public GameObject equipButton;
	public GameObject unEquipButton;
	public GameObject dropButton;
	public GameObject combineButton;
	public GameObject examineButton;
	public GameObject holdButton;
	public GameObject discardButton;

	public RectTransform optionsPanel;

	public RectTransform panelBackground;

	int numberOfOptionsEnabled;


	public void setButtonsState (bool useState, bool equipState, bool unEquipState, bool dropState, bool combineState, bool examineState, bool holdState, bool discardState)
	{
		if (useButton != null && useButton.activeSelf != useState) {
			useButton.SetActive (useState);
		}

		if (equipButton != null && equipButton.activeSelf != equipState) {
			equipButton.SetActive (equipState);
		}

		if (unEquipButton != null && unEquipButton.activeSelf != unEquipState) {
			unEquipButton.SetActive (unEquipState);
		}

		if (dropButton != null && dropButton.activeSelf != dropState) {
			dropButton.SetActive (dropState);
		}

		if (combineButton != null && combineButton.activeSelf != combineState) {
			combineButton.SetActive (combineState);
		}

		if (examineButton != null && examineButton.activeSelf != examineState) {
			examineButton.SetActive (examineState);
		}

		if (holdButton != null && holdButton.activeSelf != holdState) {
			holdButton.SetActive (holdState);
		}

		if (discardButton != null && discardButton.activeSelf != discardState) {
			discardButton.SetActive (discardState);
		}

		numberOfOptionsEnabled = 0;

		if (useState) {
			numberOfOptionsEnabled++;
		}

		if (equipState) {
			numberOfOptionsEnabled++;
		}

		if (unEquipState) {
			numberOfOptionsEnabled++;
		}

		if (dropState) {
			numberOfOptionsEnabled++;
		}

		if (combineState) {
			numberOfOptionsEnabled++;
		}

		if (examineState) {
			numberOfOptionsEnabled++;
		}

		if (holdState) {
			numberOfOptionsEnabled++;
		}

		if (discardState) {
			numberOfOptionsEnabled++;
		}

		optionsPanel.sizeDelta = new Vector2 (optionsPanel.sizeDelta.x, (buttonHeight * numberOfOptionsEnabled) + extraHeight);

		panelBackground.sizeDelta = new Vector2 (panelBackground.sizeDelta.x, (buttonHeight * numberOfOptionsEnabled) + panelBackgroundOffset);
	}
}
