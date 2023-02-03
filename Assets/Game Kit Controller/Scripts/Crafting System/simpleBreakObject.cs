using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class simpleBreakObject : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool breakObjectEnabled = true;

	public bool canBreakWithCollisionsEnabled = true;
	public float minVelocityToBreak;
	public AudioClip explosionSound;

	public bool breakInPieces;

	public bool objectIsKinematic;

	[Space]
	[Header ("Disable Object Settings")]
	[Space]

	public bool disableMainObject;
	public bool disableMainObjectMesh;
	public GameObject mainObjectToDisable;

	public bool removePiecesParent;

	[Space]
	[Header ("Fade Pieces Settings")]
	[Space]

	public bool fadePiecesEnabled;
	public float fadePiecesSpeed = 5;

	public bool destroyObjectAfterFade;

	public bool destroyFadedPieces;

	public float timeToRemovePieces = 3;

	[Space]
	[Header ("Explosion Damage Settings")]
	[Space]

	public bool useExplosionAroundEnabled;
	public float explosionDamage;
	public bool ignoreShield;
	public float damageRadius;

	public float explosionDelay;

	public bool canDamageToObjectOwner;

	public int damageTypeID = -1;

	[Space]
	[Header ("Explosion Physics Settings")]
	[Space]

	public float explosionForceToPieces = 5;
	public float explosionRadiusToPieces = 30;
	public ForceMode forceModeToPieces = ForceMode.Impulse;

	public float explosionForce = 300;

	[Space]
	[Header ("Other Explosion Settings")]
	[Space]

	public bool pushCharacters = true;

	public bool killObjectsInRadius;

	public ForceMode explosionForceMode;

	public bool userLayerMask;
	public LayerMask layer;

	public bool applyExplosionForceToVehicles = true;
	public float explosionForceToVehiclesMultiplier = 0.2f;

	[Space]
	[Header ("Remote Events Settings")]
	[Space]

	public bool useRemoteEventOnObjectsFound;
	public string remoteEventName;

	[Space]
	[Header ("Events Settings")]
	[Space]

	public bool useEventOnObjectBroken;
	public UnityEvent eventOnObjectBroken;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool canBreakStatePaused;
	public bool broken;

	public bool fadeInProcess;

	[Space]
	[Header ("Gizmo Settings")]
	[Space]

	public bool showGizmo;

	[Space]
	[Header ("Components")]
	[Space]

	public GameObject brokenObjectPiecesParent;
	public bool spawnBrokenObjectPiecesEnabled;

	public GameObject explosionParticles;
	public Shader transparentShader;
	public Rigidbody mainRigidbody;

	public GameObject objectOwner;

	public AudioSource mainAudioSource;


	List<Material> rendererParts = new List<Material> ();
	int i, j;

	List<GameObject> piecesLit = new List<GameObject> ();


	int rendererPartsCount;

	Material currentMaterial;

	Coroutine mainUpdateCoroutine;

	float currentTimeToRemovePieces;


	IEnumerator updateCoroutine ()
	{
		var waitTime = new WaitForFixedUpdate ();

		while (true) {
			updateState ();

			yield return waitTime;
		}
	}

	void updateState ()
	{
		if (fadeInProcess) {
			if (fadePiecesEnabled) {
				if (currentTimeToRemovePieces > 0) {
					currentTimeToRemovePieces -= Time.deltaTime;
				} else {
					rendererPartsCount = rendererParts.Count;

					bool allPiecesFaded = true;

					for (i = 0; i < rendererPartsCount; i++) {
						currentMaterial = rendererParts [i];

						Color alpha = currentMaterial.color;

						alpha.a -= Time.deltaTime / fadePiecesSpeed;

						currentMaterial.color = alpha;

						//once the alpha is 0, remove the gameObject
						if (currentMaterial.color.a > 0) {
							allPiecesFaded = false;
						}
					}

					if (allPiecesFaded) {
						if (destroyObjectAfterFade) {
							Destroy (gameObject);
						} else {
							stopUpdateCoroutine ();

							fadeInProcess = false;
						}

						if (destroyFadedPieces) {
							for (i = 0; i < piecesLit.Count; i++) {
								if (piecesLit [i] != null) {
									Destroy (piecesLit [i]);
								}
							}
						}
					}
				}
			}
		}
	}

	public void fixBrokenObject ()
	{
		if (disableMainObjectMesh) {
			GetComponent<Collider> ().enabled = true;

			GetComponent<MeshRenderer> ().enabled = true;
		} else {
			if (mainObjectToDisable != null) {
				mainObjectToDisable.SetActive (true);
			}
		}

		if (mainRigidbody != null) {
			mainRigidbody.isKinematic = objectIsKinematic;
		}

		broken = false;
	}

	public void activateBreakObject ()
	{
		if (!breakObjectEnabled) {
			return;
		}

		if (broken) {
			return;
		}
			
		if (objectOwner == null) {
			objectOwner = gameObject;
		}

		if (disableMainObject) {
			if (disableMainObjectMesh) {
				GetComponent<Collider> ().enabled = false;

				GetComponent<MeshRenderer> ().enabled = false;
			} else {
				if (mainObjectToDisable != null) {
					mainObjectToDisable.SetActive (false);
				}
			}
		}

		if (!objectIsKinematic) {
			if (mainRigidbody != null) {
				mainRigidbody.isKinematic = true;
			}
		}

		if (fadePiecesEnabled) {
			if (transparentShader == null) {
				transparentShader = Shader.Find ("Legacy Shaders/Transparent/Diffuse");
			}
		}

		if (useExplosionAroundEnabled) {
			Vector3 currentPosition = transform.position;

			applyDamage.setExplosion (currentPosition, damageRadius, userLayerMask, layer, objectOwner, canDamageToObjectOwner, 
				gameObject, killObjectsInRadius, true, false, explosionDamage, pushCharacters, applyExplosionForceToVehicles,
				explosionForceToVehiclesMultiplier, explosionForce, explosionForceMode, true, objectOwner.transform, ignoreShield, 
				useRemoteEventOnObjectsFound, remoteEventName, damageTypeID);
		}

		//create the explosion particles
		if (explosionParticles != null) {
			GameObject explosionParticlesClone = (GameObject)Instantiate (explosionParticles, transform.position, transform.rotation);
			explosionParticlesClone.transform.SetParent (transform);
		}

		if (mainAudioSource != null) {
			mainAudioSource.PlayOneShot (explosionSound);
		}

		piecesLit.Clear ();

		//if the option break in pieces is enabled, create the barrel broken
		if (breakInPieces) {
			GameObject brokenObject = brokenObjectPiecesParent;

			if (spawnBrokenObjectPiecesEnabled) {
				brokenObject = (GameObject)Instantiate (brokenObjectPiecesParent, transform.position, transform.rotation);

				if (!brokenObject.activeSelf) {
					brokenObject.SetActive (true);
				}
			} else {
				brokenObjectPiecesParent.SetActive (true);
			}

			if (removePiecesParent) {
				brokenObject.transform.SetParent (null);
			} else {
				brokenObject.transform.SetParent (transform);
			}

			Component[] components = brokenObject.GetComponentsInChildren (typeof(MeshRenderer));

			foreach (MeshRenderer child in components) {

				Rigidbody currentPartRigidbody = child.gameObject.GetComponent<Rigidbody> ();

				if (currentPartRigidbody == null) {
					currentPartRigidbody = child.gameObject.AddComponent<Rigidbody> ();
				}

				if (child.gameObject.GetComponent<Collider> () == null) {
					child.gameObject.AddComponent<BoxCollider> ();
				}

				currentPartRigidbody.AddExplosionForce (explosionForceToPieces, child.transform.position,
					explosionRadiusToPieces, 1, forceModeToPieces);

				if (fadePiecesEnabled) {
					//change the shader of the fragments to fade them
					int materialsLength = child.materials.Length;

					for (j = 0; j < materialsLength; j++) {
						Material temporalMaterial = child.materials [j];

						temporalMaterial.shader = transparentShader;

						rendererParts.Add (temporalMaterial);
					}

					piecesLit.Add (child.gameObject);
				}
			}
		}

		//kill the health component, to call the functions when the object health is 0
		if (!applyDamage.checkIfDead (gameObject)) {
			applyDamage.killCharacter (gameObject);
		}

		if (useEventOnObjectBroken) {
			eventOnObjectBroken.Invoke ();
		}

		broken = true;

		fadeInProcess = true;

		currentTimeToRemovePieces = timeToRemovePieces;

		if (fadePiecesEnabled) {
			stopUpdateCoroutine ();

			mainUpdateCoroutine = StartCoroutine (updateCoroutine ());
		}

		//if the object is being carried by the player, make him drop it
		GKC_Utils.checkDropObject (gameObject);
	}

	void stopUpdateCoroutine ()
	{
		if (mainUpdateCoroutine != null) {
			StopCoroutine (mainUpdateCoroutine);
		}
	}

	public void setCanBreakStatePaused (bool state)
	{
		canBreakStatePaused = state;
	}

	public void setObjectOwner (GameObject newObject)
	{
		objectOwner = newObject;
	}

	void OnCollisionEnter (Collision col)
	{
		if (canBreakWithCollisionsEnabled) {
			if (mainRigidbody != null) {
				if (mainRigidbody.velocity.magnitude > minVelocityToBreak && !canBreakStatePaused && !broken) {
					activateBreakObject ();
				}
			}
		}
	}

	public void waitToBreak ()
	{
		if (explosionDelay > 0) {
			StartCoroutine (waitToBreakCorutine ());
		} else {
			activateBreakObject ();
		}
	}

	IEnumerator waitToBreakCorutine ()
	{
		yield return new WaitForSeconds (explosionDelay);

		activateBreakObject ();
	}

	public void setExplosionValues (float force, float radius)
	{
		explosionForce = force;

		damageRadius = radius;
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
			Gizmos.color = Color.yellow;

			Gizmos.DrawWireSphere (transform.position, damageRadius);
		}
	}
}