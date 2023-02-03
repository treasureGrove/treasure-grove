using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class characterStateIconSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool statesEnabled = true;

	public bool useIconsOnlyOnScreen;

	[Space]
	[Header ("Character States List Settings")]
	[Space]

	public List<characterStateInfo> characterStateInfoList = new List<characterStateInfo> ();

	[Space]
	[Header ("Debug")]
	[Space]

	public string currentCharacterStateName;
	public characterStateInfo currentCharacterState;

	public bool hideAfterTime;

	[Space]
	[Header ("Components")]
	[Space]

	public AudioSource mainAudioSource;
	public playerController playerControllerManager;
	public Transform mainCameraTransform;

	Vector3 directionToCamera;
	Transform currentIconGameObjectThirdPerson;

	bool currentIconGameObjectThirdPersonAssigned;

	bool mainCameraTransformLocated;

	bool playerControllerManagerLocated;


	void Update ()
	{
		if (hideAfterTime) {
			if (Time.time > currentCharacterState.lastTimeHidden + currentCharacterState.hideAfterTimeAmount) {

				enableOrDisableCharacterIcon (currentCharacterState, false);

				hideAfterTime = false;
				currentCharacterState = null;
				currentIconGameObjectThirdPerson = null;

				currentIconGameObjectThirdPersonAssigned = false;

				currentCharacterStateName = "";
			}
		}

		if (currentIconGameObjectThirdPersonAssigned && mainCameraTransformLocated) {
			if (playerControllerManagerLocated) {
				if (!playerControllerManager.isPlayerOnFirstPerson () && !useIconsOnlyOnScreen) {
					directionToCamera = currentIconGameObjectThirdPerson.position - mainCameraTransform.position;

					currentIconGameObjectThirdPerson.rotation = Quaternion.LookRotation (directionToCamera);
				}
			} else {
				directionToCamera = currentIconGameObjectThirdPerson.position - mainCameraTransform.position;

				currentIconGameObjectThirdPerson.rotation = Quaternion.LookRotation (directionToCamera);
			}
		}
	}

	void checkMainCameraTransform ()
	{
		if (mainCameraTransformLocated) {
			return;
		}

		if (mainCameraTransform == null) {

			mainCameraTransform = GKC_Utils.findMainPlayerCameraTransformOnScene ();

			if (mainCameraTransform != null) {
				mainCameraTransformLocated = true;
			}
		} else {
			mainCameraTransformLocated = true;
		}
	}

	public void enableOrDisableCharacterIcon (characterStateInfo characterState, bool state)
	{
		if (!statesEnabled) {
			return;
		}

		checkMainCameraTransform ();

		if (playerControllerManagerLocated) {
			if (playerControllerManager.isPlayerOnFirstPerson () || playerControllerManager.isUsingDevice () || useIconsOnlyOnScreen) {
				if (characterState.iconThirdPersonActiveState) {
					characterState.iconGameObjectThirdPerson.SetActive (false);

					characterState.iconThirdPersonActiveState = false;
				}

				if (characterState.iconFirstPersonActiveState != state) {
					if (characterState.iconGameObjectFirstPerson != null) {
						characterState.iconGameObjectFirstPerson.SetActive (state);

						characterState.iconFirstPersonActiveState = state;
					}
				}
			} else {
				if (characterState.iconThirdPersonActiveState != state) {
					characterState.iconGameObjectThirdPerson.SetActive (state);
					 
					characterState.iconThirdPersonActiveState = state;
				}

				if (characterState.iconFirstPersonActiveState) {
					if (characterState.iconGameObjectFirstPerson != null) {
						characterState.iconGameObjectFirstPerson.SetActive (false);

						characterState.iconFirstPersonActiveState = false;
					}
				}
			}
		} else {
			if (characterState.iconFirstPersonActiveState != state) {
				if (characterState.iconGameObjectFirstPerson != null) {
					characterState.iconGameObjectFirstPerson.SetActive (state);

					characterState.iconFirstPersonActiveState = state;
				}
			}

			if (characterState.iconThirdPersonActiveState != state) {
				characterState.iconGameObjectThirdPerson.SetActive (state);

				characterState.iconThirdPersonActiveState = state;
			}
		}
	}

	public void checkCharacterStateIconForViewChange ()
	{
		if (currentIconGameObjectThirdPerson != null) {
			enableOrDisableCharacterIcon (currentCharacterState, true);
		}
	}
		
	public void setCharacterStateIcon (string stateName)
	{
		if (!statesEnabled) {
			return;
		}

		if (currentCharacterStateName.Equals (stateName)) {
			return;
		}

		if (!playerControllerManagerLocated) {
			playerControllerManagerLocated = playerControllerManager != null;
		}

		currentCharacterState = null;

		for (int i = 0; i < characterStateInfoList.Count; i++) {
			characterStateInfo temporalCharacterStateInfo = characterStateInfoList [i];

			if (temporalCharacterStateInfo.Name.Equals (stateName)) {

				currentCharacterState = temporalCharacterStateInfo;

				currentCharacterStateName = currentCharacterState.Name;

				currentIconGameObjectThirdPerson = currentCharacterState.iconGameObjectThirdPerson.transform;

				currentIconGameObjectThirdPersonAssigned = true;

				enableOrDisableCharacterIcon (currentCharacterState, true);

				if (currentCharacterState.hideAfterTime) {
					currentCharacterState.lastTimeHidden = Time.time;
					hideAfterTime = true;
				} else {
					hideAfterTime = false;
				}

				if (currentCharacterState.useSound) {
					playSound (currentCharacterState.stateClip);
				}
			} else {
				enableOrDisableCharacterIcon (temporalCharacterStateInfo, false);
			}
		}
	}

	public void disableCharacterStateIcon ()
	{
		//print ("disable states icon");
		for (int i = 0; i < characterStateInfoList.Count; i++) {
			enableOrDisableCharacterIcon (characterStateInfoList [i], false);
		}

		currentCharacterStateName = "";
	}

	public void playSound (AudioClip sound)
	{
		if (mainAudioSource != null) {
			if (mainAudioSource.gameObject.activeSelf && mainAudioSource.enabled) {
				mainAudioSource.PlayOneShot (sound);
			}
		}
	}

	[System.Serializable]
	public class characterStateInfo
	{
		public string Name;
		public GameObject iconGameObjectThirdPerson;
		public GameObject iconGameObjectFirstPerson;
		public bool hideAfterTime;
		public float hideAfterTimeAmount;
		public float lastTimeHidden;
		public bool useSound;
		public AudioClip stateClip;

		public bool iconThirdPersonActiveState;
		public bool iconFirstPersonActiveState;
	}
}
