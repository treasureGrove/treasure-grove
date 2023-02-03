using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class throwGrenadeSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool throwGrenadeEnabled = true;

	public string grenadeInventoryObjectName = "Grenade";

	public bool checkInventoryInfo = true;

	[Space]
	[Header ("Events Settings")]
	[Space]

	public UnityEvent eventOnStartThrowGrenade;

	public UnityEvent eventOnConfirmThrowGrenade;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool showDebugPrint;

	[Space]
	[Header ("Components")]
	[Space]

	public GameObject grenadeInfoPanel;

	public Text grenadeAmountText;

	public inventoryManager mainInventoryManager;


	bool canConfirmThrowGrenade;

	int currentGrenadeAmount;


	void Start ()
	{
		checkGrenadesInfo ();
	}

	public void checkGrenadesInfo ()
	{
		if (throwGrenadeEnabled) {
			updateGrenadeAmountText ();
		} else {
			if (grenadeInfoPanel != null) {
				if (grenadeInfoPanel.activeSelf != false) {
					grenadeInfoPanel.SetActive (false);
				}
			}
		}
	}

	public void inputStartThrowGrenade ()
	{
		if (!throwGrenadeEnabled) {
			return;
		}

		canConfirmThrowGrenade = false;

		if (checkInventoryInfo) {
			currentGrenadeAmount = mainInventoryManager.getInventoryObjectAmountByName (grenadeInventoryObjectName);

			if (currentGrenadeAmount < 0) {
				currentGrenadeAmount = 0;
			}
		} else {
			currentGrenadeAmount = 1;
		}

		if (currentGrenadeAmount > 0) {

			canConfirmThrowGrenade = true;

			eventOnStartThrowGrenade.Invoke ();

			if (showDebugPrint) {
				print ("Amount found higher than 0");
			}
		} else {
			if (showDebugPrint) {
				print ("Amount found is 0");
			}
		}
	}

	public void inputConfirmThrowGrenade ()
	{
		if (!throwGrenadeEnabled) {
			return;
		}

		if (canConfirmThrowGrenade) {
			eventOnConfirmThrowGrenade.Invoke ();

			updateGrenadeAmountText ();

			if (showDebugPrint) {
				print ("Confirm to throw grenade");
			}
		} else {
			if (showDebugPrint) {
				print ("Can't confirm to throw grenade");
			}
		}
	}

	public void updateGrenadeAmountText ()
	{
		if (!throwGrenadeEnabled) {
			return;
		}

		if (grenadeAmountText != null) {

			currentGrenadeAmount = mainInventoryManager.getInventoryObjectAmountByName (grenadeInventoryObjectName);

			if (currentGrenadeAmount < 0) {
				currentGrenadeAmount = 0;
			}

			grenadeAmountText.text = currentGrenadeAmount.ToString ();
		}
	}
}
