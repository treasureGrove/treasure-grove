using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class overrideCameraController : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool cameraControllerEnabled = true;
	public Transform targetToFollow;
	public Transform pivotTransform;
	public Transform cameraTransform;
	public bool cameraActive;

	public Vector2 xLimits;
	public float rotationSpeed = 10;
	public float cameraRotationSpeedVertical = 5;
	public float cameraRotationSpeedHorizontal = 5;

	public LayerMask layer;
	public float clipCastRadius;
	public float backClipSpeed;

	[Space]
	[Header ("Other Settings")]
	[Space]

	public float controllerRadius = 2;
	public bool reapearInCertainPosition;
	public Transform positionToReapear;

	[Space]
	[Header ("Gizmo Settings")]
	[Space]

	public bool showGizmo;
	public Color gizmoColor = Color.yellow;

	[Space]
	[Header ("Components")]
	[Space]

	public overrideInputManager overrideInput;

	Vector2 axisValues;
	Vector2 mouseAxis;
	Vector2 lookAngle;

	Quaternion currentPivotRotation;
	float currentCameraUpRotation;

	Ray ray;
	RaycastHit[] hits;
	float currentCameraDistance;
	float originalCameraDistance;
	float cameraSpeed;

	Coroutine updateCoroutine;

	void Start ()
	{
		originalCameraDistance = cameraTransform.localPosition.magnitude;
	}
		
	public void stopUpdateCoroutine ()
	{
		if (updateCoroutine != null) {
			StopCoroutine (updateCoroutine);
		}
	}

	IEnumerator updateSystemCoroutine ()
	{
		var waitTime = new WaitForSecondsRealtime (0.0001f);

		while (true) {
			updateSystem ();

			yield return waitTime;
		}
	}

	void updateSystem ()
	{
		if (cameraControllerEnabled && cameraActive && !overrideInput.isGamePaused ()) {
			transform.position = targetToFollow.position;

			//get the current input axis values from the input manager
			axisValues = overrideInput.getCustomMouseAxis ();
			mouseAxis.x = axisValues.x;
			mouseAxis.y = axisValues.y;

			//if the first camera view is enabled
			lookAngle.x = mouseAxis.x * rotationSpeed;
			lookAngle.y -= mouseAxis.y * rotationSpeed;

			//clamp these values to limit the camera rotation
			lookAngle.y = Mathf.Clamp (lookAngle.y, xLimits.x, xLimits.y);

			currentPivotRotation = Quaternion.Euler (lookAngle.y, 0, 0);

			pivotTransform.localRotation = Quaternion.Slerp (pivotTransform.localRotation, currentPivotRotation, cameraRotationSpeedVertical * Time.deltaTime);

			currentCameraUpRotation = Mathf.Lerp (currentCameraUpRotation, lookAngle.x, cameraRotationSpeedHorizontal * Time.deltaTime);

			transform.Rotate (0, currentCameraUpRotation, 0);

			//get the current camera position for the camera collision detection
			currentCameraDistance = checkCameraCollision ();

			//set the local camera position
			currentCameraDistance = Mathf.Clamp (currentCameraDistance, 0, originalCameraDistance);
			cameraTransform.localPosition = -Vector3.forward * currentCameraDistance;
		}
	}

	public Transform getCameraTransform ()
	{
		return cameraTransform;
	}

	public void setCameraActiveState (bool state)
	{
		if (!cameraControllerEnabled) {
			return;
		}

		cameraActive = state;

		stopUpdateCoroutine ();

		if (!cameraActive) {
			lookAngle = Vector2.zero;
			mouseAxis = Vector2.zero;
			axisValues = Vector2.zero;
			currentPivotRotation = Quaternion.identity;

			currentCameraUpRotation = 0;

			pivotTransform.localRotation = Quaternion.identity;

			transform.rotation = Quaternion.identity;
		}

		if (cameraActive) {
			updateCoroutine = StartCoroutine (updateSystemCoroutine ());
		}
	}

	public bool isCameraControllerEnabled ()
	{
		return cameraControllerEnabled;
	}

	public void setParentState (bool state)
	{
		if (targetToFollow != null) {
			if (state) {
				transform.SetParent (null);
				transform.position = targetToFollow.position;
			} else {
				transform.SetParent (targetToFollow);
			}
		}
	}

	public void resetLocalRotationPosition ()
	{
		transform.localRotation = Quaternion.identity;
		transform.localPosition = Vector3.zero;
	}

	public void resetRotation (Quaternion newRotation)
	{
		transform.eulerAngles = new Vector3 (0, newRotation.eulerAngles.y, 0);
	}

	//adjust the camera position to avoid cross any collider
	public float checkCameraCollision ()
	{
		//launch a ray from the pivot position to the camera direction
		ray = new Ray (pivotTransform.position, -pivotTransform.forward);
	
		//store the hits received
		hits = Physics.SphereCastAll (ray, clipCastRadius, originalCameraDistance + clipCastRadius, layer);
		float closest = Mathf.Infinity;
		float hitDist = originalCameraDistance;

		//find the closest
		for (int i = 0; i < hits.Length; i++) {
			if (hits [i].distance < closest && !hits [i].collider.isTrigger) {
				//the camera will be moved that hitDist in its forward direction
				closest = hits [i].distance;
				hitDist = -pivotTransform.InverseTransformPoint (hits [i].point).z;
			}
		}

		//clamp the hidDist value
		if (hitDist < 0) {
			hitDist = 0;
		}

		if (hitDist > originalCameraDistance) {
			hitDist = originalCameraDistance;
		}

		//return the value of the collision in the camera
		return Mathf.SmoothDamp (currentCameraDistance, hitDist, ref cameraSpeed, currentCameraDistance > hitDist ? 0 : backClipSpeed);
	}

	public float getControllerRadius ()
	{
		return controllerRadius;
	}

	public bool reapearInCertainPositionActive ()
	{
		return reapearInCertainPosition;
	}

	public Vector3 getPositionToReapear ()
	{
		if (positionToReapear != null) {
			return positionToReapear.position;
		} else {
			return Vector3.zero;
		}
	}

	//Draw gizmos
	void OnDrawGizmos ()
	{
		if (!showGizmo) {
			return;
		}

		if (GKC_Utils.isCurrentSelectionActiveGameObject (gameObject)) {
			DrawGizmos ();
		}
	}

	void OnDrawGizmosSelected ()
	{
		DrawGizmos ();
	}

	void DrawGizmos ()
	{
		if (showGizmo) {
			Gizmos.color = gizmoColor;
			Gizmos.DrawWireSphere (transform.position, controllerRadius);
		}
	}
}
