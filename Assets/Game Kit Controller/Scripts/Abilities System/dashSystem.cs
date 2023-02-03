using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class dashSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool dashEnabled = true;
	public float maxTimeBetweenInputPress;

	public bool useDashCoolDown;
	public float dashCoolDownTime;

	public bool canUseDashWhileActionActive;

	public List<string> actionCategoryToUseDash = new List<string> ();

	[Space]
	[Header ("Dash In First Person Settings")]
	[Space]

	public bool useDashOnFirstPerson = true;

	public float dashDuration;

	public float dashSpeed;

	public string dashCameraState;

	public float timeToDisableDashCamera;

	public bool onlyDashOnGround;

	public bool useDoubleTapToDashOnFirstPersonEnabled;

	[Space]
	[Header ("Dash In Third Person Settings")]
	[Space]

	public bool useDashOnThirdPerson;

	[Space]
	[Header ("Dash Info List Settings")]
	[Space]

	public int currentDashID;

	[Space]
	[Space]

	public List<dashInfo> dashInfoList = new List<dashInfo> ();

	[Space]
	[Header ("Debug")]
	[Space]

	public bool showDebugPrint;
	public bool dashActive;
	public bool dashCanBeUsedCurrently;
	public bool dashOnThirdPersonInTransition;

	public bool checkGroundPaused;

	public bool overrideStrafeModeActiveState;

	[Space]
	[Header ("Components")]
	[Space]

	public playerController mainPlayerController;
	public playerInputManager playerInput;
	public playerCamera mainPlayerCamera;

	public staminaSystem mainStaminaSystem;

	Vector2 dashMovementDirection;

	float lastTimeRightInput;
	float lastTimeLeftInput;
	float lastTimeUpInput;
	float lastTimeDownInput;

	bool rightInputPressed;
	bool leftInputPressed;
	bool upInputPressed;
	bool downInputPressed;

	Vector2 rawAxisValues;

	float lastTimeDashActive;

	float originalNoAnimatorSpeedValue;

	string originalCameraStateName;

	bool dashCameraActive;

	bool dashActivatedOnFirstPerson;

	thirdPersonStrafeDashDirection thirdPersonStrafeDashDirectionResult;

	enum thirdPersonStrafeDashDirection
	{
		forward,
		backward,
		right,
		left,
		forwardRight,
		forwardLeft,
		backwardRight,
		backwardLeft
	}

	bool dashOnThirdPersonPaused;

	bool initialized;

	bool customDashDirectionActive;
	Vector3 customDashDirection;

	dashInfo currentDashInfo;


	void Update ()
	{
		if (dashEnabled) {
			if (!initialized) {
				setCurrentDashID (currentDashID);

				initialized = true;
			}

			dashCanBeUsedCurrently = canUseDash ();

			if (dashCanBeUsedCurrently) {
				rawAxisValues = mainPlayerController.getRawAxisValues ();

				if (canUseDashWhileActionActive && mainPlayerController.isActionActive ()) {
					rawAxisValues = playerInput.getPlayerRawMovementAxis ();
				}

				if (rawAxisValues.x > 0) {
					if (!rightInputPressed) {
						rightInputPressed = true;
					}

					if (lastTimeRightInput > 0) {
						if (Time.time < lastTimeRightInput + maxTimeBetweenInputPress) {
							checkDoubleTapDash ();
						}

						lastTimeRightInput = 0;
					}
				} else if (rawAxisValues.x == 0) {
					if (rightInputPressed) {
						rightInputPressed = false;

						lastTimeRightInput = Time.time;
					}
				}

				if (rawAxisValues.x < 0) {
					if (!leftInputPressed) {
						leftInputPressed = true;
					}

					if (lastTimeLeftInput > 0) {
						if (Time.time < lastTimeLeftInput + maxTimeBetweenInputPress) {
							checkDoubleTapDash ();
						}

						lastTimeLeftInput = 0;
					} 
				} else if (rawAxisValues.x == 0) {
					if (leftInputPressed) {
						leftInputPressed = false;

						lastTimeLeftInput = Time.time;
					}
				}

				if (rawAxisValues.y > 0) {
					if (!upInputPressed) {
						upInputPressed = true;
					}

					if (lastTimeUpInput > 0) {
						if (Time.time < lastTimeUpInput + maxTimeBetweenInputPress) {
							checkDoubleTapDash ();
						}

						lastTimeUpInput = 0;
					}
				} else if (rawAxisValues.y == 0) {
					if (upInputPressed) {
						upInputPressed = false;

						lastTimeUpInput = Time.time;
					}
				}

				if (rawAxisValues.y < 0) {
					if (!downInputPressed) {
						downInputPressed = true;
					}

					if (lastTimeDownInput > 0) {
						if (Time.time < lastTimeDownInput + maxTimeBetweenInputPress) {
							checkDoubleTapDash ();
						}

						lastTimeDownInput = 0;
					}
				} else if (rawAxisValues.y == 0) {
					if (downInputPressed) {
						downInputPressed = false;

						lastTimeDownInput = Time.time;
					}
				}
			}

			if (dashActive) {
				if (dashActivatedOnFirstPerson) {
					if (Time.time > lastTimeDashActive + dashDuration) {
						disableDashState ();
					} else {
						playerInput.overrideInputValues (dashMovementDirection, true);

						mainPlayerController.inputStartToRun ();

						if (dashCameraActive && Time.time > lastTimeDashActive + timeToDisableDashCamera) {
							mainPlayerCamera.setCameraState (originalCameraStateName);

							dashCameraActive = false;
						}
					}
				} else {
					if (useDashOnThirdPerson) {
						if (Time.time > lastTimeDashActive + currentDashInfo.dashDurationOnThirdPerson) {
							disableDashState ();
						}
					}
				}
			}
		}
	}

	public void checkDoubleTapDash ()
	{
		if (!canUseDash ()) {
			return;
		}

		bool isFirstPersonActive = mainPlayerController.isPlayerOnFirstPerson ();

		if (isFirstPersonActive) {
			if (useDoubleTapToDashOnFirstPersonEnabled) {
				enableDashState ();
			}
		} else {
			if (currentDashInfo.useDoubleTapToDashOnThirdPersonEnabled) {
				enableDashState ();
			}
		}
	}

	public void enableDashState ()
	{
		if (dashActive) {
			return;
		}

		dashActivatedOnFirstPerson = mainPlayerController.isPlayerOnFirstPerson ();

		dashActive = true;

		lastTimeDashActive = Time.time;

		dashMovementDirection = mainPlayerController.getRawAxisValues ();

		if (canUseDashWhileActionActive && mainPlayerController.isActionActive ()) {
			dashMovementDirection = playerInput.getPlayerRawMovementAxis ();
		}

		if (dashMovementDirection == Vector2.zero) {
			dashMovementDirection = new Vector2 (0, 1);
		}

		originalNoAnimatorSpeedValue = mainPlayerController.noAnimatorSpeed;

		if (dashActivatedOnFirstPerson) {
			mainPlayerController.setNoAnimatorSpeedValue (dashSpeed);

			originalCameraStateName = mainPlayerCamera.getCurrentStateName ();

			mainPlayerCamera.setCameraState (dashCameraState);
		} else {
			if (useDashOnThirdPerson) {

				if (!mainPlayerController.isActionActive () || canUseDashWhileActionActive) {
					bool strafeModeActive = false;

					if (currentDashInfo.dashOnThirdPersonPaused) {
						return;
					}

					if (currentDashInfo.checkDashInStrafeState) {
						if (mainPlayerController.isStrafeModeActive ()) {
							strafeModeActive = true;
						}
					}

					if (overrideStrafeModeActiveState) {
						strafeModeActive = true;
					}

					if (strafeModeActive) {
						Vector2 axisValues = mainPlayerController.getRawAxisValues ();

						if (canUseDashWhileActionActive && mainPlayerController.isActionActive ()) {
							axisValues = playerInput.getPlayerRawMovementAxis ();
						}

						if (customDashDirectionActive) {
							axisValues = customDashDirection;

							customDashDirectionActive = false;

							rawAxisValues = axisValues;
						}

						float angle = Vector2.SignedAngle (axisValues, Vector2.up);

						if (!mainPlayerCamera.isCameraTypeFree ()) {
							Transform currentLockedCameraAxis = mainPlayerCamera.getCurrentLockedCameraAxis ();
							Vector3 direction = currentLockedCameraAxis.forward * axisValues.y + currentLockedCameraAxis.right * axisValues.x;
						
							angle = Vector3.SignedAngle (direction, mainPlayerController.transform.forward, mainPlayerCamera.transform.up);

							if (currentLockedCameraAxis.eulerAngles.y > 0 || currentLockedCameraAxis.eulerAngles.y == 0) {
								angle = -angle;
							} else {
								angle = Mathf.Abs (angle);
							}
						}
							
						float ABSAngle = Mathf.Abs (angle);

						bool checkDiagonals = false;

						if (currentDashInfo.checkDiagonalInputInStrafeState) {
							checkDiagonals = true;
						}
					
						if (checkDiagonals) {
							if (angle > 0) {
								if (ABSAngle < 35) {
									thirdPersonStrafeDashDirectionResult = thirdPersonStrafeDashDirection.forward;
								} else if (angle > 35 && angle < 80) {
									thirdPersonStrafeDashDirectionResult = thirdPersonStrafeDashDirection.forwardRight;
								} else if (angle > 80 && angle < 125) {
									thirdPersonStrafeDashDirectionResult = thirdPersonStrafeDashDirection.right;
								} else if (angle > 125 && angle < 170) {
									thirdPersonStrafeDashDirectionResult = thirdPersonStrafeDashDirection.backwardRight;
								} else {
									thirdPersonStrafeDashDirectionResult = thirdPersonStrafeDashDirection.backward;
								}
							} else {
								if (ABSAngle < 35) {
									thirdPersonStrafeDashDirectionResult = thirdPersonStrafeDashDirection.forward;
								} else if (ABSAngle > 35 && ABSAngle < 80) {
									thirdPersonStrafeDashDirectionResult = thirdPersonStrafeDashDirection.forwardLeft;
								} else if (ABSAngle > 80 && ABSAngle < 125) {
									thirdPersonStrafeDashDirectionResult = thirdPersonStrafeDashDirection.left;
								} else if (ABSAngle > 125 && ABSAngle < 170) {
									thirdPersonStrafeDashDirectionResult = thirdPersonStrafeDashDirection.backwardLeft;
								} else {
									thirdPersonStrafeDashDirectionResult = thirdPersonStrafeDashDirection.backward;
								}
							}
						} else {
							if (angle > 0) {
								if (ABSAngle < 80) {
									thirdPersonStrafeDashDirectionResult = thirdPersonStrafeDashDirection.forward;
								} else if (angle > 80 && angle < 170) {
									thirdPersonStrafeDashDirectionResult = thirdPersonStrafeDashDirection.right;
								} else {
									thirdPersonStrafeDashDirectionResult = thirdPersonStrafeDashDirection.backward;
								}
							} else {
								if (ABSAngle < 80) {
									thirdPersonStrafeDashDirectionResult = thirdPersonStrafeDashDirection.forward;
								} else if (ABSAngle > 80 && ABSAngle < 170) {
									thirdPersonStrafeDashDirectionResult = thirdPersonStrafeDashDirection.left;
								} else {
									thirdPersonStrafeDashDirectionResult = thirdPersonStrafeDashDirection.backward;
								}
							}
						}

						if (rawAxisValues == Vector2.zero) {
							thirdPersonStrafeDashDirectionResult = thirdPersonStrafeDashDirection.forward;
						}

						if (showDebugPrint) {
							print (angle + " " + thirdPersonStrafeDashDirectionResult);
						}

						switch (thirdPersonStrafeDashDirectionResult) {
						case thirdPersonStrafeDashDirection.forward:
							currentDashInfo.eventOnDashOnThirdPersonForward.Invoke ();

							if (showDebugPrint) {
								print ("movement forward");
							}
						
							break;

						case thirdPersonStrafeDashDirection.backward:
							currentDashInfo.eventOnDashOnThirdPersonBackward.Invoke ();

							if (showDebugPrint) {
								print ("movement backward");
							}

							break;

						case thirdPersonStrafeDashDirection.right:
							currentDashInfo.eventOnDashOnThirdPersonRight.Invoke ();

							if (showDebugPrint) {
								print ("movement right");
							}

							break;

						case thirdPersonStrafeDashDirection.left:
							currentDashInfo.eventOnDashOnThirdPersonLeft.Invoke ();
							if (showDebugPrint) {
								print ("movement left");
							}

							break;

						case thirdPersonStrafeDashDirection.forwardRight:
							currentDashInfo.eventOnDashOnThirdPersonForwardRight.Invoke ();
			
							if (showDebugPrint) {
								print ("movement forwardRight");
							}

							break;

						case thirdPersonStrafeDashDirection.forwardLeft:
							currentDashInfo.eventOnDashOnThirdPersonForwardLeft.Invoke ();

							if (showDebugPrint) {
								print ("movement forwardLeft");
							}

							break;

						case thirdPersonStrafeDashDirection.backwardRight:
							currentDashInfo.eventOnDashOnThirdPersonBackwardRight.Invoke ();
					
							if (showDebugPrint) {
								print ("movement backwardRight");
							}

							break;

						case thirdPersonStrafeDashDirection.backwardLeft:
							currentDashInfo.eventOnDashOnThirdPersonBackwardLeft.Invoke ();
				
							if (showDebugPrint) {
								print ("movement backwardLeft");
							}

							break;
						}
					} else {
						currentDashInfo.eventOnDashOnThirdPerson.Invoke ();
					}

					if (currentDashInfo.useGeneralEventOnDashOnThirdPerson) {
						currentDashInfo.generalEventOnDashOnThirdPerson.Invoke ();
					}

					if (currentDashInfo.useStamina) {
						mainStaminaSystem.setCustomRefillStaminaDelayAfterUseValue (currentDashInfo.refillStaminaWaitTime);

						mainStaminaSystem.useStaminaAmountExternally (currentDashInfo.amountOfStaminaToUse);
					}
				}
			}
		}

		dashCameraActive = true;

		if (dashActivatedOnFirstPerson) {
			mainPlayerCamera.setCameraPositionMouseWheelEnabledState (false);
			mainPlayerCamera.enableOrDisableChangeCameraView (false);
		}
	}

	public void setOverrideStrafeModeActiveStateResult (bool state)
	{
		overrideStrafeModeActiveState = state;
	}

	public void disableDashState ()
	{
		dashActive = false;

		if (dashActivatedOnFirstPerson) {
			playerInput.overrideInputValues (Vector2.zero, false);

			mainPlayerController.inputStopToRun ();

			mainPlayerController.setNoAnimatorSpeedValue (originalNoAnimatorSpeedValue);

			mainPlayerCamera.setCameraPositionMouseWheelEnabledState (true);
			mainPlayerCamera.setOriginalchangeCameraViewEnabledValue ();
		}
	}

	public void setDashEnabledState (bool state)
	{
		dashEnabled = state;
	}

	public void inputActivateDashState ()
	{
		if (enabled && dashEnabled && canUseDash ()) {
			enableDashState ();
		}
	}

	public void activateDashStateWithoutGroundCheck ()
	{
		checkGroundPaused = true;

		inputActivateDashState ();

		checkGroundPaused = false;
	}

	public void setCheckGroundPausedState (bool state)
	{
		checkGroundPaused = state;
	}

	public void activateDashStateWithCustomDirection (Vector3 dashDirection)
	{
		if (enabled && dashEnabled && canUseDash ()) {
			customDashDirectionActive = true;
			customDashDirection = dashDirection;

			enableDashState ();
		}
	}

	public bool canUseDash ()
	{
		if (mainPlayerController.canPlayerMove () ||
		    (canUseDashWhileActionActive && mainPlayerController.isActionActive () && checkIfActionToUseOnDashIsOnCategoryList ())) {

			if (mainPlayerController.isPlayerOnFirstPerson ()) {
				if (useDashOnFirstPerson) {
					if (!dashActive && (!useDashCoolDown || Time.time > dashCoolDownTime + lastTimeDashActive)) {
						if (mainPlayerController.isPlayerOnGround () || !onlyDashOnGround || checkGroundPaused) {
							return true;
						}
					}
				}
			} else {
				if (useDashOnThirdPerson) {
					if (dashOnThirdPersonPaused) {
						return false;
					}

					if (!dashActive && (!useDashCoolDown || Time.time > dashCoolDownTime + lastTimeDashActive)) {
						if (mainPlayerController.isPlayerOnGround () || checkGroundPaused) {
							return true;
						}
					}
				}
			}
		}

		return false;
	}

	public void setUseDashOnFirstPersonState (bool state)
	{
		useDashOnFirstPerson = state;
	}

	public void setUseDashOnThirdPersonState (bool state)
	{
		useDashOnThirdPerson = state;
	}

	public void setDashOnThirdPersonInTransitionState (bool state)
	{
		dashOnThirdPersonInTransition = state;

		if (!dashOnThirdPersonInTransition) {
			disableDashState ();
		}
	}

	public bool checkIfActionToUseOnDashIsOnCategoryList ()
	{
		if (actionCategoryToUseDash.Contains (mainPlayerController.getCurrentActionCategoryActive ())) {
			return true;
		}

		return false;
	}

	public void setDashOnThirdPersonPausedState (bool state)
	{
		dashOnThirdPersonPaused = state;
	}

	public void setDashInfoOnThirdPersonPausedState (bool state, string dashInfoName)
	{
		for (int i = 0; i < dashInfoList.Count; i++) {
			if (dashInfoList [i].Name.Equals (dashInfoName)) {
				dashInfoList [i].dashOnThirdPersonPaused = state;

				return;
			}
		}
	}

	public void pauseDashInfoOnThirdPerson (string dashInfoName)
	{
		setDashInfoOnThirdPersonPausedState (true, dashInfoName);
	}

	public void resumeDashInfoOnThirdPerson (string dashInfoName)
	{
		setDashInfoOnThirdPersonPausedState (false, dashInfoName);
	}

	public void setDashInfoOnThirdPersonPausedByStaminaState (bool state, string dashInfoName)
	{
		for (int i = 0; i < dashInfoList.Count; i++) {
			if (dashInfoList [i].useStamina && dashInfoList [i].Name.Equals (dashInfoName)) {
				dashInfoList [i].dashOnThirdPersonPaused = state;

				return;
			}
		}
	}

	public void pauseByStaminaDashInfoOnThirdPerson (string dashInfoName)
	{
		setDashInfoOnThirdPersonPausedByStaminaState (true, dashInfoName);
	}

	public void resumeByStaminaDashInfoOnThirdPerson (string dashInfoName)
	{
		setDashInfoOnThirdPersonPausedByStaminaState (false, dashInfoName);
	}

	public int getCurrentDashID ()
	{
		return currentDashID;
	}

	public void setCurrentDashID (int newValue)
	{
		currentDashID = newValue;

		if (showDebugPrint) {
			print ("Setting new dash id " + currentDashID);
		}

		for (int i = 0; i < dashInfoList.Count; i++) {
			if (dashInfoList [i].dashID == currentDashID) {
				currentDashInfo = dashInfoList [i];

				dashInfoList [i].isCurrentState = true;

				if (showDebugPrint) {
					print ("Setting new dash " + dashInfoList [i].Name);
				}
			} else {
				dashInfoList [i].isCurrentState = false;
			}
		}
	}


	[System.Serializable]
	public class dashInfo
	{
		public string Name;

		public int dashID;

		public bool isCurrentState;

		[Space]
		[Header ("Regular Dash Events Settings")]
		[Space]

		public UnityEvent eventOnDashOnThirdPerson;
		public float dashDurationOnThirdPerson;

		public bool useDoubleTapToDashOnThirdPersonEnabled;

		[Space]
		[Header ("Strafe Dash Events Settings")]
		[Space]

		public bool checkDashInStrafeState = true;
		public UnityEvent eventOnDashOnThirdPersonForward;
		public UnityEvent eventOnDashOnThirdPersonBackward;
		public UnityEvent eventOnDashOnThirdPersonLeft;
		public UnityEvent eventOnDashOnThirdPersonRight;

		[Space]
		[Header ("Diagonal Dash Events Settings")]
		[Space]

		public bool checkDiagonalInputInStrafeState = true;
		public UnityEvent eventOnDashOnThirdPersonForwardRight;
		public UnityEvent eventOnDashOnThirdPersonForwardLeft;
		public UnityEvent eventOnDashOnThirdPersonBackwardRight;
		public UnityEvent eventOnDashOnThirdPersonBackwardLeft;

		[Space]
		[Header ("General Events Settings")]
		[Space]

		public bool useGeneralEventOnDashOnThirdPerson;
		public UnityEvent generalEventOnDashOnThirdPerson;

		[Space]
		[Header ("Stamina Settings")]
		[Space]

		public bool useStamina;
		public float amountOfStaminaToUse;
		public float refillStaminaWaitTime;

		[Space]
		[Header ("Debug")]
		[Space]

		public bool dashOnThirdPersonPaused;
	}
}
