using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class playerHUDManager : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	//this scripts allows to a vehicle to get all the hud elements, so all the sliders values and text info can be showed correctly
	public string playerHUDName = "Player Info";
	public string playerBarsName = "Player Bars";
	public string playerModeName = "Player Mode";

	public GameObject freeInteractionDeviceCursor;

	public bool useBlurUIPanel = true;

	[Space]
	[Header ("HUD Element List Settings")]
	[Space]

	public List<HUDElementInfo> HUDElementInfoList = new List<HUDElementInfo> ();

	[Space]
	[Header ("HUD Elements")]
	[Space]

	public GameObject playerHUD;
	public GameObject vehicleHUD;
	public Slider vehicleHealth;
	public Slider vehicleBoost;
	public Slider vehicleFuel;
	public Slider vehicleAmmo;
	public Text weaponName;
	public Text ammoInfo;
	public GameObject healthContent;
	public GameObject energyContent;
	public GameObject ammoContent;
	public GameObject fuelContent;
	public GameObject vehicleCursor;
	public Texture defaultVehicleCursor;
	public Color defaultVehicleCursorColor;
	public Vector2 defaultVehicleCursorSize;

	public GameObject speedContent;
	public Text currentSpeed;
	public GameObject vehicleControlsMenu;
	public GameObject vehicleControlsMenuElement;

	[HideInInspector] public vehicleHUDElements hudElements;

	GameObject currentVehicle;
	List<GameObject> actionList = new List<GameObject> ();
	vehicleHUDManager currentVehicleHUDManager;

	bool fingerPressingTouchPanel;

	void Start ()
	{
		vehicleControlsMenu.SetActive (true);
		vehicleControlsMenu.SetActive (false);

		hudElements.vehicleHealth = vehicleHealth;
		hudElements.vehicleBoost = vehicleBoost;
		hudElements.vehicleFuel = vehicleFuel;
		hudElements.vehicleAmmo = vehicleAmmo;
		hudElements.weaponName = weaponName;
		hudElements.ammoInfo = ammoInfo;
		hudElements.healthContent = healthContent;
		hudElements.energyContent = energyContent;
		hudElements.ammoContent = ammoContent;
		hudElements.fuelContent = fuelContent;
		hudElements.vehicleCursor = vehicleCursor;
		hudElements.currentSpeed = currentSpeed;
		hudElements.speedContent = speedContent;
		hudElements.defaultVehicleCursor = defaultVehicleCursor;
		hudElements.defaultVehicleCursorColor = defaultVehicleCursorColor;
		hudElements.defaultVehicleCursorSize = defaultVehicleCursorSize;
	}

	public void setControlList (inputActionManager manager)
	{
		for (int i = 0; i < actionList.Count; i++) {
			Destroy (actionList [i]);
		}

		actionList.Clear ();

		if (!vehicleControlsMenuElement.activeSelf) {
			vehicleControlsMenuElement.SetActive (true);
		}

		if (!vehicleControlsMenu.activeSelf) {
			vehicleControlsMenu.SetActive (true);
		}

		//every key field in the edit input button has an editButtonInput component, so create every of them
		for (int i = 0; i < manager.multiAxesList.Count; i++) {
			for (int j = 0; j < manager.multiAxesList [i].axes.Count; j++) {

				inputActionManager.Axes currentAxes = manager.multiAxesList [i].axes [j];

				if (currentAxes.showInControlsMenu) {
					GameObject buttonClone = (GameObject)Instantiate (vehicleControlsMenuElement, vehicleControlsMenuElement.transform.position, Quaternion.identity);
					buttonClone.transform.SetParent (vehicleControlsMenuElement.transform.parent);
					buttonClone.transform.localScale = Vector3.one;
					buttonClone.name = currentAxes.Name;
					buttonClone.transform.GetChild (0).GetComponent<Text> ().text = currentAxes.Name;
					buttonClone.transform.GetChild (1).GetComponentInChildren<Text> ().text = currentAxes.keyButton;
					actionList.Add (buttonClone);
				}
			}
		}

		//get the scroller in the edit input menu
		Scrollbar scroller = vehicleControlsMenu.GetComponentInChildren<Scrollbar> ();

		//set the scroller in the top position
		scroller.value = 1;

		//disable the menu
		vehicleControlsMenu.SetActive (false);
		vehicleControlsMenuElement.SetActive (false);
	}

	public void setTouchingMenuPanelState (bool state)
	{
		fingerPressingTouchPanel = state;
	}

	public bool isFingerPressingTouchPanel ()
	{
		return fingerPressingTouchPanel;
	}

	public void openOrCloseControlsMenu (bool state)
	{
		if (vehicleControlsMenu.activeSelf != state) {
			vehicleControlsMenu.SetActive (state);
		}
	}

	public void setCurrentVehicleHUD (GameObject vehicle)
	{
		currentVehicle = vehicle;

		if (currentVehicle) {
			currentVehicleHUDManager = currentVehicle.GetComponent<vehicleHUDManager> ();
		} else {
			currentVehicleHUDManager = null;
		}
	}

	public void closeControlsMenu ()
	{
		if (currentVehicleHUDManager != null) {
			currentVehicleHUDManager.IKDrivingManager.openOrCloseControlsMenu (false);
		}
	}

	public void destroyCurrentVehicle ()
	{
		if (currentVehicleHUDManager != null) {
			if (!currentVehicleHUDManager.vehicleIsDestroyed () && currentVehicleHUDManager.isVehicleBeingDriven ()) {
				currentVehicleHUDManager.destroyVehicle ();
			}
		}
	}

	public void damageCurrentVehicle (float damageAmount)
	{
		if (currentVehicleHUDManager != null) {
			if (!currentVehicleHUDManager.vehicleIsDestroyed () && currentVehicleHUDManager.isVehicleBeingDriven ()) {
				currentVehicleHUDManager.damageVehicle (damageAmount);
			}
		}
	}

	public void takeEnergyCurrentVehicle (float energyAmount)
	{
		if (currentVehicleHUDManager != null) {
			if (!currentVehicleHUDManager.vehicleIsDestroyed () && currentVehicleHUDManager.isVehicleBeingDriven ()) {
				currentVehicleHUDManager.removeEnergy (energyAmount);
			}
		}
	}

	public void takeFuelCurrentVehicle (float fuelAmount)
	{
		if (currentVehicleHUDManager != null) {
			if (!currentVehicleHUDManager.vehicleIsDestroyed () && currentVehicleHUDManager.isVehicleBeingDriven ()) {
				currentVehicleHUDManager.removeFuel (fuelAmount);
			}
		}
	}

	public void giveHealthCurrentVehicle (float healthAmount)
	{
		if (currentVehicleHUDManager != null) {
			if (!currentVehicleHUDManager.vehicleIsDestroyed () && currentVehicleHUDManager.isVehicleBeingDriven ()) {
				currentVehicleHUDManager.getHealth (healthAmount);
			}
		}
	}

	public void giveEnergyCurrentVehicle (float energyAmount)
	{
		if (currentVehicleHUDManager != null) {
			if (!currentVehicleHUDManager.vehicleIsDestroyed () && currentVehicleHUDManager.isVehicleBeingDriven ()) {
				currentVehicleHUDManager.getEnergy (energyAmount);
			}
		}
	}

	public void giveFuelCurrentVehicle (float fuelAmount)
	{
		if (currentVehicleHUDManager != null) {
			if (!currentVehicleHUDManager.vehicleIsDestroyed () && currentVehicleHUDManager.isVehicleBeingDriven ()) {
				currentVehicleHUDManager.getFuel (fuelAmount);
			}
		}
	}

	public void setInfiniteHealthCurrentVehicleState (bool state)
	{
		if (currentVehicleHUDManager != null) {
			if (!currentVehicleHUDManager.vehicleIsDestroyed () && currentVehicleHUDManager.isVehicleBeingDriven ()) {
				currentVehicleHUDManager.setInvencibleState (state);
			}
		}
	}

	public void setInfiniteEnergyCurrentVehicleState (bool state)
	{
		if (currentVehicleHUDManager != null) {
			if (!currentVehicleHUDManager.vehicleIsDestroyed () && currentVehicleHUDManager.isVehicleBeingDriven ()) {
				currentVehicleHUDManager.setInfiniteEnergyState (state);
			}
		}
	}

	public void setInfiniteFuelCurrentVehicleState (bool state)
	{
		if (currentVehicleHUDManager != null) {
			if (!currentVehicleHUDManager.vehicleIsDestroyed () && currentVehicleHUDManager.isVehicleBeingDriven ()) {
				currentVehicleHUDManager.setInfiniteFuelState (state);
			}
		}
	}

	public void setInfiniteStatCurrentVehicleState (bool state)
	{
		setInfiniteHealthCurrentVehicleState (state);

		setInfiniteEnergyCurrentVehicleState (state);

		setInfiniteFuelCurrentVehicleState (state);
	}

	public vehicleHUDElements getHudElements ()
	{
		return hudElements;
	}

	public GameObject getVehicleCursor ()
	{
		return hudElements.vehicleCursor;
	}

	public void setFreeInteractionDeviceCursorActiveState (bool state)
	{
		if (freeInteractionDeviceCursor != null && freeInteractionDeviceCursor.activeSelf != state) {
			freeInteractionDeviceCursor.SetActive (state);
		}
	}

	public void enableOrDisableHUD (bool state)
	{
		enableOrDisableHUDElement (playerHUDName, state, false);
	}

	public void enableOrDisablePlayerBars (bool state)
	{
		enableOrDisableHUDElement (playerBarsName, state, false);
	}

	public void enableOrDisablePlayerMode (bool state)
	{
		enableOrDisableHUDElement (playerModeName, state, false);
	}

	public void enableOrDisableHUDFromEditor (bool state)
	{
		enableOrDisableHUDElement (playerHUDName, state, true);
	}

	public void enableOrDisablePlayerBarsFromEditor (bool state)
	{
		enableOrDisableHUDElement (playerBarsName, state, true);
	}

	public void enableOrDisablePlayerModeFromEditor (bool state)
	{
		enableOrDisableHUDElement (playerModeName, state, true);
	}

	public void enableOrDisableVehicleHUD (bool state)
	{
		if (vehicleHUD.activeSelf != state) {
			vehicleHUD.SetActive (state);
		}
	}

	public void enableHUDElement (string elementName)
	{
		enableOrDisableHUDElement (elementName, true, false);
	}

	public void disableHUDElement (string elementName)
	{
		enableOrDisableHUDElement (elementName, false, false);
	}

	public void enableHUDElementFromEditor (string elementName)
	{
		enableOrDisableHUDElement (elementName, true, true);
	}

	public void disableHUDElementFromEditor (string elementName)
	{
		enableOrDisableHUDElement (elementName, false, true);
	}

	public void enableOrDisableHUDElement (string elementName, bool state, bool usedFromEditor)
	{
		for (int i = 0; i < HUDElementInfoList.Count; i++) {
			if (usedFromEditor) {
				if (HUDElementInfoList [i].Name.Equals (elementName)) {
					if (HUDElementInfoList [i].HUDElementGameObject.activeSelf != state) {
						HUDElementInfoList [i].HUDElementGameObject.SetActive (state);
					}

					HUDElementInfoList [i].HUDElementEnabled = state;
				}
			} else {
				if (HUDElementInfoList [i].HUDElementEnabled && HUDElementInfoList [i].Name.Equals (elementName)) {
					if (HUDElementInfoList [i].HUDElementGameObject.activeSelf != state) {
						HUDElementInfoList [i].HUDElementGameObject.SetActive (state);
					}
				}
			}
		}
	}

	public bool isHUDElementEnabled (string elementName)
	{
		for (int i = 0; i < HUDElementInfoList.Count; i++) {
			if (HUDElementInfoList [i].HUDElementEnabled && HUDElementInfoList [i].Name.Equals (elementName)) {
				return true;
			}
		}

		return false;
	}

	[System.Serializable]
	public class vehicleHUDElements
	{
		public Slider vehicleHealth;
		public Slider vehicleBoost;
		public Slider vehicleFuel;
		public Slider vehicleAmmo;
		public Text weaponName;
		public Text ammoInfo;
		public GameObject healthContent;
		public GameObject energyContent;
		public GameObject ammoContent;
		public GameObject fuelContent;
		public GameObject vehicleCursor;
		public Text currentSpeed;
		public GameObject speedContent;
		public Texture defaultVehicleCursor;
		public Color defaultVehicleCursorColor;
		public Vector2 defaultVehicleCursorSize;
	}

	[System.Serializable]
	public class HUDElementInfo
	{
		public string Name;
		public bool HUDElementEnabled = true;

		public GameObject HUDElementGameObject;
	}
}