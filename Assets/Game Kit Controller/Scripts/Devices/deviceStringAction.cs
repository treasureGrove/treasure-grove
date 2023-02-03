using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class deviceStringAction : MonoBehaviour
{
	public string deviceName;
	public string deviceAction;

	public bool useMultipleInteraction;
	public List<string> multipleInteractionNameList = new List<string> ();

	public string secondaryDeviceAction;
	public bool reloadDeviceActionOnPress;
	public bool hideIconOnPress;
	public bool disableIconOnPress;
	public bool showIcon;
	public bool showTouchIconButton;

	public bool hideIconOnUseDevice;
	public bool showIconOnStopUseDevice;

	public bool useTransformForStringAction;
	public Transform transformForStringAction;

	public bool useSeparatedTransformForEveryView;
	public Transform transformForThirdPerson;
	public Transform transformForFirstPerson;

	public bool useLocalOffset = true;
	public float actionOffset = 1;

	public bool setUsingDeviceState;

	public bool setTextFontSizeActive;
	public int textFontSize = 17;

	public bool iconEnabled = true;

	public bool useRaycastToDetectDeviceParts;

	bool showingSecondaryAction;

	string currentDeviceAction;

	public bool usingDevice;

	public bool useCustomMinDistanceToUseDevice;
	public float customMinDistanceToUseDevice;

	public bool useCustomMinAngleToUse;
	public float customMinAngleToUseDevice;
	public bool useRelativeDirectionBetweenPlayerAndObject;

	public bool useCustomIconButtonInfo;
	public string customIconButtonInfoName;

	public bool ignoreUseOnlyDeviceIfVisibleOnCamera = true;

	public bool useCustomDeviceTransformPosition;
	public Transform customDeviceTransformPosition;

	public bool useFixedDeviceIconPosition;

	public bool checkIfObstacleBetweenDeviceAndPlayer;

	public bool showIconTexture;
	public Texture iconTexture;

	public bool useObjectDescription;
	[TextArea (5, 20)]public string objectDescription;
	public int descriptionFontSize = 14;

	public bool showObjectAmount;
	public int objectAmount;

	public bool useHoldInteractionButton;
	public float holdInteractionButtonDuration;

	//GIZMO ELEMENTS
	public bool showGizmo;
	public Color gizmoLabelColor = Color.green;
	public Color gizmoColor = Color.white;
	public float gizmoRadius = 0.3f;

	//just a string to set the action made by the device which has this script
	//the option disableIconOnPress allows to remove the icon of the action once it is done
	//the option showIcon allows to show the icon or not when the player is inside the device trigger
	//the option showTouchIconButton allows to show the touch button to use devices

	void Start ()
	{
		currentDeviceAction = deviceAction;
	}


	public Transform getRegularTransformForIcon ()
	{
		if (useTransformForStringAction && transformForStringAction != null) {
			return transformForStringAction;
		} else {
			return transform;
		}
	}

	public bool useSeparatedTransformForEveryViewEnabled ()
	{
		return useTransformForStringAction && useSeparatedTransformForEveryView;
	}

	public Transform getTransformForIconThirdPerson ()
	{
		return transformForThirdPerson;
	}

	public Transform getTransformForIconFirstPerson ()
	{
		return transformForFirstPerson;
	}

	public void setIconEnabledState (bool state)
	{
		iconEnabled = state;
	}

	public void enableIcon ()
	{
		setIconEnabledState (true);
	}

	public void disableIcon ()
	{
		setIconEnabledState (false);
	}

	public string getDeviceAction ()
	{
		return currentDeviceAction;
	}

	public void setDeviceAction (string newDeviceAction)
	{
		deviceAction = newDeviceAction;

		currentDeviceAction = deviceAction;
	}

	public void changeActionName (bool state)
	{
		showingSecondaryAction = state;

		if (showingSecondaryAction && secondaryDeviceAction.Length > 0) {
			currentDeviceAction = secondaryDeviceAction;
		} else {
			currentDeviceAction = deviceAction;
		}
	}

	public void checkSetUsingDeviceState ()
	{
		checkSetUsingDeviceState (!usingDevice);
	}

	public void checkSetUsingDeviceState (bool state)
	{
		if (setUsingDeviceState) {
			usingDevice = state;
		}
	}

	public string getDeviceName ()
	{
		return deviceName;
	}

	public void setNewDeviceName (string newName)
	{
		deviceName = newName;
	}

	public int getTextFontSize ()
	{
		if (setTextFontSizeActive) {
			return textFontSize;
		}

		return 0;
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
			Gizmos.color = gizmoColor;
			Vector3 gizmoPosition = transform.position + transform.up * actionOffset;
			if (useTransformForStringAction) {
				if (useSeparatedTransformForEveryView) {
					if (transformForThirdPerson) {
						Gizmos.color = Color.white;
						gizmoPosition = transformForThirdPerson.position;
						Gizmos.DrawSphere (gizmoPosition, gizmoRadius);
					}
					if (transformForFirstPerson) {
						Gizmos.color = Color.yellow;
						gizmoPosition = transformForFirstPerson.position;
						Gizmos.DrawSphere (gizmoPosition, gizmoRadius);
					}
				} else {
					if (transformForStringAction) {
						gizmoPosition = transformForStringAction.position;
						Gizmos.DrawSphere (gizmoPosition, gizmoRadius);
					}
				}
			} else {
				Gizmos.DrawSphere (gizmoPosition, gizmoRadius);
			}
		}
	}
}