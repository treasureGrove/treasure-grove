using UnityEngine;
using System.Collections;

public class securityCamera : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public float sensitivity;
	public Vector2 clampTiltY;
	public Vector2 clampTiltX;
	public Vector2 zoomLimit;

	public bool activated;
	public float zoomSpeed;

	public bool controlOverriden;
	public float inputRotationSpeed = 5;
	public float overrideRotationSpeed = 10;

	[Space]
	[Header ("Components")]
	[Space]

	public GameObject baseX;
	public GameObject baseY;

	public overrideInputManager overrideInput;
	public Camera cam;

	[HideInInspector] public Vector2 lookAngle;
	Vector2 axisValues;

	float originalFov;

	Vector2 currentLookAngle;
	float horizontalInput;
	float verticalInput;

	Quaternion currentBaseXRotation;
	Quaternion currentBaseYRotation;

	void Start ()
	{
		//get the camera in the children, store the origianl fov and disable it
		if (cam == null) {
			cam = GetComponentInChildren<Camera> ();
		}

		cam.enabled = false;

		originalFov = cam.fieldOfView;

		if (overrideInput == null) {
			overrideInput = GetComponent<overrideInputManager> ();
		}
	}

	void Update ()
	{
		//if the camera is being used
		if (activated) {
			//get the look angle value
			lookAngle.x += axisValues.x * sensitivity;
			lookAngle.y += axisValues.y * sensitivity;
			//clamp these values to limit the camera rotation
			lookAngle.y = Mathf.Clamp (lookAngle.y, -clampTiltX.x, clampTiltX.y);
			lookAngle.x = Mathf.Clamp (lookAngle.x, -clampTiltY.x, clampTiltY.y);
			//set every angle in the camera and the pivot
			baseX.transform.localRotation = Quaternion.Euler (-lookAngle.y, 0, 0);
			baseY.transform.localRotation = Quaternion.Euler (0, lookAngle.x, 0);
		}

		if (controlOverriden) {
			axisValues = overrideInput.getCustomMovementAxis ();

			axisValues += overrideInput.getCustomMouseAxis ();

			horizontalInput = axisValues.x;
			verticalInput = axisValues.y;

			currentLookAngle.x -= verticalInput * inputRotationSpeed;
			currentLookAngle.y += horizontalInput * inputRotationSpeed;

			currentLookAngle.x = Mathf.Clamp (currentLookAngle.x, -clampTiltX.x, clampTiltX.y);
			currentLookAngle.y = Mathf.Clamp (currentLookAngle.y, -clampTiltY.x, clampTiltY.y);

			currentBaseXRotation = Quaternion.Euler (currentLookAngle.x, 0, 0);
			currentBaseYRotation = Quaternion.Euler (0, currentLookAngle.y, 0);

			baseX.transform.localRotation = Quaternion.Slerp (baseX.transform.localRotation, currentBaseXRotation, Time.deltaTime * overrideRotationSpeed);
			baseY.transform.localRotation = Quaternion.Slerp (baseY.transform.localRotation, currentBaseYRotation, Time.deltaTime * overrideRotationSpeed);
		}
	}

	//the camera is being rotated, so set the axis values
	public void getLookValue (Vector2 currentAxisValues)
	{
		axisValues = currentAxisValues;
	}

	//the zoom is being used, so change the fov according to the type of zoom, in or out
	public void setZoom (int mult)
	{
		float zoomValue = cam.fieldOfView;
		zoomValue += Time.deltaTime * mult * zoomSpeed;
		zoomValue = Mathf.Clamp (zoomValue, zoomLimit.x, zoomLimit.y);
		cam.fieldOfView = zoomValue;
	}

	//enable or disable the camera according to if the control is being using if a computer device
	public void changeCameraState (bool state)
	{
		activated = state;

		if (cam != null) {
			cam.enabled = state;

			if (!activated) {
				cam.fieldOfView = originalFov;
			}
		}
	}

	public void startOverride ()
	{
		overrideControlState (true);
	}

	public void stopOverride ()
	{
		overrideControlState (false);
	}

	public void overrideControlState (bool state)
	{
		if (state) {
			currentLookAngle = new Vector2 (baseX.transform.localRotation.y, baseY.transform.localRotation.x);
		} else {
			currentLookAngle = Vector2.zero;
			axisValues = Vector2.zero;
		}

		controlOverriden = state;
	}
}