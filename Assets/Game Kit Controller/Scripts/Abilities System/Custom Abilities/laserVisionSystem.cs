using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class laserVisionSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool laserVisionEnabled = true;

	public bool sliceWithLaserEnabled = true;

	public float sliceObjectsActiveRate = 0.5f;

	public LayerMask mainLayermask;

	public float laserRaycastDistance;

	public float minLaserMovementToActivateSlice;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool laserVisionActive;

	[Space]
	[Header ("Event Settings")]
	[Space]

	public UnityEvent eventOnLaserVisionEnabled;
	public UnityEvent eventOnLaserVisionDisabled;

	public Transform cutParentRefence;

	[Space]
	[Header ("Components")]
	[Space]

	public sliceSystem mainSliceSystem;

	public Transform laserPositionTransform;

	public Transform laserDirectionTransform;

	public GameObject laserGameObject;

	public Vector3 cutOverlapBoxSize;
	public Transform cutPositionTransform;
	public Transform cutDirectionTransform;

	public Transform planeDefiner1;
	public Transform planeDefiner2;
	public Transform planeDefiner3;


	float lastTimeSliceActive;

	Vector3 currentLaserPosition;
	Vector3 previousLaserPosition;

	RaycastHit hit;


	void Update ()
	{
		if (laserVisionActive && sliceWithLaserEnabled) {
			if (Time.time > sliceObjectsActiveRate + lastTimeSliceActive) {
				if (GKC_Utils.distance (currentLaserPosition, previousLaserPosition) > minLaserMovementToActivateSlice) {
					Vector3 laserDirection = currentLaserPosition - previousLaserPosition;

					float laserAngle = Vector3.SignedAngle (cutParentRefence.right, laserDirection, cutParentRefence.forward);

					cutParentRefence.eulerAngles = cutParentRefence.forward * laserAngle;

					cutParentRefence.localEulerAngles = new Vector3 (0, 0, cutParentRefence.localEulerAngles.z);

					mainSliceSystem.setCustomCutTransformValues (cutOverlapBoxSize, cutPositionTransform, cutDirectionTransform,
						planeDefiner1, planeDefiner2, planeDefiner3);

					mainSliceSystem.activateCutExternally ();

					lastTimeSliceActive = Time.time;

					previousLaserPosition = currentLaserPosition;
				}
			}

			if (Physics.Raycast (laserPositionTransform.position, laserDirectionTransform.forward, out hit, laserRaycastDistance, mainLayermask)) {
				currentLaserPosition = hit.point;

				cutParentRefence.position = hit.point - cutParentRefence.forward * 1.5f;

				if (previousLaserPosition == Vector3.zero) {
					previousLaserPosition = hit.point;
				}
			}
		}
	}

	public void setLaserVisionActiveState (bool state)
	{
		if (!laserVisionEnabled) {
			return;
		}

		laserVisionActive = state;

		laserGameObject.SetActive (state);

		lastTimeSliceActive = 0;

		previousLaserPosition = Vector3.zero;

		if (laserVisionActive) {
			eventOnLaserVisionEnabled.Invoke ();
		} else {
			eventOnLaserVisionDisabled.Invoke ();
		}
	}

	public void toggleLaserVisionActiveState ()
	{
		setLaserVisionActiveState (!laserVisionActive);
	}
}
