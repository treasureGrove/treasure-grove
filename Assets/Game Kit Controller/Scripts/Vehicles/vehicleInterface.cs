using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;

public class vehicleInterface : MonoBehaviour
{
	public bool interfaceCanBeEnabled = true;

	public bool interfaceEnabled;

	public GameObject vehicle;

	public GameObject interfaceCanvas;

	public List<interfaceElement> interfaceElementList = new List<interfaceElement> ();

	public bool useInterfacePanelInfoList;

	public vehicleHUDManager HUDManager;
	public RectTransform interfacePanelParent;

	public float movePanelSpeed = 2;
	public float rotatePanelSpeed = 2;

	public List<interfacePanelInfo> interfacePanelInfoList = new List<interfacePanelInfo> ();

	bool startInitialized;

	playerInputManager playerInput;

	Vector2 axisValues;

	RectTransform currentInterfacePanelSelected;
	bool interfacePanelSelected;
	bool movingPanel;
	bool rotatingPanel;

	void Start ()
	{
		if (!interfaceCanBeEnabled) {
			interfaceCanvas.SetActive (false);
		}

		for (int i = 0; i < interfaceElementList.Count; i++) {
			GameObject uiElement = interfaceElementList [i].uiElement;

			bool uiElementTypeFound = false;
			Toggle currenToggle = uiElement.GetComponent<Toggle> ();
			if (currenToggle) {
				currenToggle.isOn = interfaceElementList [i].currentBoolValue;
				uiElementTypeFound = true;
			}

			if (!uiElementTypeFound) {
				Scrollbar currenScroll = uiElement.GetComponent<Scrollbar> ();
				if (currenScroll) {
					if (interfaceElementList [i].containsRange) {
						currenScroll.value = interfaceElementList [i].currentAmountValue / interfaceElementList [i].range.y;
					} else {
						currenScroll.value = interfaceElementList [i].currentAmountValue;
					}
					uiElementTypeFound = true;
				}
			}

			if (!uiElementTypeFound) {
				Slider currenSlider = uiElement.GetComponent<Slider> ();
				if (currenSlider) {
					if (interfaceElementList [i].containsRange) {
						currenSlider.value = interfaceElementList [i].currentAmountValue / interfaceElementList [i].range.y;
					} else {
						currenSlider.value = interfaceElementList [i].currentAmountValue;
					}
				}
			}

			if (interfaceElementList [i].containsBool) {
				interfaceElementList [i].originalBoolValue = interfaceElementList [i].currentBoolValue;
			}
		}

		enableOrDisableInterface (false);

		if (useInterfacePanelInfoList) {
			for (int i = 0; i < interfacePanelInfoList.Count; i++) {
				interfacePanelInfoList [i].originalPosition = interfacePanelInfoList [i].uiRectTransform.localPosition;
				interfacePanelInfoList [i].originalRotation = interfacePanelInfoList [i].uiRectTransform.localRotation;
			}
		}
	}

	void Update ()
	{
		if (interfaceEnabled) {
			if (useInterfacePanelInfoList) {
				if (interfacePanelSelected) {
					axisValues = playerInput.getPlayerMouseAxis ();

					if (movingPanel) {
						interfacePanelParent.localPosition += (Vector3.right * axisValues.x + Vector3.up * axisValues.y) * movePanelSpeed;
					}

					if (rotatingPanel) {
						interfacePanelParent.localEulerAngles += new Vector3 (axisValues.y, -axisValues.x, 0) * rotatePanelSpeed;
					}
				}
			}
		}

		if (!startInitialized) {
			startInitialized = true;
		}
	}

	public void setInterfaceCanBeEnabledState (bool state)
	{
		interfaceCanBeEnabled = state;
	}

	public void enableOrDisableInterface (bool state)
	{
		if (!interfaceCanBeEnabled) {
			return;
		}

		interfaceEnabled = state;
		interfaceCanvas.SetActive (interfaceEnabled);

		if (!interfaceEnabled) {
			for (int i = 0; i < interfaceElementList.Count; i++) {
				if (interfaceElementList [i].disableWhenVehicleOff) {
					GameObject uiElement = interfaceElementList [i].uiElement;
					if (interfaceElementList [i].containsBool) {
						bool boolValue = interfaceElementList [i].originalBoolValue;

						bool uiElementTypeFound = false;
						Toggle currenToggle = uiElement.GetComponent<Toggle> ();
						if (currenToggle) {
							currenToggle.isOn = boolValue;
							uiElementTypeFound = true;
						}

						if (!uiElementTypeFound) {
							Scrollbar currenScroll = uiElement.GetComponent<Scrollbar> ();
							if (currenScroll) {
								if (boolValue) {
									currenScroll.value = 0;
								} else {
									currenScroll.value = 1;
								}
								uiElementTypeFound = true;
							}
						}

						if (!uiElementTypeFound) {
							Slider currenSlider = uiElement.GetComponent<Slider> ();
							if (currenSlider) {
								if (boolValue) {
									currenSlider.value = 0;
								} else {
									currenSlider.value = 1;
								}
								uiElementTypeFound = true;
							}
						}

						interfaceElementList [i].currentBoolValue = boolValue;

						if (interfaceElementList [i].setValueOnText) {
							if (interfaceElementList [i].useCustomValueOnText) {
								if (boolValue) {
									interfaceElementList [i].valuetText.text = interfaceElementList [i].boolActiveCustomText;
								} else {
									interfaceElementList [i].valuetText.text = interfaceElementList [i].boolNoActiveCustomText;
								}
							} else {
								interfaceElementList [i].valuetText.text = boolValue.ToString ();
							}
						}

						if (interfaceElementList [i].eventToCallBool.GetPersistentEventCount () > 0) {
							interfaceElementList [i].eventToCallBool.Invoke (boolValue);
						}
					}
				}
			}
		}
	}

	public void checkPressedUIElememt (GameObject uiElement)
	{
		if (!startInitialized || !interfaceCanBeEnabled) {
			return;
		}

		print (uiElement.name);

		for (int i = 0; i < interfaceElementList.Count; i++) {
			if (uiElement == interfaceElementList [i].uiElement) {
				if (interfaceElementList [i].eventSendValues) {
					if (interfaceElementList [i].containsAmount) {
						float amountToSend = 0;

						bool uiElementTypeFound = false;

						Scrollbar currenScroll = uiElement.GetComponent<Scrollbar> ();
						if (currenScroll) {
							if (interfaceElementList [i].containsRange) {
								amountToSend = interfaceElementList [i].range.y * currenScroll.value;
							} else {
								amountToSend = currenScroll.value;
							}
							uiElementTypeFound = true;
						}

						if (!uiElementTypeFound) {
							Slider currenSlider = uiElement.GetComponent<Slider> ();
							if (currenSlider) {
								if (interfaceElementList [i].containsRange) {
									amountToSend = interfaceElementList [i].range.y * currenSlider.value;
								} else {
									amountToSend = currenSlider.value;
								}
								uiElementTypeFound = true;
							}
						}

						interfaceElementList [i].currentAmountValue = amountToSend;

						if (interfaceElementList [i].setValueOnText) {
							interfaceElementList [i].valuetText.text = amountToSend.ToString ("F0");
						}

						if (interfaceElementList [i].eventToCallAmount.GetPersistentEventCount () > 0) {
							interfaceElementList [i].eventToCallAmount.Invoke (amountToSend);
						}
					} 

					if (interfaceElementList [i].containsBool) {
						bool boolValue = false;

						bool uiElementTypeFound = false;
						Toggle currenToggle = uiElement.GetComponent<Toggle> ();
						if (currenToggle) {
							boolValue = currenToggle.isOn;
							uiElementTypeFound = true;
						}

						if (!uiElementTypeFound) {
							Button currenButton = uiElement.GetComponent<Button> ();
							if (currenButton) {
								boolValue = !interfaceElementList [i].currentBoolValue;
								uiElementTypeFound = true;
							}
						}

						if (!uiElementTypeFound) {
							Scrollbar currenScroll = uiElement.GetComponent<Scrollbar> ();
							if (currenScroll) {
								if (currenScroll.value == 0) {
									boolValue = false;
								} else {
									boolValue = true;
								}
								uiElementTypeFound = true;
							}
						}

						if (!uiElementTypeFound) {
							Slider currenSlider = uiElement.GetComponent<Slider> ();
							if (currenSlider) {
								if (currenSlider.value == 0) {
									boolValue = false;
								} else {
									boolValue = true;
								}
								uiElementTypeFound = true;
							}
						}

						if (!uiElementTypeFound) {
							RawImage currentRawImage = uiElement.GetComponent<RawImage> ();
							if (currentRawImage) {
								boolValue = !interfaceElementList [i].currentBoolValue;
								uiElementTypeFound = true;
							}
						}

						if (!uiElementTypeFound) {
							Image currentImage = uiElement.GetComponent<Image> ();
							if (currentImage) {
								boolValue = !interfaceElementList [i].currentBoolValue;
								uiElementTypeFound = true;
							}
						}
							
						interfaceElementList [i].currentBoolValue = boolValue;

						if (interfaceElementList [i].setValueOnText) {
							if (interfaceElementList [i].useCustomValueOnText) {
								if (boolValue) {
									interfaceElementList [i].valuetText.text = interfaceElementList [i].boolActiveCustomText;
								} else {
									interfaceElementList [i].valuetText.text = interfaceElementList [i].boolNoActiveCustomText;
								}
							} else {
								interfaceElementList [i].valuetText.text = boolValue.ToString ();
							}
						}

						if (interfaceElementList [i].eventToCallBool.GetPersistentEventCount () > 0) {
							interfaceElementList [i].eventToCallBool.Invoke (boolValue);
						}
					}
				} else {
					if (interfaceElementList [i].eventToCall.GetPersistentEventCount () > 0) {
						interfaceElementList [i].eventToCall.Invoke ();
					}
				}
			}
		}
	}

	public void setMoveInterfacePanelPressed (RectTransform panelToCheck)
	{
		checkInterfacePanelInfoPressed (panelToCheck);
		movingPanel = !movingPanel;
	}

	public void setRotateInterfacePanelPressed (RectTransform panelToCheck)
	{
		checkInterfacePanelInfoPressed (panelToCheck);
		rotatingPanel = !rotatingPanel;
	}

	public void checkInterfacePanelInfoPressed (RectTransform panelToCheck)
	{
		if (!useInterfacePanelInfoList) {
			return;
		}

		for (int i = 0; i < interfacePanelInfoList.Count; i++) {
			if (interfacePanelInfoList [i].uiRectTransform == panelToCheck) {
				interfacePanelSelected = !interfacePanelSelected;
				if (interfacePanelSelected) {
					currentInterfacePanelSelected = panelToCheck;
					playerInput = HUDManager.getCurrentDriver ().GetComponent<playerInputManager> ();

					interfacePanelParent.position = currentInterfacePanelSelected.position;
					interfacePanelParent.rotation = currentInterfacePanelSelected.rotation;
					currentInterfacePanelSelected.SetParent (interfacePanelParent);
				} else {
					currentInterfacePanelSelected.SetParent (interfacePanelInfoList [i].panelParent);
					currentInterfacePanelSelected = null;
				}
			}
		}
	}

	public void resetInterfacePanelInfoPressed (RectTransform panelToCheck)
	{
		if (!useInterfacePanelInfoList) {
			return;
		}

		for (int i = 0; i < interfacePanelInfoList.Count; i++) {
			if (interfacePanelInfoList [i].uiRectTransform == panelToCheck) {
				interfacePanelInfoList [i].uiRectTransform.localPosition = interfacePanelInfoList [i].originalPosition;
				interfacePanelInfoList [i].uiRectTransform.localRotation = interfacePanelInfoList [i].originalRotation;
			}
		}
	}

	public void setInterfaceCanvasState (bool state)
	{
		interfaceCanvas.SetActive (state);
	}

	public void addInterfaceElement ()
	{
		interfaceElement newInterfaceElement = new interfaceElement ();
		newInterfaceElement.Name = "New Element";
		interfaceElementList.Add (newInterfaceElement);
		updateComponent ();
	}

	public void updateComponent ()
	{
		GKC_Utils.updateComponent (this);
	}

	[System.Serializable]
	public class interfaceElement
	{
		public string Name;
		public GameObject uiElement;

		public UnityEvent eventToCall;

		public bool eventSendValues;

		public bool containsAmount;
		public bool containsRange;
		public Vector2 range;

		public float currentAmountValue;

		public bool containsBool;

		public bool disableWhenVehicleOff;

		public bool originalBoolValue;
		public bool currentBoolValue;

		public bool setValueOnText;
		public Text valuetText;

		public bool useCustomValueOnText;

		public string boolActiveCustomText;
		public string boolNoActiveCustomText;

		[SerializeField] public eventParameters.eventToCallWithAmount eventToCallAmount;

		[SerializeField] public eventParameters.eventToCallWithBool eventToCallBool;
	}

	[System.Serializable]
	public class interfacePanelInfo
	{
		public string Name;

		public RectTransform uiRectTransform;
		public RectTransform panelParent;

		public Vector3 originalPosition;
		public Quaternion originalRotation;
	}
}