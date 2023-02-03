using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class showMessageOnHUDSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public List<messageInfo> messageInfoList = new List<messageInfo> ();

	[Space]
	[Header ("Other Settings")]
	[Space]

	public string actionNameField = "-ACTION NAME-";

	public string panelForCustomMessageName = "Custom Message";

	[Space]
	[Header ("Components")]
	[Space]

	public playerInputManager playerInput;

	string currentCustomMessageContent;

	bool usingCustomMessage;

	string currentPanelNameManually;


	public void setMessagePanelNameManually (string messageName)
	{
		currentPanelNameManually = messageName;
	}

	public void showMessagePanelManually (string messageContent)
	{
		if (messageContent == "") {
			return;
		}

		for (int i = 0; i < messageInfoList.Count; i++) {
			if (messageInfoList [i].Name.Equals (currentPanelNameManually)) {

				if (!messageInfoList [i].showingMessage || !messageInfoList [i].dontActivateMessageIfShowing) {
					currentCustomMessageContent = messageContent;

					usingCustomMessage = true;

					showObjectMessage (i);

					return;
				}
			}
		}
	}

	public void showCustomMessagePanel (string messageContent)
	{
		if (messageContent == "") {
			return;
		}

		for (int i = 0; i < messageInfoList.Count; i++) {
			if (messageInfoList [i].Name.Equals (panelForCustomMessageName)) {

				if (!messageInfoList [i].showingMessage || !messageInfoList [i].dontActivateMessageIfShowing) {
					currentCustomMessageContent = messageContent;

					usingCustomMessage = true;

					showObjectMessage (i);

					return;
				}
			}
		}
	}


	public void showMessagePanel (string messageName)
	{
		for (int i = 0; i < messageInfoList.Count; i++) {
			if (messageInfoList [i].Name.Equals (messageName)) {

				if (!messageInfoList [i].showingMessage || !messageInfoList [i].dontActivateMessageIfShowing) {
					showObjectMessage (i);

					return;
				}
			}
		}
	}

	public void hideMessagePanel (string messageName)
	{
		for (int i = 0; i < messageInfoList.Count; i++) {
			if (messageInfoList [i].Name.Equals (messageName)) {

				if (messageInfoList [i].messageCoroutine != null) {
					StopCoroutine (messageInfoList [i].messageCoroutine);
				}

				if (messageInfoList [i].messagePanel.activeSelf) {
					messageInfoList [i].messagePanel.SetActive (false);
				}

				messageInfoList [i].showingMessage = false;

				return;
			}
		}
	}

	public void showObjectMessage (int messageIndex)
	{
		if (messageInfoList [messageIndex].messageCoroutine != null) {
			StopCoroutine (messageInfoList [messageIndex].messageCoroutine);
		}

		messageInfoList [messageIndex].messageCoroutine = StartCoroutine (showObjectMessageCoroutine (messageIndex));
	}

	IEnumerator showObjectMessageCoroutine (int messageIndex)
	{
		messageInfo currentMessageInfo = messageInfoList [messageIndex];

		string newText = currentMessageInfo.messageContent;

		if (usingCustomMessage) {
			newText = currentCustomMessageContent;

			usingCustomMessage = false;
		}

		if (currentMessageInfo.checkForInputActionOnText && playerInput != null) {
			if (newText.Contains (actionNameField)) {
				string keyAction = playerInput.getButtonKey (currentMessageInfo.includedActionNameOnText);

				newText = newText.Replace (actionNameField, keyAction);
			}
		}

		currentMessageInfo.messageText.text = newText;

		currentMessageInfo.showingMessage = true;
		currentMessageInfo.eventOnMessage.Invoke ();

		currentMessageInfo.messagePanel.SetActive (true);

		yield return new WaitForSeconds (currentMessageInfo.messageDuration);

		if (currentMessageInfo.useMessageDuration) {
			currentMessageInfo.messagePanel.SetActive (false);

			currentMessageInfo.showingMessage = false;
		}
	}

	[System.Serializable]
	public class messageInfo
	{
		[Header ("Main Settings")]
		[Space]

		public string Name;

		[TextArea (10, 11)] public string messageContent;

		[Space]

		public bool useMessageDuration = true;
		public float messageDuration;

		[Space]
		[Header ("UI Settings")]
		[Space]

		public GameObject messagePanel;

		public Text messageText;

		[Space]
		[Header ("Other Settings")]
		[Space]

		public bool dontActivateMessageIfShowing;

		public bool checkForInputActionOnText;

		public string includedActionNameOnText;

		[Space]
		[Header ("Debug")]
		[Space]

		public bool showingMessage;

		[Space]
		[Header ("Events Settings")]
		[Space]

		public UnityEvent eventOnMessage;

		public Coroutine messageCoroutine;
	}
}