using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class customNoiseSystem : MonoBehaviour
{
	[Header ("Noise State Info List Settings")]
	[Space]

	public List<noiseStateInfo> noiseStateInfoList = new List<noiseStateInfo> ();

	public bool useNoiseMesh = true;

	public string mainNoiseMeshManagerName = "Noise Mesh Manager";

	[Space]
	[Header ("Components")]
	[Space]

	public Transform noiseOriginTransform;

	noiseMeshSystem noiseMeshManager;
	noiseStateInfo currentNoiseState;

	bool noiseMeshManagerFound;

	void Start ()
	{
		if (noiseOriginTransform == null) {
			noiseOriginTransform = transform;
		}
	}

	public void setNoiseState (string stateName)
	{
		for (int i = 0; i < noiseStateInfoList.Count; i++) {
			if (noiseStateInfoList [i].Name.Equals (stateName) && noiseStateInfoList [i].stateEnabled) {
				currentNoiseState = noiseStateInfoList [i];
				activateNoise ();
			}
		}
	}

	public void activateNoise ()
	{
		if (currentNoiseState != null && currentNoiseState.stateEnabled && currentNoiseState.useNoise) {
			if (currentNoiseState.useNoiseDetection) {
				applyDamage.sendNoiseSignal (currentNoiseState.noiseRadius, noiseOriginTransform.position, 
					currentNoiseState.noiseDetectionLayer, currentNoiseState.noiseDecibels, 
					currentNoiseState.forceNoiseDetection, currentNoiseState.showNoiseDetectionGizmo, currentNoiseState.noiseID);
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
					noiseMeshManager.addNoiseMesh (currentNoiseState.noiseRadius, noiseOriginTransform.position + Vector3.up, currentNoiseState.noiseExpandSpeed);
				}
			}

			if (currentNoiseState.useNoiseEvent) {
				currentNoiseState.noiseEvent.Invoke ();
			}
		}
	}

	[System.Serializable]
	public class noiseStateInfo
	{
		public string Name;
		public bool stateEnabled = true;

		public bool useNoise;
		public float noiseRadius;
		public float noiseExpandSpeed;
		public bool useNoiseDetection;
		public LayerMask noiseDetectionLayer;
		public bool showNoiseDetectionGizmo;
		[Range (0, 2)] public float noiseDecibels = 1;

		public bool forceNoiseDetection;

		public int noiseID = -1;

		public bool useNoiseEvent;
		public UnityEvent noiseEvent;
	}
}
