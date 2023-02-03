using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dynamicSplitScreenSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool dynamicSplitScreenActive = true;

	//The distance at which the splitscreen will be activated.
	public float splitDistance = 5;

	//The color and width of the splitter which splits the two screens up.
	public Color splitterColor;
	public float splitterWidth;

	public LayerMask secondPlayerCameraCullingMask;
	public LayerMask regularCameraCullingMask;

	[Space]
	[Header ("Other Settings")]
	[Space]

	public Vector3 extraPositionOffset1TopDown;
	public Vector3 extraPositionOffset2TopDown;
	//
	//	public Vector3 extraPositionOffset12_5dX;
	//	public Vector3 extraPositionOffset22_5dX;
	//
	//	public Vector3 extraPositionOffset12_5dZ;
	//	public Vector3 extraPositionOffset22_5dZ;
	public bool usingTopDownView;

	public float screenRotationSpeed = 20;

	[Space]
	[Header ("2.5D Settings")]
	[Space]

	public bool using2_5dView;
	public bool movingOnXAxis;

	public float minHorizontalAxisDistanceToOffset2_5d;
	public float horizontaAxisOffset2_5d;

	public float maxHorizontalDistanceForVerticalSplit = 20;

	public bool reverseMovementDirection;

	public Vector2 farScreenRotationClampValues;
	public Vector2 closeScreenRotationClampValues;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool maxDistanceReached;
	public bool player1OnLowerPosition;

	public float currentScreenRotation;

	public bool player1OnLeftSide;
	public float currentHorizontalDistance;

	public float currentDistance;

	public Vector3 currentExtraPositionOffset1;
	public Vector3 currentExtraPositionOffset2;

	public bool closeVerticalPosition;

	[Space]
	[Header ("Components")]
	[Space]

	public Transform player1;
	public Transform player2;

	//The two cameras, both of which are initalized/referenced in the start function.
	public playerCamera mainPlayerCameraManager1;
	public playerCamera mainPlayerCameraManager2;

	public Camera camera1;
	public Camera camera2;

	//The two quads used to draw the second screen, both of which are initalized in the start function.
	public GameObject split;
	public GameObject splitter;

	bool positionCompare;

	void Start ()
	{
		if (!dynamicSplitScreenActive) {
			return;
		}

		splitter.GetComponent<Renderer> ().sortingOrder = 2;

		if (camera1 != null) {
			camera1.depth = 1;
		} else {
			dynamicSplitScreenActive = false;
		}

		if (camera2 != null) {
			camera2.depth = 0;
		} else {
			dynamicSplitScreenActive = false;
		}
	}

	void FixedUpdate ()
	{
		if (!dynamicSplitScreenActive) {
			if (maxDistanceReached) {
				mainPlayerCameraManager1.setFollowingMultipleTargetsState (true, false);
				mainPlayerCameraManager2.setFollowingMultipleTargetsState (true, false);

				mainPlayerCameraManager1.setextraPositionOffsetValue (Vector3.zero);
				mainPlayerCameraManager2.setextraPositionOffsetValue (Vector3.zero);

				maxDistanceReached = false;
			}

			return;
		}

		//Gets the z axis distance between the two players and just the standard distance.
	
		currentDistance = GKC_Utils.distance (player1.position, player2.position);

		//Sets the angle of the player up, depending on who's leading on the x axis.
		currentScreenRotation = 0;

		if (usingTopDownView) {
			float zDistance = player1.position.z - player2.position.z;

			if (player1.position.x <= player2.position.x) {
				currentScreenRotation = Mathf.Rad2Deg * Mathf.Acos (zDistance / currentDistance);
			} else {
				currentScreenRotation = Mathf.Rad2Deg * Mathf.Asin (zDistance / currentDistance) - 90;
			}
		} 

		player1OnLeftSide = false;

		if (using2_5dView) {

			closeVerticalPosition = (player1.position - player2.position).y < 0.5f;

			player1OnLowerPosition = player1.position.y <= player2.position.y;

			if (movingOnXAxis) {
				if (reverseMovementDirection) {
					positionCompare = player1.position.x >= player2.position.x;
				} else {
					positionCompare = player1.position.x <= player2.position.x;
				}
			} else {
				if (reverseMovementDirection) {
					positionCompare = player1.position.z >= player2.position.z;
				} else {
					positionCompare = player1.position.z <= player2.position.z;
				}
			}

			if (movingOnXAxis) {
				if (positionCompare) {
					player1OnLeftSide = true;
				}
			} else {
				if (positionCompare) {
					player1OnLeftSide = true;
				}
			}

			currentHorizontalDistance = 0;
			if (movingOnXAxis) {
				currentHorizontalDistance =	player1.position.x - player2.position.x;
			} else {
				currentHorizontalDistance =	player1.position.z - player2.position.z;
			}

			if (player1OnLeftSide) {
				currentScreenRotation = Mathf.Rad2Deg * Mathf.Asin (currentHorizontalDistance / currentDistance);
			} else {
				currentScreenRotation = Mathf.Rad2Deg * Mathf.Acos (currentHorizontalDistance / currentDistance) + 90;
			}

			if (player1OnLeftSide) {
				if (currentDistance > maxHorizontalDistanceForVerticalSplit) {
					currentScreenRotation = Mathf.Clamp (currentScreenRotation, farScreenRotationClampValues.x, farScreenRotationClampValues.y);
				} else {
					currentScreenRotation = Mathf.Clamp (currentScreenRotation, closeScreenRotationClampValues.x, closeScreenRotationClampValues.y);
				}
			} else {
				if (currentDistance > maxHorizontalDistanceForVerticalSplit) {
					currentScreenRotation = Mathf.Clamp (currentScreenRotation, 180 + farScreenRotationClampValues.x, 180 + farScreenRotationClampValues.y);
				} else {
					currentScreenRotation = Mathf.Clamp (currentScreenRotation, 180 + closeScreenRotationClampValues.x, 180 + closeScreenRotationClampValues.y);
				}
			}
		}

		//Rotates the splitter according to the new angle.
		Quaternion targetRotation = Quaternion.Euler (new Vector3 (0, 0, currentScreenRotation));

		splitter.transform.localRotation = Quaternion.Lerp (splitter.transform.localRotation, targetRotation, Time.deltaTime * screenRotationSpeed);

		//Gets the exact midpoint between the two players.
		Vector3 midPoint = new Vector3 ((player1.position.x + player2.position.x) / 2, (player1.position.y + player2.position.y) / 2, (player1.position.z + player2.position.z) / 2); 

		//Waits for the two cameras to split and then calcuates a midpoint relevant to the difference in position between the two cameras.
		if (currentDistance > splitDistance) {
			Vector3 offset = midPoint - player1.position; 
			offset.x = Mathf.Clamp (offset.x, -splitDistance / 2, splitDistance / 2);
			offset.y = Mathf.Clamp (offset.y, -splitDistance / 2, splitDistance / 2);
			offset.z = Mathf.Clamp (offset.z, -splitDistance / 2, splitDistance / 2);
			midPoint = player1.position + offset;

			Vector3 offset2 = midPoint - player2.position; 
			offset2.x = Mathf.Clamp (offset.x, -splitDistance / 2, splitDistance / 2);
			offset2.y = Mathf.Clamp (offset.y, -splitDistance / 2, splitDistance / 2);
			offset2.z = Mathf.Clamp (offset.z, -splitDistance / 2, splitDistance / 2);
			Vector3 midPoint2 = player2.position - offset;

			//Sets the splitter and camera to active and sets the second camera position as to avoid lerping continuity errors.
			if (splitter.activeSelf == false) {
				splitter.SetActive (true);
				camera2.enabled = true;
			}

			if (!maxDistanceReached) {
				mainPlayerCameraManager1.setFollowingMultipleTargetsState (false, false);
				mainPlayerCameraManager2.setFollowingMultipleTargetsState (false, false);

				maxDistanceReached = true;
			}

			if (usingTopDownView) {
				currentExtraPositionOffset1 = extraPositionOffset1TopDown;
				currentExtraPositionOffset2 = extraPositionOffset2TopDown;
			} 

			if (using2_5dView) {
				if (movingOnXAxis) {

					currentExtraPositionOffset1.y = Mathf.Lerp (horizontaAxisOffset2_5d, 0, currentHorizontalDistance / minHorizontalAxisDistanceToOffset2_5d);
					currentExtraPositionOffset2.y = Mathf.Lerp (horizontaAxisOffset2_5d, 0, currentHorizontalDistance / minHorizontalAxisDistanceToOffset2_5d);

					if (closeVerticalPosition) {
						currentExtraPositionOffset1.y = Mathf.Lerp (currentExtraPositionOffset1.y, 0, Time.deltaTime * 3);
						currentExtraPositionOffset2.y = Mathf.Lerp (currentExtraPositionOffset2.y, 0, Time.deltaTime * 3);
					} else {
						if (player1OnLeftSide) {
							if (player1OnLowerPosition) {
								currentExtraPositionOffset2.y = -currentExtraPositionOffset2.y;
							} else {
								currentExtraPositionOffset1.y = -currentExtraPositionOffset1.y;
							}
						} else {
							if (player1OnLowerPosition) {
								currentExtraPositionOffset1.y = -currentExtraPositionOffset1.y;
							} else {
								currentExtraPositionOffset2.y = -currentExtraPositionOffset2.y;
							}
						}
					}

//					}else{
//						currentExtraPositionOffset1 = Mathf.Lerp (currentExtraPositionOffset1, multipleTargetsMaxFov, currentMultipleTargetsFov / multipleTargetsFovSpeed);
//						currentExtraPositionOffset2 = Mathf.Lerp (multipleTargetsMinFov, multipleTargetsMaxFov, currentMultipleTargetsFov / multipleTargetsFovSpeed);
//					}
//					if (playerOnLowerPosition) {
//						currentExtraPositionOffset1 = extraPositionOffset12_5dX;
//						currentExtraPositionOffset2 = -extraPositionOffset22_5dX;
//					} else {
//						currentExtraPositionOffset1 = -extraPositionOffset12_5dX;
//						currentExtraPositionOffset2 = extraPositionOffset22_5dX;
//					}
				} else {
//					if (playerOnLowerPosition) {
//						currentExtraPositionOffset1 = -extraPositionOffset12_5dZ;
//						currentExtraPositionOffset2 = extraPositionOffset22_5dZ;
//					} else {
//						currentExtraPositionOffset1 = extraPositionOffset12_5dZ;
//						currentExtraPositionOffset2 = -extraPositionOffset22_5dZ;
//					}
				}

				midPoint.y = player1.position.y;
				midPoint2.y = player2.position.y;
			}

			mainPlayerCameraManager1.setextraPositionOffsetValue (currentExtraPositionOffset1 + midPoint - mainPlayerCameraManager1.transform.position);
			mainPlayerCameraManager2.setextraPositionOffsetValue (currentExtraPositionOffset2 + midPoint2 - mainPlayerCameraManager2.transform.position);

		} else {
			//Deactivates the splitter and camera once the distance is less than the splitting distance (assuming it was at one point).
			if (splitter.activeSelf) {

				splitter.SetActive (false);
				camera2.enabled = false;
			}

			if (maxDistanceReached) {
				mainPlayerCameraManager1.setFollowingMultipleTargetsState (true, false);
				mainPlayerCameraManager2.setFollowingMultipleTargetsState (true, false);

				mainPlayerCameraManager1.setextraPositionOffsetValue (Vector3.zero);
				mainPlayerCameraManager2.setextraPositionOffsetValue (Vector3.zero);

				maxDistanceReached = false;
			}
		}
	}

	public void setDynamicSplitScreenActiveState (bool state, GameObject newPlayer1, GameObject newPlayer2, bool creatingCharactersOnEditor)
	{
		dynamicSplitScreenActive = state;

		enabled = state;

		if (newPlayer1 != null) {
			player1 = newPlayer1.transform;
		
			playerComponentsManager mainPlayerComponentsManager1 = player1.GetComponent<playerComponentsManager> ();

			if (mainPlayerComponentsManager1 != null) {
				mainPlayerCameraManager1 = mainPlayerComponentsManager1.getPlayerCamera ();

				camera1 = mainPlayerCameraManager1.getMainCamera ();
			}
		}

		if (newPlayer2 != null) {
			player2 = newPlayer2.transform;
			playerComponentsManager mainPlayerComponentsManager2 = player2.GetComponent<playerComponentsManager> ();

			if (mainPlayerComponentsManager2 != null) {
				mainPlayerCameraManager2 = mainPlayerComponentsManager2.getPlayerCamera ();

				camera2 = mainPlayerCameraManager2.getMainCamera ();

				if (state) {
					camera2.cullingMask = secondPlayerCameraCullingMask;
				} else {
					camera2.cullingMask = regularCameraCullingMask;
				} 
			}
		}

		print ("Setting dynamic split camera state as " + state);

		if (creatingCharactersOnEditor) {
			updateComponent ();
		}
	}

	public void setDynamicSplitScreenActiveState (bool state)
	{
		dynamicSplitScreenActive = state;

		enabled = state;
	}

	void updateComponent ()
	{
		GKC_Utils.updateComponent (this);
	}
}
