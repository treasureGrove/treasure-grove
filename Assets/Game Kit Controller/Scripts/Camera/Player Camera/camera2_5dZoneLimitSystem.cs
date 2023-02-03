using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camera2_5dZoneLimitSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool useCameraLimit = true;

	public bool setCameraForwardPosition;
	public float cameraForwardPosition;

	public bool useMoveInXAxisOn2_5d;

	[Space]
	[Header ("Limit Width Settings")]
	[Space]

	public bool useWidthLimit = true;
	public float widthLimitRight;
	public float widthLimitLeft;

	[Space]
	[Header ("Limit Height Settings")]
	[Space]

	public bool useHeightLimit = true;
	public float heightLimitUpper;
	public float heightLimitLower;

	[Space]
	[Header ("Limit Depth Settings")]
	[Space]

	public bool useDepthLimit;
	public float depthLimitFront;
	public float depthLimitBackward;

	[Space]
	[Header ("Set Limit At Start Settings")]
	[Space]

	public bool startGameWithThisLimit;
	public GameObject playerForView;
	public bool useMultiplePlayers;
	public List<GameObject> playerForViewList = new List<GameObject> ();

	[Space]
	[Header ("Gizmo Settings")]
	[Space]

	public bool showGizmo;
	public Color gizmoColor = Color.white;
	public float gizmoRadius = 0.2f;
	public Color depthGizmoColor = Color.white;

	Vector3 currentPosition;

	GameObject currentPlayer;

	Vector3 horizontalRightUpperLinePosition;
	Vector3 horizontalLeftUpperLinePosition;
	Vector3 horizontalRightLowerLinePosition;
	Vector3 horizontalLeftLowerLinePosition;
	Vector3 verticalRightUpperLinePosition;
	Vector3 verticalRightLowerLinePosition;
	Vector3 verticalLeftUpperLinePosition;
	Vector3 verticalLefttLowerLinePosition;

	Vector3 depthFrontPosition;
	Vector3 depthBackwardPosition;

	float totalWidth;
	float totalheight;

	playerController playerControlerManager;
	playerCamera playerCameraManager;

	void Start ()
	{
		if (startGameWithThisLimit) {
			if (useMultiplePlayers) {
				for (int k = 0; k < playerForViewList.Count; k++) {
					setCurrentPlayer (playerForViewList [k]);

					setCameraLimitOnPlayer ();
				}
			} else {
				setCurrentPlayer (playerForView);

				setCameraLimitOnPlayer ();
			}
		}
	}

	public void setCameraLimitOnPlayer ()
	{
		if (currentPlayer != null) {
			playerCameraManager.setCameraLimit (useCameraLimit, useWidthLimit, widthLimitRight, widthLimitLeft, useHeightLimit, heightLimitUpper, 
				heightLimitLower, transform.position, useDepthLimit, depthLimitFront, depthLimitBackward);

			if (setCameraForwardPosition) {
				playerCameraManager.setNewCameraForwardPosition (cameraForwardPosition);
			}
		}
	}

	public void setCameraLimitOnPlayer (GameObject newPlayer)
	{
		setCurrentPlayer (newPlayer);

		setCameraLimitOnPlayer ();
	}

	public void setCurrentPlayer (GameObject newPlayer)
	{
		if (newPlayer == null) {
			print ("WARNING: the zone limit system is trying to assign an empty player, make sure the system is properly configured");

			return;
		}

		currentPlayer = newPlayer;

		playerComponentsManager mainPlayerComponentsManager = currentPlayer.GetComponent<playerComponentsManager> ();

		playerControlerManager = mainPlayerComponentsManager.getPlayerController ();

		if (playerControlerManager != null) {
			playerCameraManager = mainPlayerComponentsManager.getPlayerCamera ();
		} else {
			vehicleHUDManager currentvehicleHUDManager = applyDamage.getVehicleHUDManager (newPlayer);
			if (currentvehicleHUDManager && currentvehicleHUDManager.isVehicleBeingDriven ()) {
				
				currentPlayer = currentvehicleHUDManager.getCurrentDriver ();

				mainPlayerComponentsManager = currentPlayer.GetComponent<playerComponentsManager> ();

				playerControlerManager = mainPlayerComponentsManager.getPlayerController ();
				playerCameraManager = mainPlayerComponentsManager.getPlayerCamera ();
			}
		}
	}

	public void setConfigurationToPlayer ()
	{
		setCameraLimitOnPlayer ();
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

			currentPosition = transform.position;

			horizontalRightUpperLinePosition = currentPosition + Vector3.up * heightLimitUpper;
			horizontalLeftUpperLinePosition = currentPosition + Vector3.up * heightLimitUpper;
			horizontalRightLowerLinePosition = currentPosition - Vector3.up * heightLimitLower;
			horizontalLeftLowerLinePosition = currentPosition - Vector3.up * heightLimitLower;
			verticalRightUpperLinePosition = currentPosition + Vector3.up * heightLimitUpper;
			verticalRightLowerLinePosition = currentPosition - Vector3.up * heightLimitLower;
			verticalLeftUpperLinePosition = currentPosition + Vector3.up * heightLimitUpper;
			verticalLefttLowerLinePosition = currentPosition - Vector3.up * heightLimitLower;

			if (useMoveInXAxisOn2_5d) {
				horizontalRightUpperLinePosition += Vector3.right * widthLimitRight;
				horizontalLeftUpperLinePosition -= Vector3.right * widthLimitLeft;
				horizontalRightLowerLinePosition += Vector3.right * widthLimitRight;
				horizontalLeftLowerLinePosition -= Vector3.right * widthLimitLeft;
				verticalRightUpperLinePosition += Vector3.right * widthLimitRight;
				verticalRightLowerLinePosition += Vector3.right * widthLimitRight;
				verticalLeftUpperLinePosition -= Vector3.right * widthLimitLeft;
				verticalLefttLowerLinePosition -= Vector3.right * widthLimitLeft;
			} else {
				horizontalRightUpperLinePosition += Vector3.forward * widthLimitRight;
				horizontalLeftUpperLinePosition -= Vector3.forward * widthLimitLeft;
				horizontalRightLowerLinePosition += Vector3.forward * widthLimitRight;
				horizontalLeftLowerLinePosition -= Vector3.forward * widthLimitLeft;
				verticalRightUpperLinePosition += Vector3.forward * widthLimitRight;
				verticalRightLowerLinePosition += Vector3.forward * widthLimitRight;
				verticalLeftUpperLinePosition -= Vector3.forward * widthLimitLeft;
				verticalLefttLowerLinePosition -= Vector3.forward * widthLimitLeft;
			}

			Gizmos.DrawCube (transform.position, Vector3.one * gizmoRadius);

			if (useWidthLimit) {
				Gizmos.DrawLine (horizontalRightUpperLinePosition, horizontalLeftUpperLinePosition);
				Gizmos.DrawLine (horizontalRightLowerLinePosition, horizontalLeftLowerLinePosition);
			}

			if (useHeightLimit) {
				Gizmos.DrawLine (verticalRightUpperLinePosition, verticalRightLowerLinePosition);
				Gizmos.DrawLine (verticalLeftUpperLinePosition, verticalLefttLowerLinePosition);
			}
			if (useDepthLimit) {
				Gizmos.color = depthGizmoColor;

				totalWidth = widthLimitRight + widthLimitLeft;
				totalheight = heightLimitUpper + heightLimitLower;

				if (useMoveInXAxisOn2_5d) {
					depthFrontPosition = transform.position + Vector3.forward * depthLimitFront / 2 + Vector3.right * widthLimitRight / 2 - Vector3.right * widthLimitLeft / 2;
					depthFrontPosition += Vector3.up * heightLimitUpper / 2 - Vector3.up * heightLimitLower / 2;

					Gizmos.DrawCube (depthFrontPosition, new Vector3 (totalWidth, totalheight, depthLimitFront));

					depthBackwardPosition = transform.position - Vector3.forward * depthLimitBackward / 2 + Vector3.right * widthLimitRight / 2 - Vector3.right * widthLimitLeft / 2;
					depthBackwardPosition += Vector3.up * heightLimitUpper / 2 - Vector3.up * heightLimitLower / 2;

					Gizmos.DrawCube (depthBackwardPosition, new Vector3 (totalWidth, totalheight, depthLimitBackward));
				} else {

					depthFrontPosition = transform.position + Vector3.right * depthLimitFront / 2 + Vector3.forward * widthLimitRight / 2 - Vector3.forward * widthLimitLeft / 2;
					depthFrontPosition += Vector3.up * heightLimitUpper / 2 - Vector3.up * heightLimitLower / 2;

					Gizmos.DrawCube (depthFrontPosition, new Vector3 (depthLimitFront, totalheight, totalWidth));

					depthBackwardPosition = transform.position - Vector3.right * depthLimitBackward / 2 + Vector3.forward * widthLimitRight / 2 - Vector3.forward * widthLimitLeft / 2;
					depthBackwardPosition += Vector3.up * heightLimitUpper / 2 - Vector3.up * heightLimitLower / 2;

					Gizmos.DrawCube (depthBackwardPosition, new Vector3 (depthLimitBackward, totalheight, totalWidth));
				}
			}
		}
	}
}
