using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class healthBarManagementSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool showSlidersActive = true;

	[Space]
	[Header ("Debug")]
	[Space]

	public List<healthSliderInfo> healthSliderInfoList = new List<healthSliderInfo> ();

	public List<playerHealthBarManagementSystem> playerHealthBarManagementSystemList = new List<playerHealthBarManagementSystem> ();

	int currentID = 0;

	public void addNewPlayer (playerHealthBarManagementSystem newPlayer)
	{
		if (!showSlidersActive) {
			return;
		}

		playerHealthBarManagementSystemList.Add (newPlayer);
	}

	public void disableHealhtBars ()
	{
		if (!showSlidersActive) {
			return;
		}

		enableOrDisableHealhtBars (false);
	}

	public void enableHealhtBars ()
	{
		if (!showSlidersActive) {
			return;
		}

		enableOrDisableHealhtBars (true);
	}

	public void enableOrDisableHealhtBars (bool state)
	{
		if (!showSlidersActive) {
			return;
		}

		for (int i = 0; i < playerHealthBarManagementSystemList.Count; i++) {
			playerHealthBarManagementSystemList [i].enableOrDisableHealhtBars (state);
		}
	}

	public int addNewTargetSlider (GameObject sliderOwner, GameObject sliderPrefab, Vector3 sliderOffset, float healthAmount, float shieldAmount,
		string ownerName, Color textColor, Color sliderColor, bool healthBarSliderActiveOnStart, bool useHealthSlideInfoOnScreen)
	{
		if (!showSlidersActive) {
			return -1;
		}

		healthSliderInfo newHealthSliderInfo = new healthSliderInfo ();
		newHealthSliderInfo.Name = ownerName;
		newHealthSliderInfo.sliderOwner = sliderOwner.transform;

		currentID++;

		newHealthSliderInfo.ID = currentID;

		healthSliderInfoList.Add (newHealthSliderInfo);

		for (int i = 0; i < playerHealthBarManagementSystemList.Count; i++) {
			playerHealthBarManagementSystemList [i].addNewTargetSlider (sliderOwner, sliderPrefab, sliderOffset, healthAmount, shieldAmount, ownerName, 
				textColor, sliderColor, currentID, healthBarSliderActiveOnStart, useHealthSlideInfoOnScreen);
		}

		return currentID;
	}

	public void removeTargetSlider (int objectID)
	{
		if (!showSlidersActive) {
			return;
		}

		for (int i = 0; i < playerHealthBarManagementSystemList.Count; i++) {
			playerHealthBarManagementSystemList [i].removeTargetSlider (objectID);
		}

		for (int i = 0; i < healthSliderInfoList.Count; i++) {
			if (healthSliderInfoList [i].ID == objectID) {
				healthSliderInfoList.RemoveAt (i);
				return;
			}
		}
	}

	public void removeElementFromObjectiveListCalledByPlayer (int objectId, GameObject currentPlayer)
	{
		if (!showSlidersActive) {
			return;
		}

		for (int i = 0; i < healthSliderInfoList.Count; i++) {
			if (healthSliderInfoList [i].ID == objectId) {

				healthSliderInfoList.Remove (healthSliderInfoList [i]);

				for (int j = 0; j < playerHealthBarManagementSystemList.Count; j++) {
					if (playerHealthBarManagementSystemList [j].playerGameObject != currentPlayer) {
						playerHealthBarManagementSystemList [j].removeTargetSlider (objectId);
					}
				}

				return;
			}
		}
	}

	public void udpateSliderInfo (int objectID, string newName, Color textColor, Color backgroundColor)
	{
		if (!showSlidersActive) {
			return;
		}

		for (int i = 0; i < playerHealthBarManagementSystemList.Count; i++) {
			playerHealthBarManagementSystemList [i].setSliderInfo (objectID, newName, textColor, backgroundColor);
		}
	}

	public void updateSliderAmount (int objectID, float value)
	{
		if (!showSlidersActive) {
			return;
		}

		for (int i = 0; i < playerHealthBarManagementSystemList.Count; i++) {
			playerHealthBarManagementSystemList [i].setSliderAmount (objectID, value);
		}
	}

	public void setSliderAmount (int objectID, float sliderValue)
	{
		if (!showSlidersActive) {
			return;
		}

		updateSliderAmount (objectID, sliderValue);
	}

	public void updateShieldSliderAmount (int objectID, float value)
	{
		if (!showSlidersActive) {
			return;
		}

		for (int i = 0; i < playerHealthBarManagementSystemList.Count; i++) {
			playerHealthBarManagementSystemList [i].setShieldSliderAmount (objectID, value);
		}
	}

	public void setShieldSliderAmount (int objectID, float sliderValue)
	{
		if (!showSlidersActive) {
			return;
		}

		updateShieldSliderAmount (objectID, sliderValue);
	}

	public void updateSliderMaxValue (int objectID, float maxSliderValue)
	{
		if (!showSlidersActive) {
			return;
		}

		for (int i = 0; i < playerHealthBarManagementSystemList.Count; i++) {
			playerHealthBarManagementSystemList [i].updateSliderMaxValue (objectID, maxSliderValue);
		}
	}

	public void updateShieldSliderMaxValue (int objectID, float maxSliderValue)
	{
		if (!showSlidersActive) {
			return;
		}

		for (int i = 0; i < playerHealthBarManagementSystemList.Count; i++) {
			playerHealthBarManagementSystemList [i].updateShieldSliderMaxValue (objectID, maxSliderValue);
		}
	}

	public void updateSliderOffset (int objectID, float value)
	{
		for (int i = 0; i < playerHealthBarManagementSystemList.Count; i++) {
			playerHealthBarManagementSystemList [i].updateSliderOffset (objectID, value);
		}
	}

	public void setSliderInfo (int objectID, string newName, Color textColor, Color backgroundColor)
	{
		if (!showSlidersActive) {
			return;
		}

		udpateSliderInfo (objectID, newName, textColor, backgroundColor);
	}

	public void setSliderVisibleStateForPlayer (int objectID, GameObject player, bool state)
	{
		if (!showSlidersActive) {
			return;
		}

		for (int i = 0; i < playerHealthBarManagementSystemList.Count; i++) {
			if (playerHealthBarManagementSystemList [i].getPlayerGameObject () == player) {
				playerHealthBarManagementSystemList [i].setSliderVisibleState (objectID, state);
			}
		}
	}

	public void setSliderLocatedState (int objectID, bool state)
	{
		if (!showSlidersActive) {
			return;
		}

		for (int i = 0; i < playerHealthBarManagementSystemList.Count; i++) {
			playerHealthBarManagementSystemList [i].setSliderLocatedState (objectID, state);
		}
	}

	public void setSliderLocatedStateForPlayer (int objectID, GameObject player, bool state)
	{
		if (!showSlidersActive) {
			return;
		}

		for (int i = 0; i < playerHealthBarManagementSystemList.Count; i++) {
			if (playerHealthBarManagementSystemList [i].getPlayerGameObject () == player) {
				playerHealthBarManagementSystemList [i].setSliderLocatedState (objectID, state);
			}
		}
	}

	public void setSliderVisibleState (int objectID, bool state)
	{
		if (!showSlidersActive) {
			return;
		}

		for (int i = 0; i < playerHealthBarManagementSystemList.Count; i++) {
			playerHealthBarManagementSystemList [i].setSliderVisibleState (objectID, state);
		}
	}

	public void pauseOrResumeShowHealthSliders (bool state)
	{
		if (!showSlidersActive) {
			return;
		}

		for (int i = 0; i < playerHealthBarManagementSystemList.Count; i++) {
			playerHealthBarManagementSystemList [i].pauseOrResumeShowHealthSliders (state);
		}
	}
}
