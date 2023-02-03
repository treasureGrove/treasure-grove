using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;

public class shipInterfaceInfo : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool vehicleInterfaceEnabled = true;

	public GameObject vehicle;

	public bool compassEnabled;

	public string speedExtraString = " km/h";

	[Space]
	[Header ("Compass Elements")]
	[Space]

	public RectTransform compassBase;
	public RectTransform north;
	public RectTransform south;
	public RectTransform east;
	public RectTransform west;
	public RectTransform altitudeMarks;

	[Space]
	[Header ("Other UI Elements")]
	[Space]

	public GameObject interfaceCanvas;
	public Text pitchValue;
	public Text yawValue;
	public Text rollValue;
	public Text altitudeValue;
	public Text velocityValue;
	public Text coordinateXValue;
	public Text coordinateYValue;
	public Text coordinateZValue;
	public RectTransform level;
	public float altitudeMarkSpeed;
	public Slider healthBar;
	public Slider energyBar;
	public Slider fuelBar;
	public Text weaponName;
	public Text weaponAmmo;
	public Text canLand;
	public Text enginesState;

	public GameObject healthContent;
	public GameObject energyContet;
	public GameObject fuelContent;
	public GameObject weaponContent;

	[Space]
	[Header ("Componets")]
	[Space]

	public vehicleHUDManager HUDManager;
	public vehicleGravityControl gravityManager;
	public vehicleWeaponSystem weaponManager;
	public Rigidbody mainRigidbody;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool interfaceActive;

	int compassDirection;
	int compassDirectionAux;
	Vector3 normal;
	float currentSpeed;
	bool showHealth;
	bool showEnergy;
	bool showFuel;
	bool hasWeapons;
	Transform vehicleTransform;

	bool weaponsManagerLocated;

	bool altitudeMarksLocated;

	void Start ()
	{
		weaponsManagerLocated = weaponManager != null;

		altitudeMarksLocated = (altitudeMarks != null);

		if (weaponsManagerLocated) {
			if (!weaponManager.enabled) {
				weaponContent.SetActive (false);
			} else {
				hasWeapons = true;
			}
		} else {
			if (weaponContent != null) {
				weaponContent.SetActive (false);
			}
		}

		healthBar.maxValue = HUDManager.healthAmount;
		healthBar.value = healthBar.maxValue;
		energyBar.maxValue = HUDManager.boostAmount;
		energyBar.value = energyBar.maxValue;

		if (!HUDManager.invincible) {
			showHealth = true;
		} else {
			if (healthContent != null) {
				healthContent.SetActive (false);
			}
		}

		if (!HUDManager.infiniteBoost) {
			showEnergy = true;
		} else {
			if (energyContet != null) {
				energyContet.SetActive (false);
			}
		}

		if (HUDManager.vehicleUseFuel && !HUDManager.infiniteFuel) {
			showFuel = true;
		} else {
			if (fuelContent != null) {
				fuelContent.SetActive (false);
			}
		}

		vehicleTransform = vehicle.transform;

		enableOrDisableInterface (false);
	}

	void Update ()
	{
		if (interfaceActive) {
			currentSpeed = mainRigidbody.velocity.magnitude;

			if (compassEnabled) {
				compassDirection = (int)Mathf.Abs (vehicleTransform.eulerAngles.y);

				if (compassDirection > 360) {
					compassDirection = compassDirection % 360;
				}

				compassDirectionAux = compassDirection;

				if (compassDirectionAux > 180) {
					compassDirectionAux = compassDirectionAux - 360;
				}

				north.anchoredPosition = new Vector2 (-compassDirectionAux * 2, 0);
				south.anchoredPosition = new Vector2 (-compassDirection * 2 + 360, 0);
				east.anchoredPosition = new Vector2 (-compassDirectionAux * 2 + 180, 0);
				west.anchoredPosition = new Vector2 (-compassDirection * 2 + 540, 0);
				normal = gravityManager.currentNormal;

				if (altitudeMarksLocated) {
					float angleX = Mathf.Asin (vehicleTransform.InverseTransformDirection (Vector3.Cross (normal.normalized, vehicleTransform.up)).x) * Mathf.Rad2Deg;
					altitudeMarks.anchoredPosition = Vector2.MoveTowards (altitudeMarks.anchoredPosition, new Vector2 (0, angleX), Time.deltaTime * altitudeMarkSpeed);
				}
			}

			if (pitchValue != null) {
				pitchValue.text = vehicleTransform.eulerAngles.x.ToString ("0") + " º";
			}

			if (yawValue != null) {
				yawValue.text = vehicleTransform.eulerAngles.y.ToString ("0") + " º";
			}

			if (rollValue != null) {
				rollValue.text = vehicleTransform.eulerAngles.z.ToString ("0") + " º";
			}

			if (altitudeValue != null) {
				altitudeValue.text = vehicleTransform.position.y.ToString ("0") + " m";
			}

			if (velocityValue != null) {
				velocityValue.text = currentSpeed.ToString ("0") + speedExtraString;
			}

			if (coordinateXValue != null) {
				coordinateXValue.text = vehicleTransform.position.x.ToString ("0"); 
			}

			if (coordinateYValue != null) {
				coordinateYValue.text = vehicleTransform.position.y.ToString ("0"); 
			}

			if (coordinateZValue != null) {
				coordinateZValue.text = vehicleTransform.position.z.ToString ("0"); 
			}

			if (level != null) {
				level.localEulerAngles = new Vector3 (0, 0, vehicleTransform.eulerAngles.z);
			}

			if (hasWeapons && weaponsManagerLocated) {
				weaponName.text = weaponManager.getCurrentWeaponName ();
				weaponAmmo.text = weaponManager.getCurrentWeaponClipSize () + "/" + weaponManager.getCurrentWeaponRemainAmmo ();
			}

			if (showHealth) {
				healthBar.value = HUDManager.getCurrentHealthAmount ();
			}

			if (showEnergy) {
				energyBar.value = HUDManager.getCurrentEnergyAmount ();
			}

			if (showFuel) {
				fuelBar.value = HUDManager.getCurrentFuelAmount ();
			}
		}
	}

	public void enableOrDisableInterface (bool state)
	{
		if (!vehicleInterfaceEnabled) {
			return;
		}

		interfaceActive = state;

		if (interfaceCanvas.activeSelf != interfaceActive) {
			interfaceCanvas.SetActive (interfaceActive);
		}
	}

	public void shipCanLand (bool state)
	{
		if (state) {
			canLand.text = "YES";
		} else {
			canLand.text = "NO";
		}
	}

	public void shipEnginesState (bool state)
	{
		if (state) {
			enginesState.text = "ON";
		} else {
			enginesState.text = "OFF";
		}
	}
}