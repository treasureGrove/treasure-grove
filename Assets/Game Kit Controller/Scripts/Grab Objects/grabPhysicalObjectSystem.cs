using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class grabPhysicalObjectSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool grabObjectPhysically = true;

	public LayerMask layerForUsers;

	public Vector3 colliderScale;
	public Vector3 colliderOffset;

	public bool setRigidbodyMassValue;
	public float rigidbodyMassValue = 1;

	public string tagForObjectWhenActive;
	public string tagForObjectWhenInactive;

	public bool carryObjectOutOfPlayerBody;

	public bool disableObjectColliderWhileGrabbed;

	[Space]
	[Header ("Grab Object Animation Settings")]
	[Space]

	public bool changeAnimationSpeed;
	[Range (0, 1)] public float animationSpeed = 1;
	public bool setNewMovementTreeID;
	public int newMovementTreeID;
	public bool applyAnimatorVelocityWithoutMoving;

	public bool disableJumpOnGrabbedObject;
	public bool disableRunOnGrabbedObject;

	[Space]
	[Header ("Grab Object Settings")]
	[Space]

	public bool checkViewToGrabObject;
	public bool canBeGrabbedOnFirstPerson = true;
	public bool canBeGrabbedOnThirdPerson = true;

	[Space]
	[Header ("Transform Reference Settings")]
	[Space]

	public Transform referencePosition;

	public bool useReferencePositionForEveryView;
	public Transform referencePositionThirdPerson;
	public Transform referencePositionFirstPerson;

	public Transform referencePositionToKeepObject;

	[Space]
	[Header ("Mount Point Settings")]
	[Space]

	public bool useBoneToKeepObject;
	public HumanBodyBones boneToKeepObject;

	public bool useMountPointToKeepObject;
	public string mountPointTokeepObjectName;

	[Space]
	[Header ("IK Settings")]
	[Space]

	public bool IKSystemEnabled = true;

	public bool useRightHand = true;
	public bool useLeftHand = true;
	public Transform rightHandPosition;
	public Transform lefHandPosition;

	public bool useRightElbow;
	public bool useLeftElbow;
	public Transform rightElbowPosition;
	public Transform lefElbowPosition;

	[Space]
	[Header ("Hands Settings")]
	[Space]

	public bool useAnimationGrabbingHand;
	public int rightGrabbingHandID;
	public int leftGrabbingHandID;

	[Space]
	[Header ("Grab Object Parent Settings")]
	[Space]

	public bool useRightHandForObjectParent = true;

	[Space]
	[Header ("Kinematic List Settings")]
	[Space]

	public bool setExtraListKinematic;
	public List<Rigidbody> extraListKinematic = new List<Rigidbody> ();

	[Space]
	[Header ("Melee Weapons Settings")]
	[Space]

	public bool useMeleeAttackSystem;
	public grabPhysicalObjectMeleeAttackSystem mainGrabPhysicalObjectMeleeAttackSystem;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool objectGrabed;

	public bool objectIsCharacter;
	public GameObject character;
	public Transform characterBody;

	public bool keepGrabbedObjectState;

	public List<grabObjects> grabObjectsList = new List<grabObjects> ();

	public Transform currentObjectParent;

	[Space]
	[Header ("Events Settings")]
	[Space]

	public bool useEventsOnGrabDrop;
	public UnityEvent eventOnGrab;
	public UnityEvent eventOnDrop;

	[Space]
	[Header ("Remote Event Settings")]
	[Space]

	public bool useRemoteEventOnObjectsFound;
	public List<string> remoteEventNameListOnStart = new List<string> ();
	public List<string> remoteEventNameListOnEnd = new List<string> ();

	[Space]
	[Header ("Grab Object Type Settings")]
	[Space]

	public bool objectMeshInsideMainParent;
	public Transform objectMeshMainParent;
	public bool useMeshCollider;
	public bool useBoxCollider;

	[Space]
	[Header ("Gizmo Settings")]
	[Space]

	public bool showGizmo;
	public Color gizmoColor = Color.red;

	[Space]
	[Header ("Components")]
	[Space]

	public GameObject objectToGrabParent;

	public Collider grabObjectTrigger;

	public Collider mainCollider;

	grabObjects grabObjectsManager;

	GameObject currentUser;
	grabObjects currentGrabObjectDetected;

	List<Rigidbody> rigidbodyList = new List<Rigidbody> ();
	List<float> rigidbodyMassList = new List<float> ();

	List<int> layerMaskList = new List<int> ();

	Vector3 lastPositionBeforeGrabbed;
	Quaternion lastRotationBeforeGrabbed;

	Vector3 lastPositionBeforeDropped;
	Quaternion lastRotationBeforeDropped;

	void Start ()
	{
		assignObjectToGrabParentIfNull ();
	}

	public void assignObjectToGrabParentIfNull ()
	{
		if (objectToGrabParent == null) {
			objectToGrabParent = gameObject;
		}
	}

	public void setRigidbodyList ()
	{
		if (rigidbodyList.Count == 0) {

			assignObjectToGrabParentIfNull ();

			Component[] components = objectToGrabParent.GetComponentsInChildren (typeof(Rigidbody));

			foreach (Rigidbody child in components) {
				rigidbodyMassList.Add (child.mass);
				rigidbodyList.Add (child);
				layerMaskList.Add (child.gameObject.layer);
			}
		}
	}

	public void grabObject (GameObject currentPlayer)
	{
		if (objectGrabed) {
			return;
		}

		//print ("grab object");
		objectGrabed = true;

		grabObjectsManager = currentPlayer.GetComponent<grabObjects> ();

		setGrabObjectTriggerState (false);

		assignObjectToGrabParentIfNull ();

		lastPositionBeforeGrabbed = objectToGrabParent.transform.position;
		lastRotationBeforeGrabbed = objectToGrabParent.transform.rotation;


		grabPhysicalObjectInfo newGrabPhysicalObjectInfo = new grabPhysicalObjectInfo ();

		newGrabPhysicalObjectInfo.objectToGrab = objectToGrabParent;
		newGrabPhysicalObjectInfo.IKSystemEnabled = IKSystemEnabled;

		newGrabPhysicalObjectInfo.changeAnimationSpeed = changeAnimationSpeed;
		newGrabPhysicalObjectInfo.animationSpeed = animationSpeed;

		newGrabPhysicalObjectInfo.setNewMovementTreeID = setNewMovementTreeID;
		newGrabPhysicalObjectInfo.newMovementTreeID = newMovementTreeID;

		newGrabPhysicalObjectInfo.applyAnimatorVelocityWithoutMoving = applyAnimatorVelocityWithoutMoving;

		newGrabPhysicalObjectInfo.disableJumpOnGrabbedObject = disableJumpOnGrabbedObject;
		newGrabPhysicalObjectInfo.disableRunOnGrabbedObject = disableRunOnGrabbedObject;
	
		newGrabPhysicalObjectInfo.rightHandPosition = rightHandPosition;
		newGrabPhysicalObjectInfo.lefHandPosition = lefHandPosition;
		newGrabPhysicalObjectInfo.useRightHand = useRightHand;
		newGrabPhysicalObjectInfo.useLeftHand = useLeftHand;
		newGrabPhysicalObjectInfo.useRightElbow = useRightElbow;
		newGrabPhysicalObjectInfo.useLeftElbow = useLeftElbow;
		newGrabPhysicalObjectInfo.rightElbowPosition = rightElbowPosition;
		newGrabPhysicalObjectInfo.lefElbowPosition = lefElbowPosition;

		newGrabPhysicalObjectInfo.useAnimationGrabbingHand = useAnimationGrabbingHand;
		newGrabPhysicalObjectInfo.rightGrabbingHandID = rightGrabbingHandID;
		newGrabPhysicalObjectInfo.leftGrabbingHandID = leftGrabbingHandID;

		newGrabPhysicalObjectInfo.useRightHandForObjectParent = useRightHandForObjectParent;

		grabObjectsManager.grabPhysicalObject (newGrabPhysicalObjectInfo);

		for (int i = 0; i < grabObjectsList.Count; i++) {
			if (grabObjectsList [i] != grabObjectsManager) {
				grabObjectsList [i].removeCurrentPhysicalObjectToGrabFound (objectToGrabParent);
			}
		}

		if (setRigidbodyMassValue) {

			setRigidbodyList ();

			int ignoreRaycastLayerIndex = LayerMask.NameToLayer ("Ignore Raycast");

			for (int i = 0; i < rigidbodyList.Count; i++) {
				rigidbodyList [i].mass = rigidbodyMassValue;
				rigidbodyList [i].gameObject.layer = ignoreRaycastLayerIndex;
			}
		} 

		if (setExtraListKinematic) {
			for (int i = 0; i < extraListKinematic.Count; i++) {
				if (extraListKinematic [i] != null) {
					extraListKinematic [i].isKinematic = true;
				}
			}
		}

		if (objectIsCharacter) {
			if (character != null) {
				ragdollActivator currentRagdollActivator = character.GetComponent<ragdollActivator> ();

				if (currentRagdollActivator != null) {
					currentRagdollActivator.setCheckGetUpPausedState (true);
				}
			}
		}
	
		checkEventsOnGrabDrop (true);

		checkRemoteEvents (true);

		if (disableObjectColliderWhileGrabbed) {
			setMainColliderEnabledState (false);
		}
	}

	public void dropObject ()
	{
		//print ("drop object");
		objectGrabed = false;

		lastPositionBeforeDropped = objectToGrabParent.transform.position;
		lastRotationBeforeDropped = objectToGrabParent.transform.rotation;

		if (grabObjectsManager != null) {
			grabObjectsManager.dropPhysicalObject ();
		}

		grabObjectsManager = null;

		setGrabObjectTriggerState (true);

		if (setExtraListKinematic) {
			for (int i = 0; i < extraListKinematic.Count; i++) {
				if (extraListKinematic [i] != null) {
					extraListKinematic [i].isKinematic = false;
				}
			}
		}

		if (setRigidbodyMassValue) {
			for (int i = 0; i < rigidbodyList.Count; i++) {
				rigidbodyList [i].mass = rigidbodyMassList [i];
				rigidbodyList [i].gameObject.layer = layerMaskList [i];
			}
		} 

		if (objectIsCharacter) {
			assignObjectToGrabParentIfNull ();

			if (character != null) {
				ragdollActivator currentRagdollActivator = character.GetComponent<ragdollActivator> ();

				if (currentRagdollActivator != null) {
					currentRagdollActivator.setCheckGetUpPausedState (false);

					if (characterBody == null) {
						characterBody = currentRagdollActivator.characterBody.transform;
					}
				}
			}

			if (characterBody != null) {
				characterBody.transform.position = objectToGrabParent.transform.position;
			}

			objectToGrabParent.transform.SetParent (characterBody);
		}
			
		checkEventsOnGrabDrop (false);

		checkRemoteEvents (false);

		if (disableObjectColliderWhileGrabbed) {
			setMainColliderEnabledState (true);
		}

		keepGrabbedObjectState = false;

		currentObjectParent = null;
	}

	public void checkParentAssignedState ()
	{
		checkNewParentAssignedState (objectToGrabParent.transform.parent);
	}

	public void checkNewParentAssignedState (Transform newParent)
	{
		
	}

	public bool isUseReferencePositionForEveryViewActive ()
	{
		return useReferencePositionForEveryView;
	}

	public Transform getReferencePositionThirdPerson ()
	{
		return referencePositionThirdPerson;
	}

	public Transform getReferencePositionFirstPerson ()
	{
		return referencePositionFirstPerson;
	}

	public Transform getReferencePosition ()
	{
		return referencePosition;
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
		if (objectGrabed) {
			return;
		}
	
		if ((1 << col.gameObject.layer & layerForUsers.value) == 1 << col.gameObject.layer) {
			//if the player is entering in the trigger
			currentGrabObjectDetected = col.gameObject.GetComponent<grabObjects> ();

			if (currentGrabObjectDetected != null) {
				assignObjectToGrabParentIfNull ();

				if (isEnter) {

					if (!currentGrabObjectDetected.isGrabObjectsPhysicallyEnabled ()) {
						return;
					}

					currentUser = col.gameObject;

					if (grabObjectPhysically) {
						if (objectIsCharacter) {
							if (character == currentUser) {
								return;
							}
						}
			
						grabObjectsList.Add (currentGrabObjectDetected);

						currentGrabObjectDetected.addCurrentPhysicalObjectToGrabFound (objectToGrabParent);
					}
				} else {
					if (objectToGrabParent != null) {
						currentGrabObjectDetected.removeCurrentPhysicalObjectToGrabFound (objectToGrabParent);
					}

					grabObjectsList.Remove (currentGrabObjectDetected);
				}
			}
		}
	}

	public void removeAllGrabObjectsFound ()
	{
		assignObjectToGrabParentIfNull ();

		for (int i = grabObjectsList.Count - 1; i >= 0; i--) {

			if (grabObjectsList [i].isCarryingPhysicalObject () && grabObjectsList [i].getCurrentPhysicalObjectGrabbed () != null &&
			    grabObjectsList [i].getCurrentPhysicalObjectGrabbed () == objectToGrabParent) {

				grabObjectsList [i].dropObject ();
			}

			grabObjectsList [i].removeCurrentPhysicalObjectToGrabFound (objectToGrabParent);

			grabObjectsList.RemoveAt (i);
		}
	}

	public void setCurrentPlayer (GameObject newPlayer)
	{
		checkTriggerInfo (newPlayer.GetComponent<Collider> (), true);
	}

	public bool isGrabObjectPhysicallyEnabled ()
	{
		return grabObjectPhysically;
	}

	public bool isCanBeGrabbedOnFirstPersonEnabled ()
	{
		return canBeGrabbedOnFirstPerson;
	}

	public bool isCanBeGrabbedOnThirdPersonEnabled ()
	{
		return canBeGrabbedOnThirdPerson;
	}

	public void disableGrabPhysicalObject ()
	{
		grabObjectPhysically = false;

		if (currentUser != null) {

			assignObjectToGrabParentIfNull ();
			
			currentUser.GetComponent<grabObjects> ().removeCurrentPhysicalObjectToGrabFound (objectToGrabParent);
		}
	}

	public void setGrabObjectPhysicallyEnabledState (bool state)
	{
		grabObjectPhysically = state;
	}

	public void setGrabObjectPhysicallyEnabledStateFromEditor (bool state)
	{
		setGrabObjectPhysicallyEnabledState (state);

		updateComponent ();
	}

	public void setMainColliderEnabledState (bool state)
	{
		if (mainCollider != null) {
			mainCollider.enabled = state;
		}
	}

	public void setGrabObjectTriggerState (bool state)
	{
		if (grabObjectTrigger != null) {
			grabObjectTrigger.enabled = state;
		}
	}

	public void setGrabObjectTriggerStateFromEditor (bool state)
	{
		setGrabObjectTriggerState (state);

		updateComponent ();
	}

	public void setCharacterBody (Transform newCharacterBody)
	{
		characterBody = newCharacterBody;

		updateComponent ();
	}

	public void setCharacter (GameObject newCharacter)
	{
		character = newCharacter;

		updateComponent ();
	}


	public void setObjectToGrabParent (GameObject newObjectToGrabParent)
	{
		objectToGrabParent = newObjectToGrabParent;

		updateComponent ();
	}

	public void setActiveOrInactiveObjectState (bool state)
	{
		if (state) {
			if (tagForObjectWhenActive == "") {
				tagForObjectWhenActive = "box";
			}

			objectToGrabParent.tag = tagForObjectWhenActive;
		} else {
			if (tagForObjectWhenInactive == "") {
				tagForObjectWhenInactive = "Untagged";
			}

			objectToGrabParent.tag = tagForObjectWhenInactive;
		}
	}

	public void checkEventsOnGrabDrop (bool state)
	{
		if (useEventsOnGrabDrop) {
			if (state) {
				eventOnGrab.Invoke ();
			} else {
				eventOnDrop.Invoke ();
			}
		}
	}

	public void disableGrabPhysicalObjectComponent ()
	{
		removeAllGrabObjectsFound ();

		setGrabObjectTriggerState (false);

		setActiveOrInactiveObjectState (false);
	}

	public bool hasObjectMeleeAttackSystem ()
	{
		return useMeleeAttackSystem;
	}

	public grabPhysicalObjectMeleeAttackSystem getMainGrabPhysicalObjectMeleeAttackSystem ()
	{
		return mainGrabPhysicalObjectMeleeAttackSystem;
	}

	public void activateReturnProjectilesOnContact ()
	{
		if (grabObjectsManager != null) {
			grabObjectsManager.activateReturnProjectilesOnContact ();
		}
	}

	public void checkRemoteEvents (bool state)
	{
		if (state) {
			checkRemoteEventOnStart ();
		} else {
			checkRemoteEventOnEnd ();
		}
	}

	public void checkRemoteEventOnStart ()
	{
		if (useRemoteEventOnObjectsFound) {
			if (currentUser != null) {
				remoteEventSystem currentRemoteEventSystem = currentUser.GetComponent<remoteEventSystem> ();

				if (currentRemoteEventSystem != null) {
					for (int i = 0; i < remoteEventNameListOnStart.Count; i++) {
						currentRemoteEventSystem.callRemoteEvent (remoteEventNameListOnStart [i]);
					}
				}
			}
		}
	}

	public void checkRemoteEventOnEnd ()
	{
		if (useRemoteEventOnObjectsFound) {
			if (currentUser != null) {
				remoteEventSystem currentRemoteEventSystem = currentUser.GetComponent<remoteEventSystem> ();

				if (currentRemoteEventSystem != null) {
					for (int i = 0; i < remoteEventNameListOnEnd.Count; i++) {
						currentRemoteEventSystem.callRemoteEvent (remoteEventNameListOnEnd [i]);
					}
				}
			}
		}
	}

	public void setKeepOrCarryGrabbebObjectState (bool state)
	{
		if (objectGrabed) {
			keepGrabbedObjectState = state;

			if (useMeleeAttackSystem) {
				mainGrabPhysicalObjectMeleeAttackSystem.setKeepOrCarryGrabbebObjectState (state);
			}
		}
	}

	public void assignObjectParent (Transform newParent)
	{
		currentObjectParent = newParent;
	}

	public Transform getCurrentObjectParent ()
	{
		return currentObjectParent;
	}

	public Vector3 getLastPositionBeforeGrabbed ()
	{
		return lastPositionBeforeGrabbed;
	}

	public Quaternion getLastRotationBeforeGrabbed ()
	{
		return lastRotationBeforeGrabbed;
	}

	public Vector3 getLastPositionBeforeDropped ()
	{
		return lastPositionBeforeDropped;
	}

	public Quaternion getLastRotationBeforeDropped ()
	{
		return lastRotationBeforeDropped;
	}

	Transform lastParentAssigned;

	public void setLastParentAssigned (Transform newParent)
	{
		lastParentAssigned = newParent;
	}

	public Transform getLastParentAssigned ()
	{
		return lastParentAssigned;
	}

	void updateComponent ()
	{
		GKC_Utils.updateComponent (this);
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
			GKC_Utils.drawRectangleGizmo (transform.position, transform.rotation, colliderOffset, colliderScale, gizmoColor);

			if (rightHandPosition) {
				Gizmos.DrawSphere (rightHandPosition.position, 0.2f);
			}

			if (lefHandPosition) {
				Gizmos.DrawSphere (lefHandPosition.position, 0.2f);
			}
		}
	}

	[System.Serializable]
	public class grabPhysicalObjectInfo
	{
		public GameObject objectToGrab;
		public bool IKSystemEnabled;

		public bool changeAnimationSpeed;
		public float animationSpeed;

		public bool setNewMovementTreeID;
		public int newMovementTreeID;

		public bool applyAnimatorVelocityWithoutMoving;

		public bool disableJumpOnGrabbedObject;
		public bool disableRunOnGrabbedObject;

		public Transform rightHandPosition;
		public Transform lefHandPosition;
		public bool useRightHand;
		public bool useLeftHand;
		public bool useRightElbow;
		public bool useLeftElbow;
		public Transform rightElbowPosition;
		public Transform lefElbowPosition;

		public bool useRightHandForObjectParent;

		public bool useAnimationGrabbingHand;
		public int rightGrabbingHandID;
		public int leftGrabbingHandID;
	}
}
