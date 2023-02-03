using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class setTransparentSurfaces : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool checkSurfaceEnabled = true;

	public bool checkSurfaceActiveAtStart;

	public bool checkSurfaceActive;

	public bool lockedCameraActive;

	public LayerMask layer;

	public float capsuleCastRadius = 0.3f;

	public bool useCustomShader;
	public Shader customShader;

	public string mainManagerName = "Set Transparent Surfaces Manager";

	[Space]
	[Header ("Debug")]
	[Space]

	public bool surfaceFound;
	public bool surfaceFoundPreviously;
	public bool playerIsUsingDevices;
	public bool playerIsUsingDevicesPreviously;

	public List<GameObject> currentSurfaceGameObjectFoundList = new List<GameObject> ();

	[Space]
	[Header ("Events Settings")]
	[Space]

	public bool useEventsOnStartCheckSurface;
	public UnityEvent eventOnStartCheckSurface;
	public bool useEventsOnStopCheckSurface;
	public UnityEvent eventOnStopCheckSurface;

	public bool useEventOnSurfaceFound;
	public UnityEvent eventOnSurfaceFound;
	public bool useEventOnNoSurfaceFound;
	public UnityEvent eventOnNoSurfaceFound;

	[Space]
	[Header ("Gizmo Settings")]
	[Space]

	public bool showGizmo;
	public Color sphereColor = Color.green;
	public Color cubeColor = Color.blue;

	[Space]
	[Header ("Components")]
	[Space]

	public playerController playerControllerManager;
	public int playerID;
	public Transform rayOriginPositionFreeCamera;
	public Transform rayTargetPositionFreeCamera;
	public Transform rayOriginPositionLockedCamera;
	public Transform rayTargetPositionLockedCamera;

	public setTransparentSurfacesSystem setTransparentSurfacesManager;

	RaycastHit[] hits;
	float distanceToTarget;

	Vector3 rayDirection;
	List<GameObject> surfaceGameObjectList = new List<GameObject> ();
	Vector3 point1;
	Vector3 point2;

	GameObject currentSurfaceGameObjectFound;

	setTransparentSurfacesSystem.surfaceInfo currentSurfaceInfo;

	Transform currentRayOriginPosition;
	Transform currentRayTargetPosition;

	bool playerLocated;

	void Start ()
	{
		if (setTransparentSurfacesManager == null) {
			GKC_Utils.instantiateMainManagerOnSceneWithType (mainManagerName, typeof(setTransparentSurfacesSystem));

			setTransparentSurfacesManager = FindObjectOfType<setTransparentSurfacesSystem> ();
		}

		if (playerControllerManager != null) {
			playerID = playerControllerManager.getPlayerID ();

			playerLocated = true;
		}

		if (checkSurfaceActiveAtStart) {
			setCheckSurfaceActiveState (true);
		}
	}

	void Update ()
	{
		if (checkSurfaceActive) {
			if (lockedCameraActive) {
				currentRayOriginPosition = rayOriginPositionLockedCamera;
				currentRayTargetPosition = rayTargetPositionLockedCamera;
			} else {
				currentRayOriginPosition = rayOriginPositionFreeCamera;
				currentRayTargetPosition = rayTargetPositionFreeCamera;
			}

			distanceToTarget = GKC_Utils.distance (currentRayOriginPosition.position, currentRayTargetPosition.position);
			rayDirection = currentRayOriginPosition.position - currentRayTargetPosition.position;
			rayDirection = rayDirection / rayDirection.magnitude;

			Debug.DrawLine (currentRayTargetPosition.position, (rayDirection * distanceToTarget) + currentRayTargetPosition.position, Color.red, 2);

			point1 = currentRayOriginPosition.position - rayDirection * capsuleCastRadius;
			point2 = currentRayTargetPosition.position + rayDirection * capsuleCastRadius;

			hits = Physics.CapsuleCastAll (point1, point2, capsuleCastRadius, rayDirection, 0, layer);

			surfaceFound = hits.Length > 0;

			if (surfaceFound != surfaceFoundPreviously) {
				surfaceFoundPreviously = surfaceFound;

				if (surfaceFoundPreviously) {
					if (useEventOnSurfaceFound) {
						eventOnSurfaceFound.Invoke ();
					}
				} else {
					if (useEventOnNoSurfaceFound) {
						eventOnNoSurfaceFound.Invoke ();
					}
				}
			}

			if (playerLocated) {
				playerIsUsingDevices = playerControllerManager.isUsingDevice ();
			}

			if (playerIsUsingDevices != playerIsUsingDevicesPreviously) {
				playerIsUsingDevicesPreviously = playerIsUsingDevices;

				changeSurfacesToTransparentOrRegularTemporaly (!playerIsUsingDevices);
			}

			surfaceGameObjectList.Clear ();

			for (int i = 0; i < hits.Length; i++) {
				currentSurfaceGameObjectFound = hits [i].collider.gameObject;

				if (!setTransparentSurfacesManager.listContainsSurface (currentSurfaceGameObjectFound)) {
					if (useCustomShader) {
						setTransparentSurfacesManager.addNewSurface (currentSurfaceGameObjectFound, customShader);
					} else {
						setTransparentSurfacesManager.addNewSurface (currentSurfaceGameObjectFound, null);
					}
				} else {
					setTransparentSurfacesManager.checkSurfaceToSetTransparentAgain (currentSurfaceGameObjectFound);
				}

				if (!currentSurfaceGameObjectFoundList.Contains (currentSurfaceGameObjectFound)) {
					currentSurfaceGameObjectFoundList.Add (currentSurfaceGameObjectFound);
					setTransparentSurfacesManager.addPlayerIDToSurface (playerID, currentSurfaceGameObjectFound);
				}

				surfaceGameObjectList.Add (currentSurfaceGameObjectFound);
			}
		
			for (int i = 0; i < setTransparentSurfacesManager.surfaceInfoList.Count; i++) {
				currentSurfaceInfo = setTransparentSurfacesManager.surfaceInfoList [i];

				if (!surfaceGameObjectList.Contains (currentSurfaceInfo.surfaceGameObject)) {
					if (currentSurfaceInfo.playerIDs.Contains (playerID)) {

						setTransparentSurfacesManager.removePlayerIDToSurface (playerID, i);
					}

					if (currentSurfaceGameObjectFoundList.Contains (currentSurfaceInfo.surfaceGameObject)) {
						currentSurfaceGameObjectFoundList.Remove (currentSurfaceInfo.surfaceGameObject);
					}

					if (currentSurfaceInfo.numberOfPlayersFound < 1 && !currentSurfaceInfo.changingToOriginalActive) {
						setTransparentSurfacesManager.setSurfaceToRegular (i, true);
						i = 0;
					}
				}	 
			}
		}
	}

	public void checkSurfacesToRemove ()
	{
		for (int i = 0; i < setTransparentSurfacesManager.surfaceInfoList.Count; i++) {
			currentSurfaceInfo = setTransparentSurfacesManager.surfaceInfoList [i];

			if (currentSurfaceGameObjectFoundList.Contains (currentSurfaceInfo.surfaceGameObject)) {
			
				if (currentSurfaceInfo.playerIDs.Contains (playerID)) {

					setTransparentSurfacesManager.removePlayerIDToSurface (playerID, i);
				}
			}
		}
	}

	public void changeSurfacesToTransparentOrRegularTemporaly (bool state)
	{
		for (int i = 0; i < setTransparentSurfacesManager.surfaceInfoList.Count; i++) {
			currentSurfaceInfo = setTransparentSurfacesManager.surfaceInfoList [i];

			if (currentSurfaceGameObjectFoundList.Contains (currentSurfaceInfo.surfaceGameObject)) {

				if (currentSurfaceInfo.playerIDs.Contains (playerID)) {

					setTransparentSurfacesManager.changeSurfacesToTransparentOrRegularTemporaly (playerID, i, state);
				}
			}
		}
	}

	public void setCheckSurfaceActiveState (bool state)
	{
		if (!checkSurfaceEnabled) {
			return;
		}

		checkSurfaceActive = state;

		if (!checkSurfaceActive) {

			checkSurfacesToRemove ();

			setTransparentSurfacesManager.checkSurfacesToRemove ();
		}

		if (checkSurfaceActive) {
			if (useEventsOnStartCheckSurface) {
				eventOnStartCheckSurface.Invoke ();
			}
		} else {
			if (useEventsOnStopCheckSurface) {
				eventOnStopCheckSurface.Invoke ();
			}
		}
	}

	public void setLockedCameraActiveState (bool state)
	{
		lockedCameraActive = state;
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
		if (showGizmo && Application.isPlaying && checkSurfaceActive) {
		
			Gizmos.color = sphereColor;
		
			Gizmos.DrawSphere (point1, capsuleCastRadius);
			Gizmos.DrawSphere (point2, capsuleCastRadius);

			Gizmos.color = cubeColor;
			Vector3 scale = new Vector3 (capsuleCastRadius * 2, capsuleCastRadius * 2, distanceToTarget - capsuleCastRadius * 2);
			Matrix4x4 cubeTransform = Matrix4x4.TRS ((rayDirection * (distanceToTarget / 2)) + currentRayTargetPosition.position, Quaternion.LookRotation (rayDirection, point1 - point2), scale);
			Matrix4x4 oldGizmosMatrix = Gizmos.matrix;

			Gizmos.matrix *= cubeTransform;

			Gizmos.DrawCube (Vector3.zero, Vector3.one);

			Gizmos.matrix = oldGizmosMatrix;
		}
	}
}
