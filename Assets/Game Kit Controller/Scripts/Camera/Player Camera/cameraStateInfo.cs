using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class cameraStateInfo
{
	public string Name;
	public Vector3 camPositionOffset;
	public Vector3 pivotPositionOffset;
	public Vector3 pivotParentPositionOffset;
	public Vector2 yLimits;

	public float initialFovValue;
	public float fovTransitionSpeed = 10;

	public float maxFovValue;
	public float minFovValue = 17;

	public bool showGizmo;
	public Color gizmoColor;

	public Vector3 originalCamPositionOffset;

	public bool leanEnabled;
	public float maxLeanAngle;
	public float leanSpeed;
	public float leanResetSpeed;
	public float leanRaycastDistance;
	public float leanAngleOnSurfaceFound;
	public float leanHeight;

	public bool headTrackActive = true;

	public bool lookInPlayerDirection;
	public float lookInPlayerDirectionSpeed;
	public bool allowRotationWithInput;
	public float timeToResetRotationAfterInput;

	public bool useYLimitsOnLookAtTargetActive;
	public Vector2 YLimitsOnLookAtTargetActive = new Vector2 (-20, 30);

	public bool cameraCollisionActive = true;

	public float pivotRotationOffset;

	public bool ignoreCameraShakeOnRunState;

	public cameraStateInfo (cameraStateInfo newState)
	{
		Name = newState.Name;

		camPositionOffset = newState.camPositionOffset;
		pivotPositionOffset = newState.pivotPositionOffset;
		pivotParentPositionOffset = newState.pivotParentPositionOffset;

		yLimits = newState.yLimits;       

		initialFovValue = newState.initialFovValue;
		fovTransitionSpeed = newState.fovTransitionSpeed;
		maxFovValue = newState.maxFovValue;
		minFovValue = newState.minFovValue;

		originalCamPositionOffset = newState.originalCamPositionOffset;

		leanEnabled = newState.leanEnabled;
		maxLeanAngle = newState.maxLeanAngle;
		leanSpeed = newState.leanSpeed;
		leanResetSpeed = newState.leanResetSpeed;
		leanRaycastDistance = newState.leanRaycastDistance;
		leanAngleOnSurfaceFound = newState.leanAngleOnSurfaceFound;
		leanHeight = newState.leanHeight;

		headTrackActive = newState.headTrackActive;

		lookInPlayerDirection = newState.lookInPlayerDirection;
		lookInPlayerDirectionSpeed = newState.lookInPlayerDirectionSpeed;
		allowRotationWithInput = newState.allowRotationWithInput;
		timeToResetRotationAfterInput = newState.timeToResetRotationAfterInput;

		useYLimitsOnLookAtTargetActive = newState.useYLimitsOnLookAtTargetActive;
		YLimitsOnLookAtTargetActive = newState.YLimitsOnLookAtTargetActive;

		cameraCollisionActive = newState.cameraCollisionActive;

		pivotRotationOffset = newState.pivotRotationOffset;

		ignoreCameraShakeOnRunState = newState.ignoreCameraShakeOnRunState;
	}

	public void assignCameraStateValues (cameraStateInfo from, float lerpSpeed)
	{
		Name = from.Name;

		if (lerpSpeed > 0) {
			if (camPositionOffset != from.camPositionOffset) {
				camPositionOffset = Vector3.Lerp (camPositionOffset, from.camPositionOffset, lerpSpeed);  
			}

			if (pivotPositionOffset != from.pivotPositionOffset) {
				pivotPositionOffset = Vector3.Lerp (pivotPositionOffset, from.pivotPositionOffset, lerpSpeed);
			}

			if (pivotParentPositionOffset != from.pivotParentPositionOffset) {
				pivotParentPositionOffset = Vector3.Lerp (pivotParentPositionOffset, from.pivotParentPositionOffset, lerpSpeed);
			}

			if (yLimits.x != from.yLimits.x) {
				yLimits.x = Mathf.Lerp (yLimits.x, from.yLimits.x, lerpSpeed);
			}

			if (yLimits.y != from.yLimits.y) {
				yLimits.y = Mathf.Lerp (yLimits.y, from.yLimits.y, lerpSpeed);
			}
		} else {
			camPositionOffset = from.camPositionOffset;  
			pivotPositionOffset = from.pivotPositionOffset;
			pivotParentPositionOffset = from.pivotParentPositionOffset;

			yLimits = from.yLimits;
		}

		initialFovValue = from.initialFovValue;
		fovTransitionSpeed = from.fovTransitionSpeed;
		maxFovValue = from.maxFovValue;
		minFovValue = from.minFovValue;

		originalCamPositionOffset = from.originalCamPositionOffset;

		leanEnabled = from.leanEnabled;
		maxLeanAngle = from.maxLeanAngle;
		leanSpeed = from.leanSpeed;
		leanResetSpeed = from.leanResetSpeed;
		leanRaycastDistance = from.leanRaycastDistance;
		leanAngleOnSurfaceFound = from.leanAngleOnSurfaceFound;
		leanHeight = from.leanHeight;

		headTrackActive = from.headTrackActive;

		lookInPlayerDirection = from.lookInPlayerDirection;
		lookInPlayerDirectionSpeed = from.lookInPlayerDirectionSpeed;
		allowRotationWithInput = from.allowRotationWithInput;
		timeToResetRotationAfterInput = from.timeToResetRotationAfterInput;

		useYLimitsOnLookAtTargetActive = from.useYLimitsOnLookAtTargetActive;
		YLimitsOnLookAtTargetActive = from.YLimitsOnLookAtTargetActive;

		cameraCollisionActive = from.cameraCollisionActive;

		pivotRotationOffset = from.pivotRotationOffset;

		ignoreCameraShakeOnRunState = from.ignoreCameraShakeOnRunState;
	}
}