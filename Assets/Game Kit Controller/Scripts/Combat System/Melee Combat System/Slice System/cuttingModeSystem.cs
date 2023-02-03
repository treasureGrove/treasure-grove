using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class cuttingModeSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool cuttingModeActive;

	public float cuttingPanelRotationSpeed = 5;

	public float cuttingSpeed = 10;

	public bool playerCanMoveOnCuttingModeActive = true;

	public bool useCameraStateOnCuttingMode;
	public string cameraStateNameOnCuttingMode;

	public bool setBulletTime;
	public float bulletTimeScale;
	public float delayToSetBulletTime;

	public bool canToggleCameraRotation;

	public bool pauseCameraRotationAtStartOnCutModeActive = true;

	[Space]
	[Header ("Animation Settings")]
	[Space]

	public int cuttingModeStateID = 50000;
	public string cuttingModeActionActiveName = "Action Active Upper Body";
	public string cuttingModeActionIDName = "Action ID";

	public string horizontalAnimatorName = "Horizontal Action";
	public string verticalAnimatorName = "Vertical Action";

	[Space]
	[Header ("Stamina Settings")]
	[Space]

	public bool useStaminaOnSliceEnabled = true;
	public string sliceStaminaState = "Slice Object";
	public float staminaToUseOnSlice = 10;
	public float customRefillStaminaDelayAfterSlice;

	public float generalStaminaUseMultiplier = 1;

	[Space]
	[Header ("Events Settings")]
	[Space]

	public UnityEvent eventOnCuttingModeStart;
	public UnityEvent eventOnCuttingModeEnd;

	public UnityEvent eventOnCutting;

	[Space]
	[Header ("Components")]
	[Space]

	public Transform cutParticlesParent;
	public GameObject cuttingPanelGameObject;
	public Transform cuttingPanelTransform;
	public Transform cuttingPanelTransformReference;
	public Transform cuttingPanelFirstReferenceTransform;
	public Transform cuttingPanelSecondReferenceTransform;

	public Transform cuttingDirectionTransform;

	public playerController mainPlayerController;
	public playerCamera mainPlayerCamera;
	public playerInputManager mainPlayerInputManager;
	public timeBullet timeBulletManager;

	public staminaSystem mainStaminaSystem;

	public Animator mainAnimator;

	string previousCameraStateName;

	Vector2 axisValues;

	int horizontalAnimatorID;
	int verticalAnimatorID;

	float horizontalRotationValue;
	float verticalRotationValue;

	float cutDirection = 1;

	Coroutine moveReferencePanelOnCutCoroutine;

	Coroutine bulletTimeCoroutine;

	bool cutPanelRotationActive = true;

	bool sliceInputPausedForStamina;

	float horizontalOffset = 0.3f;
	float verticalOffset = 0.18f;

	Vector2 rotationValues;

	void Update ()
	{
		if (cuttingModeActive) {
			if (mainPlayerCamera.cameraCanBeUsedActive ()) {
				if (cutPanelRotationActive) {
					axisValues = mainPlayerInputManager.getPlayerMouseAxis ();
				} else {
					axisValues = Vector2.zero;
				}

				cuttingPanelTransform.localEulerAngles += new Vector3 (0, 0, -axisValues.x * cuttingPanelRotationSpeed);

				cuttingPanelTransformReference.localEulerAngles = cuttingPanelTransform.localEulerAngles;

				cuttingPanelSecondReferenceTransform.position = cuttingPanelFirstReferenceTransform.position;

				horizontalRotationValue = Mathf.Clamp (cuttingPanelSecondReferenceTransform.localPosition.x + horizontalOffset, -1, 1);
				verticalRotationValue = Mathf.Clamp (cuttingPanelSecondReferenceTransform.localPosition.y + verticalOffset, -1, 1);

				rotationValues = new Vector2 (horizontalRotationValue, verticalRotationValue);

				rotationValues = Vector2.ClampMagnitude (rotationValues, 1);

				mainAnimator.SetFloat (horizontalAnimatorID, rotationValues.x);
				mainAnimator.SetFloat (verticalAnimatorID, rotationValues.y);
			}
		}
	}

	public void setSliceInputPausedForStaminaState (bool state)
	{
		sliceInputPausedForStamina = state;
	}

	public void inputToggleCameraRotation ()
	{
		if (cuttingModeActive) {
			if (canToggleCameraRotation) {
				cutPanelRotationActive = !cutPanelRotationActive;

				mainPlayerCamera.changeCameraRotationState (!cutPanelRotationActive);
			}
		}
	}

	public void inputActivateCut ()
	{
		if (cuttingModeActive) {
//			stopActivateCutCoroutine ();

			if (sliceInputPausedForStamina) {
//				print ("Not enough stamina");

				return;
			}

			if (cuttingInProcess) {
				return;
			}

			if (useStaminaOnSliceEnabled) {
				mainStaminaSystem.activeStaminaStateWithCustomAmount (sliceStaminaState, staminaToUseOnSlice * generalStaminaUseMultiplier, customRefillStaminaDelayAfterSlice);				
			}
				
			moveReferencePanelOnCutCoroutine = StartCoroutine (activateCutCoroutine ());

			cutParticlesParent.localEulerAngles += Vector3.forward * 180;

			cuttingDirectionTransform.localEulerAngles += Vector3.up * 180;

			eventOnCutting.Invoke ();
		}
	}

	public void setCuttingModeActiveState (bool state)
	{
		cuttingModeActive = state;

		if (cuttingModeActive) {
			horizontalAnimatorID = Animator.StringToHash (horizontalAnimatorName);
			verticalAnimatorID = Animator.StringToHash (verticalAnimatorName);

			cuttingPanelTransform.localEulerAngles = Vector3.zero;
			cuttingPanelTransformReference.localEulerAngles = cuttingPanelTransform.localEulerAngles;

			stopActivateCutCoroutine ();

			cutDirection = 1;

			cuttingPanelFirstReferenceTransform.localPosition = Vector3.right * cutDirection * 1.3f;

			cutParticlesParent.localEulerAngles = Vector3.zero;

			cuttingDirectionTransform.localEulerAngles = Vector3.zero;

			cutPanelRotationActive = true;
		}

		cuttingPanelGameObject.SetActive (cuttingModeActive);

		if (useCameraStateOnCuttingMode) {
			if (cuttingModeActive) {
				previousCameraStateName = mainPlayerCamera.getCurrentStateName ();

				mainPlayerCamera.setCameraState (cameraStateNameOnCuttingMode);
			} else {
				mainPlayerCamera.setCameraState (previousCameraStateName);

				previousCameraStateName = "";
			}
		}

		if (cuttingModeActive) {
			mainAnimator.SetInteger (cuttingModeActionIDName, cuttingModeStateID);
		} else {
			mainAnimator.SetInteger (cuttingModeActionIDName, 0);
		}

		mainAnimator.SetBool (cuttingModeActionActiveName, cuttingModeActive);

		if (pauseCameraRotationAtStartOnCutModeActive) {
			mainPlayerCamera.changeCameraRotationState (!cuttingModeActive);
		}

		if (!playerCanMoveOnCuttingModeActive) {
			if (cuttingModeActive) {
				mainPlayerController.smoothChangeScriptState (false);
		
				mainPlayerController.setCanMoveState (false);
	
				mainPlayerController.resetPlayerControllerInput ();
			
				mainPlayerController.resetOtherInputFields ();
			} else {
				mainPlayerController.setCanMoveState (true);
			}
		}

		checkSetBulletTime ();

		if (cuttingModeActive) {
			eventOnCuttingModeStart.Invoke ();
		} else {
			eventOnCuttingModeEnd.Invoke ();
		}
	}

	public void checkSetBulletTime ()
	{
		if (setBulletTime) {
			stopCheckSetBulletTimeCoroutine ();

			if (cuttingModeActive) {
				bulletTimeCoroutine = StartCoroutine (checkSetBulletTimeCoroutine ());
			} else {
				timeBulletManager.setBulletTimeState (false, 1);
			}
		}
	}

	public void stopCheckSetBulletTimeCoroutine ()
	{
		if (bulletTimeCoroutine != null) {
			StopCoroutine (bulletTimeCoroutine);
		}
	}

	IEnumerator checkSetBulletTimeCoroutine ()
	{
		yield return new WaitForSeconds (delayToSetBulletTime);

		timeBulletManager.setBulletTimeState (true, bulletTimeScale);
	}

	bool cuttingInProcess;

	public void stopActivateCutCoroutine ()
	{
		if (moveReferencePanelOnCutCoroutine != null) {
			StopCoroutine (moveReferencePanelOnCutCoroutine);
		}
	}

	IEnumerator activateCutCoroutine ()
	{
		cuttingInProcess = true;

		float movementSpeed = cuttingSpeed;

		bool targetReached = false;

		float positionDifference = 0;

		cutDirection *= (-1);

		Vector3 targetPosition = Vector3.right * (cutDirection * 1.3f);

		float dist = GKC_Utils.distance (cuttingPanelFirstReferenceTransform.localPosition, targetPosition);

		float duration = dist / movementSpeed;

		float t = 0;

		float movementTimer = 0;

		while (!targetReached) {
			t += Time.deltaTime / duration; 

			cuttingPanelFirstReferenceTransform.localPosition = 
				Vector3.Lerp (cuttingPanelFirstReferenceTransform.localPosition, targetPosition, t);

			positionDifference = GKC_Utils.distance (cuttingPanelFirstReferenceTransform.localPosition, targetPosition);

			movementTimer += Time.deltaTime;

			if (positionDifference < 0.01f || movementTimer > (duration + 1)) {
				targetReached = true;
			}
			yield return null;
		}

		cuttingInProcess = false;
	}

	public void setGeneralStaminaUseMultiplierValue (float newValue)
	{
		generalStaminaUseMultiplier = newValue;
	}

	public void setUseStaminaOnSliceEnabledState (bool state)
	{
		useStaminaOnSliceEnabled = state;
	}
}