using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class slowObject : characterStateAffectedInfo
{
	[Header ("Main Settings")]
	[Space]

	public bool useCustomSlowSpeed;
	public float customSlowSpeed;

	public GameObject objectToCallFunction;

	public bool useSlowObjectColor = true;

	public Color customSlowObjectsColor;

	[Space]
	[Header ("Other Settings")]
	[Space]

	public bool useMeshesToIgnore;
	public List<Transform> meshesToIgnore = new List<Transform> ();

	[Space]
	[Header ("Event Settings")]
	[Space]

	public UnityEvent eventOnReducedSpeed;
	public UnityEvent eventOnRegularSpeed;

	List<Transform> objectsToIgnoreChildren = new List<Transform> ();

	void Start ()
	{
		if (objectToCallFunction == null) {
			objectToCallFunction = gameObject;
		}

		if (useMeshesToIgnore) {
			for (int i = 0; i < meshesToIgnore.Count; i++) {
				if (meshesToIgnore [i] != null) {
					Component[] childrens = meshesToIgnore [i].GetComponentsInChildren (typeof(Transform));

					foreach (Transform child in childrens) {
						objectsToIgnoreChildren.Add (child);
					}
				}
			}
		}
	}

	public GameObject getObjectToCallFunction ()
	{
		return objectToCallFunction;
	}

	public bool isUseCustomSlowSpeedEnabled ()
	{
		return useCustomSlowSpeed;
	}

	public float getCustomSlowSpeed ()
	{
		return customSlowSpeed;
	}

	public bool checkChildsObjectsToIgnore (Transform obj)
	{
		bool value = false;

		if (meshesToIgnore.Contains (obj) || objectsToIgnoreChildren.Contains (obj)) {
			value = true;
			return value;
		}

		return value;
	}

	public bool useMeshesToIgnoreEnabled ()
	{
		return useMeshesToIgnore;
	}

	public void checkEventOnReducedSpeed ()
	{
		eventOnReducedSpeed.Invoke ();
	}

	public void checkEventOnRegularSpeed ()
	{
		eventOnRegularSpeed.Invoke ();
	}

	public override void activateStateAffected (float stateDuration, float stateAmount)
	{
		slowObjectsColor currentSlowObjectsColor = objectToCallFunction.GetComponent<slowObjectsColor> ();

		if (currentSlowObjectsColor == null) {
			currentSlowObjectsColor = objectToCallFunction.AddComponent<slowObjectsColor> ();
		}

		if (currentSlowObjectsColor != null) {
			currentSlowObjectsColor.startSlowObject (useSlowObjectColor, customSlowObjectsColor, stateAmount, stateDuration, this);
		}
	}
}