using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class noiseSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool useNoise;
	public float noiseRadius;
	public float noiseExpandSpeed;
	public bool useNoiseDetection;
	public LayerMask noiseDetectionLayer;
	[Range (0, 2)] public float noiseDecibels = 1;

	public bool enableOnlyUsedByPlayer;

	public bool forceNoiseDetection;

	public int noiseID = -1;

	public bool activateNoiseAtStart;

	[Space]
	[Header ("Rigidbody Settings")]
	[Space]

	public bool usedOnRigidbody;
	public bool useRigidbodyCollisionSpeed;
	public float maxCollisionSpeedValue;

	public float collisionNoiseMultiplier = 1;

	[Space]
	[Header ("Other Settings")]
	[Space]

	public string mainDecalManagerName = "Decal Manager";

	public string mainNoiseMeshManagerName = "Noise Mesh Manager";

	public bool useNoiseMesh = true;

	[Space]
	[Header ("Gizmo Settings")]
	[Space]

	public bool showNoiseDetectionGizmo;

	noiseMeshSystem noiseMeshManager;
	float extraNoiseRadiusValue;
	float extraNoiseExpandSpeedValue;
	float collisionSpeed;

	bool noiseMeshManagerFound;

	GameObject impactObject;
	decalManager impactDecalManager;


	void Start ()
	{
		if (activateNoiseAtStart) {
			activateNoise ();
		}
	}

	public void activateNoise ()
	{
		if (useNoise) {

			bool canActivateNoise = false;

			bool decalInfoFound = false;

			if (impactObject != null) {
				if (impactDecalManager == null) {
					GKC_Utils.instantiateMainManagerOnSceneWithType (mainDecalManagerName, typeof(decalManager));

					impactDecalManager = FindObjectOfType<decalManager> ();
				}

				decalTypeInformation currentDecalTypeInformation = impactObject.GetComponent<decalTypeInformation> ();

				if (currentDecalTypeInformation != null) {
					decalInfoFound = true;

					if (impactDecalManager.surfaceUseNoise (currentDecalTypeInformation.getDecalImpactIndex ())) {
						canActivateNoise = true;
					}
				} 

				if (!canActivateNoise) {
					health healthManager = impactObject.GetComponent<health> ();

					if (healthManager != null) {
						decalInfoFound = true;

						if (impactDecalManager.surfaceUseNoise (healthManager.getDecalImpactIndex ())) {
							canActivateNoise = true;
						}
					} 
				}

				if (!canActivateNoise) {
					characterDamageReceiver currentCharacterDamageReceiver = impactObject.GetComponent<characterDamageReceiver> ();

					if (currentCharacterDamageReceiver != null) {
						decalInfoFound = true;

						if (impactDecalManager.surfaceUseNoise (currentCharacterDamageReceiver.getHealthManager ().getDecalImpactIndex ())) {
							canActivateNoise = true;
						}
					}
				}

				if (!canActivateNoise) {
					vehicleHUDManager currentVehicleHUDManager = impactObject.GetComponent<vehicleHUDManager> ();

					if (currentVehicleHUDManager != null) {
						decalInfoFound = true;

						if (impactDecalManager.surfaceUseNoise (currentVehicleHUDManager.getDecalImpactIndex ())) {
							canActivateNoise = true;
						}
					}
				}

				if (!canActivateNoise) {
					vehicleDamageReceiver currentVehicleDamageReceiver = impactObject.GetComponent<vehicleDamageReceiver> ();

					if (currentVehicleDamageReceiver != null) {
						decalInfoFound = true;

						if (impactDecalManager.surfaceUseNoise (currentVehicleDamageReceiver.getHUDManager ().getDecalImpactIndex ())) {
							canActivateNoise = true;
						}
					}
				}

				if (!canActivateNoise) {
					canActivateNoise = true;
				}
			} else {
				canActivateNoise = true;
			}

			if (canActivateNoise || !decalInfoFound) {
				if (useNoiseDetection) {
					applyDamage.sendNoiseSignal (noiseRadius + extraNoiseRadiusValue, transform.position, noiseDetectionLayer, 
						noiseDecibels, showNoiseDetectionGizmo, forceNoiseDetection, noiseID);
				}

				if (useNoiseMesh) {
					if (!noiseMeshManagerFound) {
						if (noiseMeshManager == null) {
							GKC_Utils.instantiateMainManagerOnSceneWithType (mainNoiseMeshManagerName, typeof(noiseMeshSystem));

							noiseMeshManager = FindObjectOfType<noiseMeshSystem> ();

							if (noiseMeshManager != null) {
								noiseMeshManagerFound = true;
							}
						}
					}

					if (noiseMeshManagerFound) {
						noiseMeshManager.addNoiseMesh (noiseRadius + extraNoiseRadiusValue, transform.position + (Vector3.up * 0.3f), noiseExpandSpeed + extraNoiseExpandSpeedValue);
					}
				}

				extraNoiseRadiusValue = 0;
				extraNoiseExpandSpeedValue = 0;

				if (enableOnlyUsedByPlayer) {
					useNoise = false;
				}
			}
		}
	}

	public void OnCollisionEnter (Collision col)
	{
		if (useNoise && usedOnRigidbody) {
			impactObject = col.gameObject;

			if (useRigidbodyCollisionSpeed) {

				collisionSpeed = Mathf.Abs (col.relativeVelocity.magnitude * collisionNoiseMultiplier);
				collisionSpeed = Mathf.Clamp (collisionSpeed, 0, maxCollisionSpeedValue);
				extraNoiseRadiusValue = collisionSpeed;
				extraNoiseExpandSpeedValue = extraNoiseRadiusValue * 2;
			}

			activateNoise ();
		}
	}

	public void activateNoiseExternally ()
	{
		if (useNoise) {
			activateNoise ();
		}
	}

	public void setUseNoiseState (bool state)
	{
		useNoise = state;
	}

	public void setimpactObject (GameObject newObject)
	{
		impactObject = newObject;
	}
}
