using UnityEngine;
using System.Collections;

public class mechanismPart : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool enableRotation;

	public int mechanimType;

	public Vector3 rotateDirection;

	public float rotationSpeed = 10;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool gearActivated;
	public bool rotatoryGearEngaged;

	[Space]
	[Header ("Objects Settings")]
	[Space]

	public GameObject rotor;

	public GameObject gear;

	grabbedObjectState currentGrabbedObject;
	electronicDevice electronicDeviceManager;

	void Start ()
	{
		electronicDeviceManager = GetComponent<electronicDevice> ();
	}

	void Update ()
	{
		//the script checks if the object on rails has been engaged
		if (enableRotation && mechanimType == 0) {
			rotor.transform.Rotate (rotateDirection * (-rotationSpeed * Time.deltaTime));
		}

		if (enableRotation && mechanimType == 1) {
			if (rotatoryGearEngaged && gear != null) {
				gear.transform.Rotate (new Vector3 (0, 0, rotationSpeed * Time.deltaTime));

				rotor.transform.Rotate (rotateDirection * (-rotationSpeed * Time.deltaTime));
			}

			if (gear && gearActivated) {
				float currentAngle = Vector3.SignedAngle (gear.transform.up, transform.up, gear.transform.forward);
//				if (Mathf.Abs (gear.transform.localEulerAngles.z) > 350) {

				if (Mathf.Abs (currentAngle) < 10) {
					//if the object is being carried by the player, make him drop it
					currentGrabbedObject = gear.GetComponent<grabbedObjectState> ();

					if (currentGrabbedObject != null) {
						GKC_Utils.dropObject (currentGrabbedObject.getCurrentHolder (), gear);
					}

					gear.tag = "Untagged";

					gearActivated = false;

					rotatoryGearEngaged = true;

					electronicDeviceManager.unlockObject ();
				}
			}
		}
	}

	public void setVelocity (float v)
	{
		rotationSpeed = v;
	}

	public void setEnableRotationState (bool state)
	{
		enableRotation = state;
	}

	public void setGearActivatedState (bool state)
	{
		gearActivated = state;

		rotatoryGear currentRotatoryGear = gear.GetComponent<rotatoryGear> ();

		if (currentRotatoryGear != null) {
			currentRotatoryGear.setRotationEnabledState (true);

			gear.tag = "box";
		}
	}
}