using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class attachmentInfo
{
	public string Name;
	public bool attachmentEnabled = true;
	public bool attachmentActive;
	public bool currentlyActive;

	public bool attachmentUseHUD;

	public bool onlyEnabledWhileCarrying;
	public GameObject attachmentGameObject;
	public UnityEvent activateEvent;
	public UnityEvent deactivateEvent;
	public attachmentSlot attachmentSlotManager;
	public AudioClip selectAttachmentSound;

	public bool useEventOnPress;
	public UnityEvent eventOnPress;
	public bool useEventOnPressDown;
	public UnityEvent eventOnPressDown;
	public bool useEventOnPressUp;
	public UnityEvent eventOnPressUp;

	public bool useEventHandPosition;
	public UnityEvent activateEventHandPosition;
	public UnityEvent deactivateEventHandPosition;

	public bool useAttachmentHoverInfo;
	[TextArea (3, 10)] public string attachmentHoverInfo;
}

[System.Serializable]
public class attachmentIcon
{
	public RectTransform iconRectTransform;
	public Text attachmentNumberText;
	public Text attachmentNameText;
	public Transform attachmentContent;
	public attachmentSlot notAttachmentButton;

	public Transform attachmentPointTransform;
	public Transform attachmentLineTransform;
}

[System.Serializable]
public class attachmentSlot
{
	public Button slotButton;
	public Text attachmentNameText;
	public GameObject attachmentSelectedIcon;

	public RectTransform attachmentHoverInfoPanelPosition;
}
