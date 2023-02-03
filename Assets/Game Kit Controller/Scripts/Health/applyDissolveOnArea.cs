using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class applyDissolveOnArea : applyEffectOnArea
{
	[Header ("Custom Settings")]
	[Space]

	public Shader shaderToApply;
	public Texture dissolveTexture;
	public Color dissolveColor;
	public float dissolveColorAlpha;
	public float timeToDestroyObject;
	public float dissolveSpeed = 0.2f;

	public override void applyEffect (GameObject objectToAffect)
	{
		dissolveObject currentDissolveObject = objectToAffect.GetComponent<dissolveObject> ();

		if (currentDissolveObject) {
			removeDetectedObject (objectToAffect);

			return;
		}

		if (currentDissolveObject == null) {
			currentDissolveObject = objectToAffect.AddComponent<dissolveObject> ();
		}

		if (currentDissolveObject) {

			currentDissolveObject.shaderToApply = shaderToApply;
			currentDissolveObject.dissolveTexture = dissolveTexture;
			currentDissolveObject.dissolveColor = dissolveColor;
			currentDissolveObject.dissolveColorAlpha = dissolveColorAlpha;
			currentDissolveObject.timeToDestroyObject = timeToDestroyObject;
			currentDissolveObject.dissolveSpeed = dissolveSpeed;

			currentDissolveObject.activateDissolve (objectToAffect);

		}
	}
}