using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sleepingStateIconSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public float lifeTime;
	public float increaseSizeSpeed;
	public int maxNumberOfIconsAtTime;
	public float spawnRate;
	public float sleepDuration;
	public float spawnPositionOffset;

	public Vector3 localOffset = new Vector3 (0, 0, 0.13f);

	[Space]
	[Header ("Icon Movement Settings")]
	[Space]

	public float movementSpeed;
	public float verticalSpeed;
	public float horizontalMovementAmount;
	public float horizontalMovementSpeed;

	[Space]
	[Header ("Color Settings")]
	[Space]

	public bool changeTextColorToDuration;
	public Color startColor = Color.white;
	public Color endColor = Color.red;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool sleepStateActive;
	public float transitionColor;

	[Space]
	[Header ("Components")]
	[Space]

	public GameObject sleepIcon;

	public Transform spawnPosition;

	public health healthManager;

	public Transform parentToAttach;

	List<sleepIconInfo> sleepIconList = new List<sleepIconInfo> ();

	float lastTimeSpawn;
	float posTargetX;
	Vector3 posTarget;
	Vector3 directionToCamera;
	Transform mainCameraTransform;

	Coroutine sleepCoroutine;

	void getComponents ()
	{
		if (mainCameraTransform == null) {
			mainCameraTransform = GKC_Utils.findMainPlayerCameraTransformOnScene ();

			if (parentToAttach != null) {
				transform.SetParent (parentToAttach);
				transform.localPosition = Vector3.zero + localOffset;
			}
		}
	}

	public void setSleepState (bool state)
	{
		sleepStateActive = state;

		if (sleepStateActive) {
			getComponents ();

			transitionColor = 0;

			sleepDuration = healthManager.getCurrentSedateDuration ();

			stopSleepCoroutine ();

			sleepCoroutine = StartCoroutine (activateSleepCorouine ());
		} else {
			stopSleepCoroutine ();

			for (int i = 0; i < sleepIconList.Count; i++) {
				Destroy (sleepIconList [i].iconTransform.gameObject);
			}

			sleepIconList.Clear ();
		}
	}

	public void stopSleepCoroutine ()
	{
		if (sleepCoroutine != null) {
			StopCoroutine (sleepCoroutine);
		}
	}

	IEnumerator activateSleepCorouine ()
	{
		var waitTime = new WaitForFixedUpdate ();

		while (true) {
			yield return waitTime;

			updateSleepState ();
		}
	}

	void updateSleepState ()
	{
		if (Time.time > lastTimeSpawn + spawnRate && sleepIconList.Count < maxNumberOfIconsAtTime) {
			GameObject newIcon = Instantiate (sleepIcon);
			newIcon.transform.position = spawnPosition.position + Vector3.up * spawnPositionOffset;
			lastTimeSpawn = Time.time;
			newIcon.transform.SetParent (spawnPosition);

			sleepIconInfo newSleepIconInfo = new sleepIconInfo ();
			newSleepIconInfo.iconTransform = newIcon.transform;
			newSleepIconInfo.spawnTime = Time.time;
			newSleepIconInfo.meshText = newIcon.GetComponentInChildren<TextMesh> ();
			newSleepIconInfo.meshText.color = startColor; 

			sleepIconList.Add (newSleepIconInfo);
		}

		for (int i = 0; i < sleepIconList.Count; i++) {
			sleepIconInfo curentIcon = sleepIconList [i];

			posTargetX = Mathf.Sin (Time.time * horizontalMovementSpeed) * horizontalMovementAmount;
			posTarget = curentIcon.iconTransform.right * posTargetX;
			posTarget += curentIcon.iconTransform.position;
			posTarget += Vector3.up * verticalSpeed;

			curentIcon.iconTransform.position = Vector3.Lerp (curentIcon.iconTransform.position, posTarget, Time.deltaTime * movementSpeed);

			if (Time.time > curentIcon.spawnTime + lifeTime) {
				Destroy (curentIcon.iconTransform.gameObject);

				sleepIconList.RemoveAt (i);

				i = 0;
			}

			curentIcon.iconTransform.localScale += Vector3.one * (increaseSizeSpeed * Time.deltaTime);

			directionToCamera = curentIcon.iconTransform.position - mainCameraTransform.position;
			curentIcon.iconTransform.rotation = Quaternion.LookRotation (directionToCamera);

			if (changeTextColorToDuration) {
				if (sleepDuration > 0) {
					curentIcon.meshText.color = Color.Lerp (startColor, endColor, transitionColor);
				}
			}
		}

		if (changeTextColorToDuration) {
			if (sleepDuration > 0) {
				transitionColor += Time.deltaTime / sleepDuration;
			}
		}
	}

	[System.Serializable]
	public class sleepIconInfo
	{
		public Transform iconTransform;
		public float spawnTime;
		public TextMesh meshText;
	}
}
