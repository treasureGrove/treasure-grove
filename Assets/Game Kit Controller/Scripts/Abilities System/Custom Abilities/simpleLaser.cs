using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;

public class simpleLaser : laser
{
	[Space]
	[Header ("Main Settings")]
	[Space]

	public bool laserEnabled = true;
	public float laserRotationSpeed = 20;

	public bool useForwardDirection;

	[Space]
	[Header ("Objects To Affect Settings")]
	[Space]

	public LayerMask layer;

	public bool useObjectsToIgnoreList;
	public List<GameObject> objectsToIgnoreList = new List<GameObject> ();

	[Space]
	[Header ("Damage Settings")]
	[Space]

	public bool applyDamageActive;
	public float damageAmount;
	public bool ignoreShield;
	public float applyDamageRate;

	public int damageTypeID = -1;

	[Space]
	[Header ("Events Settings")]
	[Space]

	public bool sendMessageOnContact;
	public UnityEvent contactFunctions = new UnityEvent ();

	[Space]
	[Header ("Remote Events Settings")]
	[Space]

	public bool useRemoteEventOnObjectsFound;
	public List<string> remoteEventNameList = new List<string> ();

	[Space]
	[Header ("Debug")]
	[Space]

	public bool sameObjectFound;
	public bool hittingSurface;

	public	bool laserCanBeUsed;

	[Space]
	[Header ("Components")]
	[Space]

	public Vector3 hitPointOffset;
	public Transform offsetReference;

	public bool useLaserDot;
	public bool useLaserDotIconOnScreen;
	public GameObject laserDot;
	public laserDotOnScreenSystem mainLaserDotOnScreenSystem;

	public Transform mainCameraTransform;

	RaycastHit hit;
	Vector3 hitPointPosition;
	float rayDistance;

	GameObject lastObjectDetected;

	Quaternion targetRotation;
	Vector3 direction;

	GameObject objectDetectedByCamera;
	GameObject objectDetectedByLaser;

	Vector3 hitPointCameraDirection;



	bool laserDotActive;

	Vector3 currentHitNormal;

	Vector3 currentTransformPosition;

	Vector3 currentCameraPosition;

	Vector3 currentForwardDirection;

	bool firstPersonActive;

	float lastTimeDamageApplied;

	GameObject lastSurfaceDetected;

	void Start ()
	{
		rayDistance = Mathf.Infinity;
	}

	void Update ()
	{
		if (laserEnabled) {
			animateLaser ();

			currentTransformPosition = transform.position;

			currentCameraPosition = mainCameraTransform.position;

			if (laserCanBeUsed) {
				if (Physics.Raycast (currentCameraPosition, mainCameraTransform.TransformDirection (Vector3.forward), out hit, rayDistance, layer)) {
					//Debug.DrawLine (mainCameraTransform.position, hit.point, Color.white, 2);
					direction = hit.point - currentTransformPosition;
					direction = direction / direction.magnitude;
					targetRotation = Quaternion.LookRotation (direction);

					objectDetectedByCamera = hit.collider.gameObject;

					hitPointCameraDirection = direction;

					currentHitNormal = hit.normal;
				} else {
					targetRotation = Quaternion.LookRotation (mainCameraTransform.forward);

					objectDetectedByCamera = null;

					direction = currentTransformPosition + mainCameraTransform.position * laserDistance;
					direction = direction / direction.magnitude;

					hitPointCameraDirection = direction;
				}

				if (sameObjectFound) {
					transform.rotation = Quaternion.Slerp (transform.rotation, targetRotation, Time.deltaTime * laserRotationSpeed);
				}
			} else {
				if (sameObjectFound) {
					targetRotation = Quaternion.identity;
					transform.localRotation = Quaternion.Slerp (transform.localRotation, targetRotation, Time.deltaTime * laserRotationSpeed);
				}

				objectDetectedByCamera = null;
			}
		

			lRenderer.positionCount = 2;
			lRenderer.SetPosition (0, currentTransformPosition);

			if (useForwardDirection) {
				currentForwardDirection = transform.forward;
			} else {
				currentForwardDirection = hitPointCameraDirection;
			}

			if (Physics.Raycast (currentTransformPosition, currentForwardDirection, out hit, rayDistance, layer)) {
				hittingSurface = true;

				hitPointPosition = hit.point;

				if (hitPointOffset != Vector3.zero && offsetReference) {
					hitPointPosition += Vector3.Scale (offsetReference.up, hitPointOffset);
				}

				if (hit.collider.gameObject != lastObjectDetected) {
					lastObjectDetected = hit.collider.gameObject;

					if (sendMessageOnContact) {
						if (contactFunctions.GetPersistentEventCount () > 0) {
							contactFunctions.Invoke ();
						}
					}
				}

				currentHitNormal = hit.normal;

				if (applyDamageActive) {
					if (Time.time > applyDamageRate + lastTimeDamageApplied) {
						bool canApplyDamage = true;

						if (useObjectsToIgnoreList) {
							if (objectsToIgnoreList.Contains (hit.transform.gameObject)) {
								canApplyDamage = false;
							}
						}

						if (canApplyDamage) {
							applyDamage.checkHealth (gameObject, hit.transform.gameObject, damageAmount, -transform.forward, hit.point, 
								gameObject, true, true, ignoreShield, false, false, -1, damageTypeID);
						}

						lastTimeDamageApplied = Time.time;
					}
				}
			} else {
				//the laser does not hit anything, so disable the shield if it was enabled
				hittingSurface = false;
			}

			if (Physics.Raycast (currentTransformPosition, hitPointCameraDirection, out hit, rayDistance, layer)) {
				objectDetectedByLaser = hit.collider.gameObject;

				//Debug.DrawLine (transform.position, hit.point, Color.red, 2);
			} else {
				objectDetectedByLaser = null;
			}

			if (objectDetectedByCamera == objectDetectedByLaser || (objectDetectedByCamera == null && objectDetectedByLaser == null)) {
				sameObjectFound = true;
			} else {
				sameObjectFound = false;
			}

			if (lastSurfaceDetected != objectDetectedByLaser) {
				lastSurfaceDetected = objectDetectedByLaser;

				if (useRemoteEventOnObjectsFound && lastSurfaceDetected != null) {
					bool canActivateRemoteEvent = true;

					if (useObjectsToIgnoreList) {
						if (objectsToIgnoreList.Contains (lastSurfaceDetected)) {
							canActivateRemoteEvent = false;
						}
					}

					if (canActivateRemoteEvent) {
						remoteEventSystem currentRemoteEventSystem = lastSurfaceDetected.GetComponent<remoteEventSystem> ();

						if (currentRemoteEventSystem != null) {
							for (int i = 0; i < remoteEventNameList.Count; i++) {

								currentRemoteEventSystem.callRemoteEvent (remoteEventNameList [i]);
							}
						}
					}
				}
			}

			if (!sameObjectFound) {
				hittingSurface = false;
				targetRotation = Quaternion.identity;
				transform.localRotation = Quaternion.Slerp (transform.localRotation, targetRotation, Time.deltaTime * laserRotationSpeed);
			}

			if (hittingSurface) {					
				lRenderer.SetPosition (1, hitPointPosition);

				if (useLaserDot) {
					if (!laserDotActive) {
						if (useLaserDotIconOnScreen) {
							mainLaserDotOnScreenSystem.setLasetDotIconActiveState (true);
						} else {
							laserDot.SetActive (true);
						}

						laserDotActive = true;
					}

					laserDot.transform.position = hitPointPosition + currentHitNormal * 0.01f;

					laserDot.transform.rotation = Quaternion.LookRotation (currentHitNormal, transform.up);

					if (useLaserDotIconOnScreen) {
						mainLaserDotOnScreenSystem.updateLaserDotPosition (hitPointPosition);
					}
				}
			} else {
				laserDistance = 1000;	

				if (laserCanBeUsed) {
					lRenderer.SetPosition (1, (mainCameraTransform.position + laserDistance * mainCameraTransform.forward));
				} else {
					lRenderer.SetPosition (1, (currentTransformPosition + laserDistance * transform.forward));
				}

				if (useLaserDot) {
					if (laserDotActive) {
						if (useLaserDotIconOnScreen) {
							mainLaserDotOnScreenSystem.setLasetDotIconActiveState (false);
						} else {
							laserDot.SetActive (false);
						}

						laserDotActive = false;
					}
				}
			}
		}
	}

	public void setLaserEnabledState (bool state)
	{
		laserEnabled = state;

		if (lRenderer != null) {
			lRenderer.enabled = state;
		}

		transform.localRotation = Quaternion.identity;

		if (useLaserDot) {
			if (!laserEnabled) {
				if (useLaserDotIconOnScreen) {
					mainLaserDotOnScreenSystem.setLasetDotIconActiveState (false);
				} else {
					laserDot.SetActive (false);
				}
			}
		}

		lastTimeDamageApplied = 0;
	}
}