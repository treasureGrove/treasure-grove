using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class materialExtractionStationUISystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool stationEnabled = true;

	public string noZonesLocatedString = "No Material Zons Located";

	public bool showMaxAndRemainEnergyAmount;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool stationActive;

	public float currentEnergyAmount;

	[Space]
	[Header ("Events Settings")]
	[Space]

	public Text numberOfZonesLocatedText;

	public Text materialsZonesStateText;

	public Text stationEnergyAmountText;

	public materialExtractionStationSystem mainMaterialExtractionStationSystem;


	int numberOfZonesLocated;

	string materialsZonesStateString;


	public void setExtractionActiveState (bool state)
	{
		if (!stationEnabled) {
			return;
		}

		if (stationActive == state) {
			return;
		}

		stationActive = state;

		stopUpdateCoroutine ();

		if (stationActive) {
			updateCoroutine = StartCoroutine (updateSystemCoroutine ());
		} else {
			updateEnergyAmountText ();
		}
	}

	Coroutine updateCoroutine;

	public void stopUpdateCoroutine ()
	{
		if (updateCoroutine != null) {
			StopCoroutine (updateCoroutine);
		}
	}

	IEnumerator updateSystemCoroutine ()
	{
		var waitTime = new WaitForFixedUpdate ();

		while (true) {
			updateSystem ();

			yield return waitTime;
		}
	}

	void updateSystem ()
	{
		numberOfZonesLocated = mainMaterialExtractionStationSystem.numberOfZonesLocated;

		if (numberOfZonesLocated > 0) {
			numberOfZonesLocatedText.text = numberOfZonesLocated.ToString ();

			materialsZonesStateString = "Material Zones Located Info\n";

			for (int i = 0; i < mainMaterialExtractionStationSystem.materialsZoneSystemLocatedList.Count; i++) {
				materialsZonesStateString += "Station " + (i + 1).ToString () + " : ";

				materialsZonesStateString += mainMaterialExtractionStationSystem.materialsZoneSystemLocatedList [i].getRemainMaterialsAmount ();
			
				materialsZonesStateString += "\n";
			}

			materialsZonesStateText.text = materialsZonesStateString;
			
		} else {
			numberOfZonesLocatedText.text = noZonesLocatedString;

			materialsZonesStateString = "Material Zones Located Info\n";

			for (int i = 0; i < mainMaterialExtractionStationSystem.materialsZoneSystemLocatedList.Count; i++) {
				materialsZonesStateString += "Station " + (i + 1).ToString () + " : EMPTY";

				materialsZonesStateString += "\n";
			}

			materialsZonesStateText.text = materialsZonesStateString;
		}

		if (mainMaterialExtractionStationSystem.useEnergyToExtract) {
			updateEnergyAmountText ();
		}
	}

	void updateEnergyAmountText ()
	{
		currentEnergyAmount = mainMaterialExtractionStationSystem.getCurrentEnergyAmount ();

		if (currentEnergyAmount <= 0) {
			currentEnergyAmount = 0;
		}

		if (showMaxAndRemainEnergyAmount) {
			stationEnergyAmountText.text = mainMaterialExtractionStationSystem.getMaxEnergyAmount ().ToString () + "/" +
			currentEnergyAmount.ToString ();
		} else {
			stationEnergyAmountText.text = currentEnergyAmount.ToString ();
		}
	}
}
