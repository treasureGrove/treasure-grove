using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class objectToAttractWithGrapplingHook : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool attractObjectEnabled = true;

	public bool attractPlayerEnabled;

	public bool enableGravityOnDeactivateAttract = true;

	public bool useReducedVelocityOnDisableAttract;
	public float maxReducedSpeed;
	public float reducedSpeedDuration;
	public float newSpeedAfterReducedDurationMultiplier = 1;

	public bool ignoreApplySpeedOnHookStopOnPlayer;

	public bool resetPlayerSpeedOnHookStop;

	[Space]
	[Header ("Auto Grab Settings")]
	[Space]

	public bool autoGrabObjectOnCloseDistance;
	public float minDistanceToAutoGrab;

	[Space]
	[Header ("Activate Interaction Action Settings")]
	[Space]

	public bool activateInteractionActionWithObject;
	public float minDistanceToActivateInteractionActionWithObject;
	public GameObject objectToActivate;

	[Space]
	[Header ("Offset Position Settings")]
	[Space]

	public bool useHookTargetPostionOffset;
	public Vector3 hookTargetPositionOffset;

	[Space]
	[Header ("Rigidbody Elements")]
	[Space]

	public Rigidbody mainRigidbody;

	public bool useRigidbodyList;
	public List<Rigidbody> rigidbodyList = new List<Rigidbody> ();

	[Space]
	[Header ("Custom Settings")]
	[Space]

	public bool useCustomForceAttractionValues;
	public float customMinDistanceToStopAttractObject = 5;
	public bool customAddUpForceForAttraction;
	public float customUpForceForAttraction;
	public float customAddUpForceForAttractionDuration;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool attractingObjectActive;

	[Space]
	[Header ("Event Settings")]
	[Space]

	public bool useEventsOnAttractState;
	public UnityEvent eventOnActivateAttract;
	public UnityEvent eventOnDeactivateAttract;

	public bool useEventAfterReduceSpeed;
	public UnityEvent eventAfterReduceSpeed;

	[Space]
	[Header ("Remote Event Settings")]
	[Space]

	public bool useRemoteEventsOnStateChange;
	public List<string> remoteEventNameListOnStart = new List<string> ();
	public List<string> remoteEventNameListOnEnd = new List<string> ();

	[Space]
	[Header ("Components")]
	[Space]

	public Transform mainTransform;

	[Space]
	[Header ("Gizmo Settings")]
	[Space]

	public bool showGizmo;
	public Color gizmoLabelColor = Color.green;
	public Color gizmoColor = Color.white;
	public float gizmoRadius = 0.3f;

	Coroutine reduceSpeedCoroutine;

	bool attractionHookRemovedByDistance;

	bool reducedSpeedInProcess;

	Vector3 speedDirection;
	Vector3 previousAngularVelocity;
	float previousSpeed;


	public void setAttractionHookRemovedByDistanceState (bool state)
	{
		attractionHookRemovedByDistance = state;
	}

	public bool setAttractObjectState (bool state)
	{
		if (attractObjectEnabled) {
			attractingObjectActive = state;

			if (enableGravityOnDeactivateAttract) {
				if (useRigidbodyList) {
					for (int i = 0; i < rigidbodyList.Count; i++) {
						if (rigidbodyList [i] != null) {
							rigidbodyList [i].useGravity = !state;
						}
					}
				} else {
					mainRigidbody.useGravity = !state;
				}
			}

			if (useEventsOnAttractState) {
				if (attractingObjectActive) {
					eventOnActivateAttract.Invoke ();
				} else {
					eventOnDeactivateAttract.Invoke ();
				}
			}

			if (!attractingObjectActive) {
				checkToReduceSpeed ();
			}

			return attractingObjectActive;
		} else {
			return false;
		}
	}

	public Rigidbody getRigidbodyToAttract ()
	{
		return mainRigidbody;
	}

	public void checkToReduceSpeed ()
	{
		if (useReducedVelocityOnDisableAttract) {
			stopCheckToReduceSpeed ();

			reduceSpeedCoroutine = StartCoroutine (checkToReduceSpeedCoroutine ());
		}
	}

	public void stopCheckToReduceSpeed ()
	{
		if (reduceSpeedCoroutine != null) {
			StopCoroutine (reduceSpeedCoroutine);
		}
	}

	IEnumerator checkToReduceSpeedCoroutine ()
	{
		if (!attractionHookRemovedByDistance) {
			yield return null;
		} else {
			reducedSpeedInProcess = true;

			previousSpeed = mainRigidbody.velocity.magnitude;

			speedDirection = mainRigidbody.velocity.normalized;

			previousAngularVelocity = mainRigidbody.angularVelocity;

			float t = 0;

			bool targetReached = false;

			while (!targetReached) {
				t += Time.deltaTime;

				if (t >= reducedSpeedDuration) {
					targetReached = true;
				}
				
				if (useRigidbodyList) {
					for (int i = 0; i < rigidbodyList.Count; i++) {
						if (rigidbodyList [i] != null) {
							rigidbodyList [i].velocity = speedDirection * maxReducedSpeed;

							rigidbodyList [i].angularVelocity = previousAngularVelocity * maxReducedSpeed;
						}
					}
				} else {
					mainRigidbody.velocity = speedDirection * maxReducedSpeed;

					mainRigidbody.angularVelocity = previousAngularVelocity * maxReducedSpeed;
				}

				yield return null;
			}

			resumeSpeed ();
		}

		if (useEventAfterReduceSpeed) {
			eventAfterReduceSpeed.Invoke ();
		}

		attractionHookRemovedByDistance = false;

		reducedSpeedInProcess = false;
	}

	public void stopGrapplingHookAndResumeValuesWithoutForce ()
	{
		speedDirection = Vector3.zero;
		previousAngularVelocity = Vector3.zero;
		previousSpeed = 0;

		stopGrapplingHookAndResumeValues ();
	}

	public void stopGrapplingHookAndResumeValues ()
	{
		if (!reducedSpeedInProcess) {
			return;
		}

		stopCheckToReduceSpeed ();

		resumeSpeed ();

		if (useEventAfterReduceSpeed) {
			eventAfterReduceSpeed.Invoke ();
		}

		attractionHookRemovedByDistance = false;

		reducedSpeedInProcess = false;
	}

	public void stopGrapplingHookIfActiveOnGrabObject ()
	{
		if (!reducedSpeedInProcess) {
			return;
		}

		stopCheckToReduceSpeed ();

		attractionHookRemovedByDistance = false;

		reducedSpeedInProcess = false;

		speedDirection = Vector3.zero;
		previousAngularVelocity = Vector3.zero;
		previousSpeed = 0;
	}

	void resumeSpeed ()
	{
		if (useRigidbodyList) {
			for (int i = 0; i < rigidbodyList.Count; i++) {
				if (rigidbodyList [i] != null) {
					rigidbodyList [i].velocity = speedDirection * (previousSpeed * newSpeedAfterReducedDurationMultiplier);
				}
			}
		} else {
			mainRigidbody.velocity = speedDirection * (previousSpeed * newSpeedAfterReducedDurationMultiplier);
			mainRigidbody.angularVelocity = previousAngularVelocity * newSpeedAfterReducedDurationMultiplier;
		}

		speedDirection = Vector3.zero;
		previousAngularVelocity = Vector3.zero;
		previousSpeed = 0;
	}

	public void searchRigidbodyElements ()
	{
		useRigidbodyList = true;

		rigidbodyList.Clear ();

		mainRigidbody = null;

		if (mainTransform == null) {
			mainTransform = transform;
		}

		Component[] childrens = mainTransform.GetComponentsInChildren (typeof(Rigidbody));
		foreach (Rigidbody child in childrens) {
			if (child.transform != mainTransform) {
				Collider currentCollider = child.GetComponent<Collider> ();

				if (currentCollider != null && !currentCollider.isTrigger) {
					if (mainRigidbody == null) {
						mainRigidbody = child;
					}

					rigidbodyList.Add (child);
				}
			}
		}

		updateComponent ();
	}

	public void updateComponent ()
	{
		GKC_Utils.updateComponent (this);

		GKC_Utils.updateDirtyScene ("Update Object To Attract with Grappling Hook", gameObject);
	}

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

			if (useHookTargetPostionOffset) {
				if (mainTransform == null) {
					mainTransform = transform;
				}

				if (mainTransform != null) {
					Gizmos.DrawWireSphere (mainTransform.position + hookTargetPositionOffset, gizmoRadius);
				}
			}
		}
	}
}
