using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class destroyableObject : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool explosionEnabled = true;
	public GameObject destroyedParticles;
	public AudioClip destroyedSound;

	[Space]
	[Header ("Explosion Settings")]
	[Space]

	public bool useExplosionForceWhenDestroyed;
	public float explosionRadius;
	public float explosionForce;
	public float explosionDamage;
	public bool ignoreShield;

	public int damageTypeID = -1;

	public bool killObjectsInRadius;
	public ForceMode forceMode;

	public bool applyExplosionForceToVehicles = true;
	public float explosionForceToVehiclesMultiplier = 0.2f;

	public bool pushCharactersOnExplosion = true;

	[Space]
	[Header ("Detect Objects Settings")]
	[Space]

	public bool userLayerMask;
	public LayerMask layer;

	[Space]
	[Header ("Remote Events Settings")]
	[Space]

	public bool useRemoteEventOnObjectsFound;
	public string removeEventName;

	[Space]
	[Header ("Object Pieces Settings")]
	[Space]

	public bool fadePiecesEnabled = true;
	public float timeToFadePieces;

	public bool storeRendererParts = true;
	public bool disableColliders = true;

	public bool destroyObjectAfterFacePieces = true;
	public Shader transparentShader;

	[Space]
	[Header ("Meshes Force Settings")]
	[Space]

	public float meshesExplosionForce = 500;
	public float meshesExplosionRadius = 50;
	public ForceMode meshesExplosionForceMode;

	[Space]
	[Header ("Extra Pieces Settings")]
	[Space]

	public bool useExtraParts;
	public List<GameObject> extraParts = new List<GameObject> ();

	[Space]
	[Header ("Events Settings")]
	[Space]

	public bool useEventOnObjectDestroyed;
	public UnityEvent eventOnObjectDestroyed;

	public bool useEventOnObjectBeforeDestroyed;
	public UnityEvent eventOnObjectBeforeDestroyed;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool destroyed;

	[Space]
	[Header ("Gizmo Settings")]
	[Space]

	public bool showGizmo;

	[Space]
	[Header ("Components")]
	[Space]

	public Rigidbody mainRigidbody;
	public mapObjectInformation mapInformationManager;
	public AudioSource destroyedSource;

	public Transform explosionCenter;


	List<Material> rendererParts = new List<Material> ();

	bool objectDisabled;


	void Start ()
	{
		if (mainRigidbody == null) {
			mainRigidbody = GetComponent<Rigidbody> ();
		}

		if (mapInformationManager == null) {
			mapInformationManager = GetComponent<mapObjectInformation> ();
		}

		if (destroyedSource == null) {
			destroyedSource = GetComponent<AudioSource> ();
		}
	}

	void Update ()
	{
		if (destroyed && !objectDisabled) {
			if (destroyObjectAfterFacePieces) {
				if (timeToFadePieces > 0) {
					timeToFadePieces -= Time.deltaTime;
				}

				if (storeRendererParts) {
					if (timeToFadePieces <= 0) {
						int piecesAmountFade = 0;

						for (int i = 0; i < rendererParts.Count; i++) {
							Color alpha = rendererParts [i].color;

							alpha.a -= Time.deltaTime / 5;

							rendererParts [i].color = alpha;

							if (alpha.a <= 0) {
								piecesAmountFade++;
							}
						}

						if (piecesAmountFade == rendererParts.Count) {
							Destroy (gameObject);
						}
					}
				} else {
					if (timeToFadePieces <= 0) {
						Destroy (gameObject);
					}
				}
			} else {
				objectDisabled = true;
			}
		}
	}

	//Destroy the object
	public void destroyObject ()
	{
		if (!explosionEnabled) {
			return;
		}

		if (destroyed) {
			return;
		}

		//instantiated an explosiotn particles
		if (destroyedParticles != null) {
			GameObject destroyedParticlesClone = (GameObject)Instantiate (destroyedParticles, transform.position, transform.rotation);
			destroyedParticlesClone.transform.SetParent (transform);
		}

		if (destroyedSource != null) {
			destroyedSource.PlayOneShot (destroyedSound);
		}

		//set the velocity of the object to zero
		if (mainRigidbody != null) {
			mainRigidbody.velocity = Vector3.zero;
			mainRigidbody.isKinematic = true;
		}

		//get every renderer component if the object
		if (storeRendererParts) {

			if (fadePiecesEnabled) {
				if (transparentShader == null) {
					transparentShader = Shader.Find ("Legacy Shaders/Transparent/Diffuse");
				}
			}

			Component[] components = GetComponentsInChildren (typeof(MeshRenderer));

			List<Component> componentsList = new List<Component> (components);

			if (useExtraParts) {
				foreach (GameObject currentObject in extraParts) {
					Component[] extraComponents = currentObject.GetComponentsInChildren (typeof(MeshRenderer));
				
					if (extraComponents.Length > 0) {
						componentsList.AddRange (extraComponents);
					}
				}
			}

			int ignoreRaycastLayerIndex = LayerMask.NameToLayer ("Ignore Raycast");

			int layerToIgnoreIndex = LayerMask.NameToLayer ("Scanner");

			foreach (MeshRenderer child in componentsList) {
				if (child != null && child.gameObject.layer != layerToIgnoreIndex) {
					if (child.enabled) {
						if (fadePiecesEnabled) {
							//for every renderer object, change every shader in it for a transparent shader 
							for (int j = 0; j < child.materials.Length; j++) {
								child.materials [j].shader = transparentShader;

								rendererParts.Add (child.materials [j]);
							}
						}

						//set the layer ignore raycast to them
						child.gameObject.layer = ignoreRaycastLayerIndex;

						//add rigidbody and box collider to them
						Rigidbody currentRigidbody = child.gameObject.GetComponent<Rigidbody> ();

						if (currentRigidbody == null) {
							currentRigidbody = child.gameObject.AddComponent<Rigidbody> ();
						} else {
							currentRigidbody.isKinematic = false;
							currentRigidbody.useGravity = true;
						}

						Collider currentCollider = child.gameObject.GetComponent<Collider> ();

						if (currentCollider == null) {
							currentCollider = child.gameObject.AddComponent<BoxCollider> ();
						}

						//apply explosion force
						currentRigidbody.AddExplosionForce (meshesExplosionForce, transform.position, meshesExplosionRadius, 3, meshesExplosionForceMode);
					}
				} 
			}
		}

		if (disableColliders) {
			//any other object with a collider but with out renderer, is disabled
			Component[] collidersInObject = GetComponentsInChildren (typeof(Collider));

			foreach (Collider currentCollider in collidersInObject) {
				if (currentCollider != null && !currentCollider.GetComponent<Renderer> ()) {
					currentCollider.enabled = false;
				}
			}
		}

		if (mapInformationManager != null) {
			mapInformationManager.removeMapObject ();
		}

		if (useEventOnObjectBeforeDestroyed) {
			eventOnObjectBeforeDestroyed.Invoke ();
		}

		if (useExplosionForceWhenDestroyed) {
			if (explosionCenter == null) {
				explosionCenter = transform;
			}

			Vector3 currentPosition = explosionCenter.position;

			applyDamage.setExplosion (currentPosition, explosionRadius, userLayerMask, layer, gameObject, false, gameObject, 
				killObjectsInRadius, true, false, explosionDamage, pushCharactersOnExplosion, applyExplosionForceToVehicles, 
				explosionForceToVehiclesMultiplier, explosionForce, forceMode, true, transform, ignoreShield, 
				useRemoteEventOnObjectsFound, removeEventName, damageTypeID);

		}

		if (useEventOnObjectDestroyed) {
			eventOnObjectDestroyed.Invoke ();
		}

		destroyed = true;
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
			if (useExplosionForceWhenDestroyed) {
				Gizmos.color = Color.red;
				Gizmos.DrawWireSphere (transform.position, explosionRadius);
			}
		}
	}
}
