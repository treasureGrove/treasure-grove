using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class elevatorFloorsPanel : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public GameObject elevator;
	public GameObject floorButton;
	public RectTransform floorListContent;
	public elevatorSystem elevatorManager;

	[Space]
	[Header ("Debug")]
	[Space]

	public List<GameObject> floorButtonList = new List<GameObject> ();

	int i;
	bool usingPanel;
	electronicDevice deviceManager;

	void Start ()
	{
		if (elevatorManager == null) {
			elevatorManager = elevator.GetComponent<elevatorSystem> ();
		}

		int floorsAmount = elevatorManager.floors.Count;

		for (i = 0; i < floorsAmount; i++) {
			GameObject newIconButton = (GameObject)Instantiate (floorButton, Vector3.zero, floorButton.transform.rotation);
			newIconButton.transform.SetParent (floorListContent.transform);
			newIconButton.transform.localScale = Vector3.one;
			newIconButton.transform.localPosition = Vector3.zero;

			newIconButton.transform.GetComponentInChildren<Text> ().text = elevatorManager.floors [i].floorNumber.ToString ();
			newIconButton.name = "Floor - " + (i + 1);

			floorButtonList.Add (newIconButton);
		}

		floorButton.SetActive (false);

		deviceManager = GetComponent<electronicDevice> ();
	}

	//activate the device
	public void activateElevatorFloorPanel ()
	{
		usingPanel = !usingPanel;
	}

	public void goToFloor (Button button)
	{
		int index = -1;

		for (i = 0; i < floorButtonList.Count; i++) {
			if (floorButtonList [i] == button.gameObject) {
				index = i;

				if (elevatorManager.goToNumberFloor (index)) {
					deviceManager.setDeviceState (false);
				}

				return;
			}
		}
	}
}