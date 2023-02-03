using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class pointAndClickElement : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool elementEnabled = true;

	public typeOfObject elementType;

	public bool useOnOffDeviceState;

	public bool isPickup;

	public bool activePanelAfterStopUse;
	public bool disablePanelAfterUse;

	public bool followObjectToUsePosition;

	public bool useFrontRearPositionForNavMesh;

	public bool useCustomElementMinDistance;
	public float customElementMinDistance;

	public bool useCustomElementName;
	public string customElementName;

	public LayerMask layerForElements;

	[Space]
	[Header ("Element Text Info Settings")]
	[Space]

	public bool useElementTextInfo;
	[TextArea (1, 10)] public string elementTextInfo;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool usingElement;
	public bool hovervingPointAndClickElement;

	[Space]
	[Header ("Events Settings")]
	[Space]

	public UnityEvent activateElementEvent;
	public UnityEvent deactivateElementEvent;

	[Space]
	[Header ("Events On Hover Settings")]
	[Space]

	public bool useEventsOnHover;
	public UnityEvent eventOnHoverOn;
	public UnityEvent eventOnHoverOff;

	[Space]
	[Header ("Gizmo Settings")]
	[Space]

	public bool showGizmo;
	public Color gizmoLabelColor = Color.green;
	public Color gizmoColor = Color.white;
	public float gizmoRadius = 0.3f;

	[Space]
	[Header ("Components")]
	[Space]

	public GameObject objectToUse;

	public Transform positionForNavMesh;
	public Transform frontPositionForNavMesh;
	public Transform rearPositionForNavMesh;

	public Collider mainCollider;

	public enum typeOfObject
	{
		device,
		vehicle,
		friend,
		enemy
	}

	Transform objectToFollow;

	List<playerNavMeshSystem> playerNavMeshSystemList = new List<playerNavMeshSystem> ();

	bool elementInstantiated;


	void Start ()
	{
		if (followObjectToUsePosition) {
			if (objectToUse != null) {
				objectToFollow = objectToUse.transform;
			}
		}
			
		if (mainCollider == null) {
			mainCollider = GetComponent<Collider> ();
		}

		elementInstantiated = true;
	}

	void Update ()
	{
		if (followObjectToUsePosition) {
			transform.position = objectToFollow.position;
		}
	}

	public void setElementEnabledState (bool state)
	{
		if (enabled && elementInstantiated) {
			elementEnabled = state;

			mainCollider.enabled = elementEnabled;

			if (playerNavMeshSystemList.Count > 0) {
				for (int i = 0; i < playerNavMeshSystemList.Count; i++) {
					if (playerNavMeshSystemList [i] != null) {
						playerNavMeshSystemList [i].disablePanelInfo ();
					}
				}
			}
		}
	}

	public string getPointAndClickElementTextInfo ()
	{
		return elementTextInfo;
	}

	public void usePointAndClickElement ()
	{
		usingElement = !usingElement;

		if (elementType == typeOfObject.device) {

		} else if (elementType == typeOfObject.vehicle) {

		} else if (elementType == typeOfObject.friend) {

		} else if (elementType == typeOfObject.enemy) {

		}

		if (usingElement) {
			if (activateElementEvent.GetPersistentEventCount () > 0) {
				activateElementEvent.Invoke ();
			}
		} else {
			if (deactivateElementEvent.GetPersistentEventCount () > 0) {
				deactivateElementEvent.Invoke ();
			}
		}
	}

	public bool isDevice ()
	{
		return elementType == typeOfObject.device;
	}

	public bool isFriend ()
	{
		return elementType == typeOfObject.friend;
	}

	public bool isEnemy ()
	{
		return elementType == typeOfObject.enemy;
	}

	public GameObject getElementToUse ()
	{
		return objectToUse;
	}

	public Transform getPositionForNavMesh (Vector3 playerPosition)
	{
		if (useFrontRearPositionForNavMesh) {
			float dot = Vector3.Dot (transform.forward, (playerPosition - transform.position).normalized);

			if (dot > 0) {
				return frontPositionForNavMesh;
			} else {
				return rearPositionForNavMesh;
			}
		} else {
			return positionForNavMesh;
		}
	}

	public void removeElement ()
	{
		Destroy (gameObject);
	}

	public bool checkIfRemove ()
	{
		if (isPickup) {
			return true;
		}
		return false;
	}

	public void setHoveringPointAndClickElementState (bool state)
	{
		hovervingPointAndClickElement = state;

		if (useEventsOnHover) {
			if (hovervingPointAndClickElement) {
				eventOnHoverOn.Invoke ();
			} else {
				eventOnHoverOff.Invoke ();
			}
		}
	}

	void OnTriggerEnter (Collider col)
	{
		checkTriggerInfo (col, true);
	}

	void OnTriggerExit (Collider col)
	{
		checkTriggerInfo (col, false);
	}

	public void checkTriggerInfo (Collider col, bool isEnter)
	{
		if ((1 << col.gameObject.layer & layerForElements.value) == 1 << col.gameObject.layer) {
			if (isEnter) {

				playerComponentsManager currentPlayerComponentsManager = col.GetComponent<playerComponentsManager> ();

				if (currentPlayerComponentsManager != null) {
					playerNavMeshSystem currentPlayerNavmeshSystem = currentPlayerComponentsManager.getPlayerNavMeshSystem ();

					if (currentPlayerNavmeshSystem != null) {
						if (!playerNavMeshSystemList.Contains (currentPlayerNavmeshSystem)) {
							playerNavMeshSystemList.Add (currentPlayerNavmeshSystem);

							currentPlayerNavmeshSystem.setPointAndClickDetectedState (true);
						}
					}
				}
			} else {
				playerComponentsManager currentPlayerComponentsManager = col.GetComponent<playerComponentsManager> ();

				if (currentPlayerComponentsManager != null) {
					playerNavMeshSystem currentPlayerNavmeshSystem = currentPlayerComponentsManager.getPlayerNavMeshSystem ();

					if (currentPlayerNavmeshSystem != null) {
						if (playerNavMeshSystemList.Contains (currentPlayerNavmeshSystem)) {
							playerNavMeshSystemList.Remove (currentPlayerNavmeshSystem);

							currentPlayerNavmeshSystem.setPointAndClickDetectedState (false);
						}
					}
				}
			}
		}
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

	//draw the pivot and the final positions of every door
	void DrawGizmos ()
	{
		if (showGizmo) {
			Gizmos.color = gizmoColor;
			Gizmos.DrawSphere (transform.position, gizmoRadius);

			if (useFrontRearPositionForNavMesh) {
				if (frontPositionForNavMesh != null) {
					Gizmos.DrawLine (transform.position, frontPositionForNavMesh.position);
					Gizmos.DrawSphere (frontPositionForNavMesh.position, gizmoRadius);
				}

				if (rearPositionForNavMesh != null) {
					Gizmos.DrawLine (transform.position, rearPositionForNavMesh.position);
					Gizmos.DrawSphere (rearPositionForNavMesh.position, gizmoRadius);
				}
			} else {
				if (positionForNavMesh != null) {
					Gizmos.DrawLine (transform.position, positionForNavMesh.position);
					Gizmos.DrawSphere (positionForNavMesh.position, gizmoRadius);
				}
			}
		}
	}
}
