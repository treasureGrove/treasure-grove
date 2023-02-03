using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class dissolveProjectile : projectileSystem
{
	[Header ("Main Settings")]
	[Space]

	public LayerMask layer;

	public Shader shaderToApply;
	public Texture dissolveTexture;
	public Color dissolveColor;
	public float dissolveColorAlpha;

	public float timeToDestroyObject = 0.9f;

	public string dissolveShaderFieldName = "_Amount";
	public string dissolveShaderTextureFieldName = "_DissolveTexture";
	public string dissolveShaderColorFieldName = "_DissolveColor";
	public string dissolveShaderAlphaColorFieldName = "_DissolveColorAlpha";

	public float dissolveSpeed = 1;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool objectToDissolveFound;

	public float currentFadeValue = 1;

	dissolveObject currentDissolveObject;

	GameObject objectToDissolve;

	void Update ()
	{
		if (objectToDissolveFound) {
			currentFadeValue = currentDissolveObject.currentFadeValue;

			if (currentFadeValue >= 1 || currentFadeValue > timeToDestroyObject) {
				destroyObject ();
			} 
		}
	}

	public void dissolveObject (GameObject objectToDissolve)
	{
		checkObjectDetected (objectToDissolve.GetComponent<Collider> ());
	}

	//when the bullet touchs a surface, then
	public void checkObjectDetected (Collider col)
	{
		if (canActivateEffect (col)) {
			if (currentProjectileInfo.impactSoundEffect != null) {
				GetComponent<AudioSource> ().PlayOneShot (currentProjectileInfo.impactSoundEffect);
			}

			projectileUsed = true;

			//set the bullet kinematic
			objectToDamage = col.GetComponent<Collider> ().gameObject;

			mainRigidbody.isKinematic = true;

			if ((1 << col.gameObject.layer & layer.value) == 1 << col.gameObject.layer) {

				if (objectToDamage.GetComponent<Rigidbody> ()) {

					objectToDissolve = objectToDamage;

					objectToDissolveFound = true;
				} 

				bool isCharacter = applyDamage.isCharacter (objectToDamage);

				if (isCharacter) {
					objectToDissolve = applyDamage.getCharacter (objectToDamage);

					objectToDissolveFound = true;
				} else {
					bool isVehicle = applyDamage.isVehicle (objectToDamage);

					if (isVehicle) {
						objectToDissolve = applyDamage.getVehicle (objectToDamage);

						objectToDissolveFound = true;
					}
				}
					
				if (objectToDissolveFound) {
					currentDissolveObject = objectToDissolve.GetComponent<dissolveObject> ();

					if (currentDissolveObject != null) {
						destroyObject ();

						return;
					}

					if (currentDissolveObject == null) {
						currentDissolveObject = objectToDissolve.AddComponent<dissolveObject> ();
					}

					if (currentDissolveObject != null) {
						currentDissolveObject.shaderToApply = shaderToApply;
						currentDissolveObject.dissolveTexture = dissolveTexture;
						currentDissolveObject.dissolveColor = dissolveColor;
						currentDissolveObject.dissolveColorAlpha = dissolveColorAlpha;
						currentDissolveObject.timeToDestroyObject = timeToDestroyObject;
						currentDissolveObject.dissolveSpeed = dissolveSpeed;

						currentDissolveObject.activateDissolve (objectToDissolve);
					}
				}
			}

			checkProjectilesParent ();
		}
	}

	public void destroyObject ()
	{
		destroyProjectile ();
	}

	public override void resetProjectile ()
	{
		base.resetProjectile ();

		objectToDissolveFound = false;

		currentDissolveObject = null;

		objectToDissolve = null;
	}
}