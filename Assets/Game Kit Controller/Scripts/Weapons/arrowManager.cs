using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class arrowManager : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool useArrowsEnabled = true;

	public string currentArrowInventoryObjectName = "Regular Arrow";

	[Space]
	[Header ("Events Settings")]
	[Space]

	public UnityEvent eventOnArrowsFoundOnInventory;

	public UnityEvent eventOnArrowsNotFoundOnInventory;

	[Space]
	[Header ("Components")]
	[Space]

	public GameObject arrowInfoPanel;

	public Text arrowAmountText;

	public RawImage arrowTypeIcon;

	public GameObject infiniteArrowsIcon;

	public inventoryManager mainInventoryManager;

	int currentArrowAmount;


	void Start ()
	{
		if (useArrowsEnabled) {
			updateArrowAmountText ();
		} else {
			enableOrDisableArrowInfoPanel (false);
		}
	}

	public void enableOrDisableArrowInfoPanel (bool state)
	{
		if (useArrowsEnabled) {
			if (arrowInfoPanel != null) {
				if (arrowInfoPanel.activeSelf != state) {
					arrowInfoPanel.SetActive (state);
				}
			}
		}
	}

	public void checkIfArrowsFoundOnInventory ()
	{
		if (!useArrowsEnabled) {
			return;
		}

		currentArrowAmount = mainInventoryManager.getInventoryObjectAmountByName (currentArrowInventoryObjectName);

		if (currentArrowAmount > 0) {

			eventOnArrowsFoundOnInventory.Invoke ();
		} else {
			eventOnArrowsNotFoundOnInventory.Invoke ();
		}
	}

	public void useArrowFromInventory (int amountToUse)
	{
		checkIfArrowsFoundOnInventory ();

		if (currentArrowAmount > 0) {
			mainInventoryManager.removeObjectAmountFromInventoryByName (currentArrowInventoryObjectName, amountToUse);
		}

		updateArrowAmountText ();
	}

	public void updateArrowAmountText ()
	{
		if (!useArrowsEnabled) {
			return;
		}

		currentArrowAmount = mainInventoryManager.getInventoryObjectAmountByName (currentArrowInventoryObjectName);

		if (arrowAmountText != null) {
			if (currentArrowAmount < 0) {
				currentArrowAmount = 0;
			}

			arrowAmountText.text = currentArrowAmount.ToString ();
		}
	}

	public void setCurrentArrowInventoryObjectName (string newName)
	{
		currentArrowInventoryObjectName = newName;
	}

	public void setArrowTypeIcon (Texture newIcon, bool useInfiniteArrows)
	{
		if (!useArrowsEnabled) {
			return;
		}

		if (arrowTypeIcon != null) {
			if (newIcon == null) {
				arrowTypeIcon.enabled = false;
			} else {
				arrowTypeIcon.enabled = true;

				arrowTypeIcon.texture = newIcon;
			}

			if (infiniteArrowsIcon != null) {
				if (infiniteArrowsIcon.activeSelf != useInfiniteArrows) {
					infiniteArrowsIcon.SetActive (useInfiniteArrows);
				}
			}

			if (arrowAmountText.gameObject.activeSelf != !useInfiniteArrows) {
				arrowAmountText.gameObject.SetActive (!useInfiniteArrows);
			}
		}
	}
}
