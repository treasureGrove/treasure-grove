using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class setObjectScaleSystem : MonoBehaviour
{
	public float changeScaleSpeed;

	public Transform objectToScale;

	public List<scaleStateInfo> scaleStateInfoList = new List<scaleStateInfo> ();

	Coroutine scaleCoroutine;

	scaleStateInfo currentScaleStateInfo;

	public void setScaleState (string scaleStateName)
	{
		for (int i = 0; i < scaleStateInfoList.Count; i++) {
			if (scaleStateInfoList [i].Name.Equals (scaleStateName)) {
				currentScaleStateInfo = scaleStateInfoList [i];
				changeScale (currentScaleStateInfo.objectScale);
			}
		}
	}

	public void changeScale (Vector3 newScale)
	{
		if (scaleCoroutine != null) {
			StopCoroutine (scaleCoroutine);
		}

		scaleCoroutine = StartCoroutine (changeScaleCoroutine (newScale));
	}

	IEnumerator changeScaleCoroutine (Vector3 newScale)
	{
		currentScaleStateInfo.eventOnStartChangeScale.Invoke ();

		float t = 0;

		bool targetReached = false;

		while (!targetReached) {
			t += Time.deltaTime * changeScaleSpeed;

			objectToScale.localScale = Vector3.Lerp (objectToScale.localScale, newScale, t);

			if (GKC_Utils.distance (objectToScale.localScale, newScale) < 0.01f) {
				targetReached = true;
			}

			yield return null;
		}

		currentScaleStateInfo.eventOnStopChangeScale.Invoke ();
	}

	[System.Serializable]
	public class scaleStateInfo
	{
		public string Name;

		public Vector3 objectScale;

		public UnityEvent eventOnStartChangeScale;
		public UnityEvent eventOnStopChangeScale;
	}
}
