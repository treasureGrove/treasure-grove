using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class laserAttachment : laser
{
	[Header ("Main Settings")]
	[Space]

	public LayerMask layer;

	public bool laserEnabled = true;
	public float laserRotationSpeed = 20;
	public bool alwaysLookInCameraDirection;

	public bool useLaserDot;
	public bool useLaserDotIconOnScreen;

	[Space]
	[Header ("Events Settings")]
	[Space]

	public bool sendMessageOnContact;
	public UnityEvent contantFunctions = new UnityEvent ();

	[Space]
	[Header ("Debug")]
	[Space]

	public bool sameObjectFound;
	public bool hittingSurface;

	public	bool laserCanBeUsed;

	public Vector3 hitPointOffset;
	public Transform offsetReference;

	public bool laserDotActive;

	[Space]
	[Header ("Components")]
	[Space]

	public playerWeaponsManager weaponsManager;
	public playerWeaponSystem weaponManager;
	public IKWeaponSystem IkWeaponManager;

	public Transform mainCameraTransform;

	public GameObject laserDot;
	public laserDotOnScreenSystem mainLaserDotOnScreenSystem;

	RaycastHit hit;
	Vector3 hitPointPosition;
	float rayDistance;

	GameObject lastObjectDetected;

	Quaternion targetRotation;
	Vector3 direction;

	GameObject objectDetectedByCamera;
	GameObject objectDetectedByLaser;

	Vector3 hitPointCameraDirection;

	Vector3 currentHitNormal;

	Vector3 currentTransformPosition;

	Vector3 currentCameraPosition;

	Vector3 currentForwardDirection;

	bool useForwardDirectionOnLaserAttachments;

	bool firstPersonActive;

	bool firstPersonViewFirstCheck;

	void Start ()
	{
		rayDistance = Mathf.Infinity;
	}

	void Update ()
	{
		if (laserEnabled) {

			useForwardDirectionOnLaserAttachments = weaponsManager.isUseForwardDirectionOnLaserAttachmentsActive ();

			if (mainCameraTransform != null) {
				
				laserCanBeUsed = !weaponManager.weaponIsMoving () &&
				(weaponManager.aimingInThirdPerson || weaponManager.carryingWeaponInFirstPerson) &&
				!weaponsManager.isEditinWeaponAttachments () &&
				(!weaponManager.isWeaponOnRecoil () || alwaysLookInCameraDirection)	&&
				!IkWeaponManager.isWeaponSurfaceDetected () &&
				!IkWeaponManager.isWeaponInRunPosition () &&
				!IkWeaponManager.isMeleeAtacking ();

				currentTransformPosition = transform.position;

				currentCameraPosition = mainCameraTransform.position;

				if (laserCanBeUsed) {
					if (Physics.Raycast (currentCameraPosition, mainCameraTransform.TransformDirection (Vector3.forward), out hit, rayDistance, layer)) {
						//Debug.DrawLine (mainCameraTransform.position, hit.point, Color.white, 2);
						direction = hit.point - currentTransformPosition;
						direction = direction / direction.magnitude;
						targetRotation = Quaternion.LookRotation (direction);

						objectDetectedByCamera = hit.collider.gameObject;

						if (!IkWeaponManager.isWeaponSurfaceDetected ()) {
							hitPointCameraDirection = direction;
						} else {
							hitPointCameraDirection = hit.point - currentCameraPosition;
							hitPointCameraDirection = hitPointCameraDirection / hitPointCameraDirection.magnitude;
						}

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
			} else {
				mainCameraTransform = weaponManager.getMainCameraTransform ();
			}

			lRenderer.positionCount = 2;
			lRenderer.SetPosition (0, currentTransformPosition);

			if (useForwardDirectionOnLaserAttachments) {
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
						if (contantFunctions.GetPersistentEventCount () > 0) {
							contantFunctions.Invoke ();
						}
					}
				}

				currentHitNormal = hit.normal;
			} else {
				//the laser does not hit anything, so disable the shield if it was enabled
				hittingSurface = false;
			}

			if (Physics.Raycast (currentTransformPosition, hitPointCameraDirection, out hit, rayDistance, layer)) {
				objectDetectedByLaser = hit.collider.gameObject;

//				Debug.DrawLine (transform.position, hit.point, Color.white, 2);
			} else {
				objectDetectedByLaser = null;
			}

			if (objectDetectedByCamera == objectDetectedByLaser || (objectDetectedByCamera == null && objectDetectedByLaser == null)) {
				sameObjectFound = true;
			} else {
				sameObjectFound = false;
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
							if (mainLaserDotOnScreenSystem != null) {
								mainLaserDotOnScreenSystem.setLasetDotIconActiveState (true);
							}
						} else {
							laserDot.SetActive (true);
						}

						laserDotActive = true;
					}

					laserDot.transform.position = hitPointPosition + currentHitNormal * 0.01f;

					laserDot.transform.rotation = Quaternion.LookRotation (currentHitNormal, transform.up);

					if (weaponManager.carryingWeaponInFirstPerson != firstPersonActive || !firstPersonViewFirstCheck) {
						firstPersonActive = weaponManager.carryingWeaponInFirstPerson;

						if (firstPersonActive) {
							laserDot.transform.SetParent (transform.parent);

							weaponsManager.setWeaponPartLayerFromCameraView (laserDot, true);
						} else {
							laserDot.transform.SetParent (weaponsManager.getTemporalParentForWeapons ());

							weaponsManager.setWeaponPartLayerFromCameraView (laserDot, false);
						}

						firstPersonViewFirstCheck = true;
					}

					if (useLaserDotIconOnScreen) {
						if (mainLaserDotOnScreenSystem != null) {
							mainLaserDotOnScreenSystem.updateLaserDotPosition (hitPointPosition);
						}
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
							if (mainLaserDotOnScreenSystem != null) {
								mainLaserDotOnScreenSystem.setLasetDotIconActiveState (false);
							}
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

		initializeComponents ();

		if (laserEnabled) {
			if (weaponsManager == null) {
				weaponsManager = weaponManager.getPlayerWeaponsManger ();
			}
		}

		if (lRenderer) {
			lRenderer.enabled = state;
		}

		transform.localRotation = Quaternion.identity;

		if (useLaserDot) {
			if (!laserEnabled) {
				if (useLaserDotIconOnScreen) {
					if (mainLaserDotOnScreenSystem != null) {
						mainLaserDotOnScreenSystem.setLasetDotIconActiveState (false);
					}
				} else {
					laserDot.SetActive (false);
				}
			}
		}

		firstPersonViewFirstCheck = false;
	}

	bool componentsInitialized;

	void initializeComponents ()
	{
		if (componentsInitialized) {
			return;
		}

		if (weaponManager != null) {
			GameObject playerControllerGameObject = weaponManager.getPlayerWeaponsManger ().gameObject;

			playerComponentsManager mainPlayerComponentsManager = playerControllerGameObject.GetComponent<playerComponentsManager> ();

			if (mainPlayerComponentsManager != null) {

				mainCameraTransform = mainPlayerComponentsManager.getPlayerCamera ().getCameraTransform ();
			}
		}

		componentsInitialized = true;
	}
}