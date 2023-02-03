using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class slowObjectsColor : MonoBehaviour
{
	public Color slowColor;
	public bool changeToSlow = true;
	public bool changeToNormal;
	public float slowDownDuration;
	public float timer = 0;

	public string setNormalVelocityString = "setNormalVelocity";
	public string setReducedVelocityString = "setReducedVelocity";
	public string setCanMoveAIStateString = "setCanMoveAIState";
	public string setOverrideAnimationSpeedActiveStateString = "setOverrideAnimationSpeedActiveState";

	public List<Renderer> rendererParts = new List<Renderer> ();
	public List<Color> originalColor = new List<Color> ();
	public List<Color> transistionColor = new List<Color> ();

	bool renderPartsStored;
	float slowValue;
	int i = 0;
	int j = 0;
	slowObject slowObjectManager;
	GameObject objectToCallFunction;

	int rendererPartsCount;
	int materialsLength;

	Renderer currentRenderer;
	Material currentMaterial;

	bool useSlowObjectColor;

	void Update ()
	{
		//change the color smoothly from the original, to the other
		timer += Time.deltaTime;

		if (useSlowObjectColor) {
			rendererPartsCount = rendererParts.Count;

			for (i = 0; i < rendererPartsCount; i++) {
				currentRenderer = rendererParts [i];

				if (currentRenderer != null) {
					materialsLength = currentRenderer.materials.Length;

					for (j = 0; j < materialsLength; j++) {
						currentMaterial = currentRenderer.materials [j];

						currentMaterial.color = Color.Lerp (currentMaterial.color, transistionColor [i], timer);
					}
				}
			}
		}

		//after the 80% of the time has passed, the color will change from the slowObjectsColor, to the original
		if (timer >= slowDownDuration * 0.8f && changeToSlow) {
			//set the transition color to the original
			changeToSlow = false;

			changeToNormal = true;

			transistionColor = originalColor;

			timer = 0;
		}

		//when the time is over, set the color and remove the script
		if (timer >= slowDownDuration * 0.2f && !changeToSlow && changeToNormal) {
			if (useSlowObjectColor) {
				rendererPartsCount = rendererParts.Count;

				for (i = 0; i < rendererPartsCount; i++) {

					currentRenderer = rendererParts [i];

					if (currentRenderer != null) {
						materialsLength = currentRenderer.materials.Length;

						for (j = 0; j < materialsLength; j++) {
							currentRenderer.materials [j].color = transistionColor [i];
						}
					}
				}
			}

			if (objectToCallFunction != null) {
				objectToCallFunction.BroadcastMessage (setNormalVelocityString, SendMessageOptions.DontRequireReceiver);

				if (slowValue < 0.09f) {
					objectToCallFunction.BroadcastMessage (setCanMoveAIStateString, true, SendMessageOptions.DontRequireReceiver);

					objectToCallFunction.BroadcastMessage (setOverrideAnimationSpeedActiveStateString, false, SendMessageOptions.DontRequireReceiver);
				}
			}

			slowObjectManager.checkEventOnRegularSpeed ();

			Destroy (gameObject.GetComponent<slowObjectsColor> ());
		}
	}

	public void startSlowObject (bool useSlowObjectColorValue, Color newSlowColor, float newSlowValue, float newSlowDownDuration, slowObject newSlowObjectComponent)
	{
		useSlowObjectColor = useSlowObjectColorValue;

		slowColor = newSlowColor;

		slowValue = newSlowValue;

		slowDownDuration = newSlowDownDuration;

		timer = 0;

		changeToSlow = true;
		changeToNormal = false;

		slowObjectManager = newSlowObjectComponent;

		objectToCallFunction = slowObjectManager.getObjectToCallFunction ();

		if (slowObjectManager.isUseCustomSlowSpeedEnabled ()) {
			slowValue = slowObjectManager.getCustomSlowSpeed ();
		}

		slowObjectManager.checkEventOnReducedSpeed ();

		//send a message to slow down the object
		if (objectToCallFunction != null) {
			objectToCallFunction.BroadcastMessage (setReducedVelocityString, slowValue, SendMessageOptions.DontRequireReceiver);

			if (slowValue < 0.09f) {
				objectToCallFunction.BroadcastMessage (setCanMoveAIStateString, false, SendMessageOptions.DontRequireReceiver);

				objectToCallFunction.BroadcastMessage (setOverrideAnimationSpeedActiveStateString, true, SendMessageOptions.DontRequireReceiver);

			}
		}
			
		if (!renderPartsStored) {
			if (useSlowObjectColor) {
				bool useCustomSlowSpeed = slowObjectManager.useMeshesToIgnoreEnabled ();

				int propertyNameID = Shader.PropertyToID ("_Color");

				//get all the renderers inside of it, to change their color with the slowObjectsColor from otherpowers
				Component[] components = GetComponentsInChildren (typeof(Renderer));
				foreach (Renderer child in components) {
					if (!useCustomSlowSpeed || !slowObjectManager.checkChildsObjectsToIgnore (child.transform)) {
						if (child.material.HasProperty (propertyNameID)) {

							materialsLength = child.materials.Length;

							for (j = 0; j < materialsLength; j++) {
								rendererParts.Add (child);

								originalColor.Add (child.materials [j].color);

								transistionColor.Add (slowColor);
							}
						}
					}
				}
			}

			renderPartsStored = true;
		}
	}
}