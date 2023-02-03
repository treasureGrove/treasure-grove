using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class playerAbilitiesUIEditorSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public float abilitiesWheelScale;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool abilitiesUIEditorActive;

	public bool movingSlotFromFullAbilitiesPanel;
	public bool movingSlotFromCurrentAbilitiesPanel;


	[Space]
	[Header ("UI Elements")]
	[Space]

	public GameObject abilitiesUIEditorObject;

	public ScrollRect fullAbilitiesUIEditorListScrollRect;

	public Scrollbar fullAbilitiesUIEditorScrollbar;

	public Transform fullAbilitiesUIEditorContent;

	public Transform currentWheelAbilitiesPanelBackgroundTransform;

	public Transform currentGridAbilitiesPanelBackgroundTransform;

	public Transform fullAbilitiesPanelTransform;

	public Transform mainAbilitiesWheelTransform;

	public Transform originalAbilitiesWheelParent;

	public Transform abilitiesWheelPosition;

	public GameObject abilityListElement;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool showDebugPrint;

	public List<playerAbilitiesUISystem.abilitySlotInfo> abilitySlotInfoList = new List<playerAbilitiesUISystem.abilitySlotInfo> ();
	public List<playerAbilitiesUISystem.abilitySlotInfo> temporalAbilitySlotInfoList = new List<playerAbilitiesUISystem.abilitySlotInfo> ();

	[Space]
	[Header ("Components")]
	[Space]

	public playerAbilitiesUISystem mainPlayerAbilitiesUISystem;

	public playerAbilitiesSystem mainPlayerAbilitiesSystem;


	bool fullAbilitiesUIEditorContentCreated;

	bool slotDroppedOnFullAbilitiesPanel;
	bool slotDroppedOnCurrentAbilitiesPanel;

	readonly List<RaycastResult> captureRaycastResults = new List<RaycastResult> ();

	bool touchPlatform;

	Touch currentTouch;

	Vector2 currentTouchPosition;

	bool touching;

	GameObject buttonToMove;

	RectTransform buttonToMoveRectTransform;

	playerAbilitiesUISystem.abilitySlotInfo currentSlotSelected;


	void Start ()
	{
		touchPlatform = touchJoystick.checkTouchPlatform ();
	}

	void Update ()
	{
		if (abilitiesUIEditorActive) {
			int touchCount = Input.touchCount;
			if (!touchPlatform) {
				touchCount++;
			}

			for (int i = 0; i < touchCount; i++) {
				if (!touchPlatform) {
					currentTouch = touchJoystick.convertMouseIntoFinger ();
				} else {
					currentTouch = Input.GetTouch (i);
				}

				currentTouchPosition = currentTouch.position;

				if (currentTouch.phase == TouchPhase.Began) {
					touching = true;

					//if the edit powers manager is open, then
					movingSlotFromFullAbilitiesPanel = false;

					movingSlotFromCurrentAbilitiesPanel = false;

					//check where the mouse or the finger press, to get a power list element, to edit the powers
					captureRaycastResults.Clear ();
					PointerEventData p = new PointerEventData (EventSystem.current);
					p.position = currentTouchPosition;
					p.clickCount = i;
					p.dragging = false;
					EventSystem.current.RaycastAll (p, captureRaycastResults);

					foreach (RaycastResult r in captureRaycastResults) {
	
						for (int k = 0; k < abilitySlotInfoList.Count; k++) {
							if (abilitySlotInfoList [k].slotActive) {

								if (abilitySlotInfoList [k].slot == r.gameObject) {

									if (showDebugPrint) {
										print ("comparing slot to object pressed " + r.gameObject.name + "  " + abilitySlotInfoList [k].Name);
									}

									
									buttonToMove = (GameObject)Instantiate (r.gameObject, r.gameObject.transform.position, Quaternion.identity);
									buttonToMove.transform.SetParent (abilitiesUIEditorObject.transform.parent);

									currentSlotSelected = abilitySlotInfoList [k];

									buttonToMoveRectTransform = buttonToMove.GetComponent<RectTransform> ();

									movingSlotFromFullAbilitiesPanel = true;

									movingSlotFromCurrentAbilitiesPanel = false;

									if (showDebugPrint) {
										print ("slot found from full panel");
									}

									return;
								}
							}
						}

						temporalAbilitySlotInfoList = mainPlayerAbilitiesUISystem.getAbilitySlotInfoList ();

						for (int k = 0; k < temporalAbilitySlotInfoList.Count; k++) {
							if (temporalAbilitySlotInfoList [k].slotActive) {

								if (showDebugPrint) {
									print ("comparing slot to object pressed " + r.gameObject.name + "  " + temporalAbilitySlotInfoList [k].Name);
								}

								if (temporalAbilitySlotInfoList [k].abilityIcon.gameObject == r.gameObject) {

									if (showDebugPrint) {
										print ("comparing slot to object pressed " + r.gameObject.name + "  " + temporalAbilitySlotInfoList [k].Name);
									}

									buttonToMove = (GameObject)Instantiate (temporalAbilitySlotInfoList [k].innerSlot.gameObject, r.gameObject.transform.position, Quaternion.identity);

									buttonToMove.transform.SetParent (abilitiesUIEditorObject.transform.parent);

									currentSlotSelected = temporalAbilitySlotInfoList [k];

									buttonToMoveRectTransform = buttonToMove.GetComponent<RectTransform> ();

									movingSlotFromFullAbilitiesPanel = false;

									movingSlotFromCurrentAbilitiesPanel = true;

									if (showDebugPrint) {
										print ("slot found from current panel");
									}

									return;
								}
							}
						}
					}
				}

				//if the power list element is grabbed, follow the mouse/finger position in screen
				if ((currentTouch.phase == TouchPhase.Stationary || currentTouch.phase == TouchPhase.Moved)) {

					if (buttonToMove != null) {
						buttonToMoveRectTransform.position = currentTouchPosition;
					}

				}

				//if the mouse/finger press is released, then
				if (currentTouch.phase == TouchPhase.Ended && touching) {
					touching = false;

					slotDroppedOnFullAbilitiesPanel = false;
					slotDroppedOnCurrentAbilitiesPanel = false;

					//if the player was editing the powers
				
					if (buttonToMove != null) {
						//get the elements in the position where the player released the power element
						captureRaycastResults.Clear ();
						PointerEventData p = new PointerEventData (EventSystem.current);
						p.position = currentTouchPosition;
						p.clickCount = i;
						p.dragging = false;
						EventSystem.current.RaycastAll (p, captureRaycastResults);

						string currentAbilityName = currentSlotSelected.Name;

						if (showDebugPrint) {
							print (currentAbilityName);
						}

						int abilityIndexByName = mainPlayerAbilitiesUISystem.getAbilityIndexByName (currentAbilityName);

						foreach (RaycastResult r in captureRaycastResults) {
							if (r.gameObject != buttonToMove) {
								//if the power element was released above other power element from the wheel, store the power element from the wheel
								if (fullAbilitiesPanelTransform.gameObject == r.gameObject) {
									slotDroppedOnFullAbilitiesPanel = true;
								} else {
									if (mainPlayerAbilitiesUISystem.isUseWheelMenuActive ()) {
										if (currentWheelAbilitiesPanelBackgroundTransform.gameObject == r.gameObject) {
											slotDroppedOnCurrentAbilitiesPanel = true;
										}
									} else {
										if (currentGridAbilitiesPanelBackgroundTransform.gameObject == r.gameObject) {
											slotDroppedOnCurrentAbilitiesPanel = true;
										}
									}
								}
							}
						}

						bool fullAbilitiesPanelChanged = false;

						if (slotDroppedOnFullAbilitiesPanel) {
							if (movingSlotFromFullAbilitiesPanel) {
								if (showDebugPrint) {
									print ("slot dropped on full abilities panel and moving slot from full abilities panel");
								}
							}

							if (movingSlotFromCurrentAbilitiesPanel) {
								if (abilityIndexByName > -1) {
									mainPlayerAbilitiesSystem.abilityInfoList [abilityIndexByName].setAbilityCanBeShownOnWheelSelectionState (false);
								} else {
									print ("ability index not found");
								}

								if (showDebugPrint) {
									print ("slot dropped on full abilities panel and moving slot from current abilities panel");
								}

								int abilityIndex = abilitySlotInfoList.FindIndex (a => a.Name == currentAbilityName);

								if (abilityIndex > -1) {
									abilitySlotInfoList [abilityIndex].slotVisible = true;
								}

								fullAbilitiesPanelChanged = true;

								mainPlayerAbilitiesUISystem.checkIfAssignActivatedAbilitiesToFreeDpadSlots (currentAbilityName, abilityIndex, false);
							
								mainPlayerAbilitiesUISystem.updateSlotsInfo ();
							}
						}

						if (slotDroppedOnCurrentAbilitiesPanel) {
							if (movingSlotFromFullAbilitiesPanel) {
								if (abilityIndexByName > -1) {
									mainPlayerAbilitiesSystem.abilityInfoList [abilityIndexByName].setAbilityCanBeShownOnWheelSelectionState (true);
								} else {
									print ("ability index not found");
								}

								if (showDebugPrint) {
									print ("slot dropped on current abilities panel and moving slot from full abilities panel");
								}

								int abilityIndex = abilitySlotInfoList.FindIndex (a => a.Name == currentAbilityName);

								if (abilityIndex > -1) {
									abilitySlotInfoList [abilityIndex].slotVisible = false;
								}

								fullAbilitiesPanelChanged = true;

								mainPlayerAbilitiesUISystem.checkIfAssignActivatedAbilitiesToFreeDpadSlots (currentAbilityName, abilityIndex, true);
							
								mainPlayerAbilitiesUISystem.updateSlotsInfo ();
							}

							if (movingSlotFromCurrentAbilitiesPanel) {
								if (showDebugPrint) {
									print ("slot dropped on current abilities panel and moving slot from current abilities panel");
								}
							}
						}

						if (!slotDroppedOnFullAbilitiesPanel && !slotDroppedOnCurrentAbilitiesPanel) {
							if (movingSlotFromCurrentAbilitiesPanel) {
								if (abilityIndexByName > -1) {
									mainPlayerAbilitiesSystem.abilityInfoList [abilityIndexByName].setAbilityCanBeShownOnWheelSelectionState (false);
								} else {
									print ("ability index not found");
								}
	
								if (showDebugPrint) {
									print ("slot dropped outside and moving slot from current abilities panel");
								}

								int abilityIndex = abilitySlotInfoList.FindIndex (a => a.Name == currentAbilityName);

								if (abilityIndex > -1) {
									abilitySlotInfoList [abilityIndex].slotVisible = true;
								}

								fullAbilitiesPanelChanged = true;

								mainPlayerAbilitiesUISystem.checkIfAssignActivatedAbilitiesToFreeDpadSlots (currentAbilityName, abilityIndex, false);
							
								mainPlayerAbilitiesUISystem.updateSlotsInfo ();
							}

							if (movingSlotFromFullAbilitiesPanel) {
								if (showDebugPrint) {
									print ("slot dropped outside and moving slot from full abilities panel ");
								}
							}
						}
							
						//remove the dragged object
						Destroy (buttonToMove);

						buttonToMoveRectTransform = null;

						updateAbilitiesUIEditorValues ();

						if (fullAbilitiesPanelChanged) {
							if (showDebugPrint) {
								print ("update full abilities panel");
							}

							resetScroll (fullAbilitiesUIEditorScrollbar);

							resetInventorySlotsRectTransform ();
						}
					}

					if (showDebugPrint) {
						print ("end of press");
					}
				}
			}
		}
	}

	public void openOrCloseAbilitiesUIEditor (bool state)
	{
		if (abilitiesUIEditorActive == state) {
			return;
		}

		abilitiesUIEditorActive = state;

		abilitiesUIEditorObject.SetActive (abilitiesUIEditorActive);

		if (abilitiesUIEditorActive) {
			updateAbilitiesUIEditorValues ();

			mainAbilitiesWheelTransform.SetParent (abilitiesWheelPosition);
			mainAbilitiesWheelTransform.localPosition = Vector3.zero;

			mainAbilitiesWheelTransform.localScale = Vector3.one * abilitiesWheelScale;

			mainAbilitiesWheelTransform.gameObject.SetActive (true);

			mainPlayerAbilitiesUISystem.updateSlotsInfo ();

			CanvasGroup abilitiesWheelCanvasGroup = mainAbilitiesWheelTransform.GetComponent<CanvasGroup> ();

			if (abilitiesWheelCanvasGroup != null) {
				abilitiesWheelCanvasGroup.alpha = 1;
			}

			resetScroll (fullAbilitiesUIEditorScrollbar);

			resetInventorySlotsRectTransform ();

		} else {
			mainAbilitiesWheelTransform.SetParent (originalAbilitiesWheelParent);
			mainAbilitiesWheelTransform.localPosition = Vector3.zero;

			mainAbilitiesWheelTransform.localScale = Vector3.one;

			mainAbilitiesWheelTransform.gameObject.SetActive (false);
		}

		mainPlayerAbilitiesUISystem.enableOrDisableExtraElementsOnAbilitiesUI (!abilitiesUIEditorActive);
	}

	public void updateAbilitiesUIEditorValues ()
	{
		if (!fullAbilitiesUIEditorContentCreated) {
			int abilitySlotInfoListCount = mainPlayerAbilitiesSystem.abilityInfoList.Count;

			for (int i = 0; i < abilitySlotInfoListCount; i++) {
				GameObject newAbilityListElement = (GameObject)Instantiate (abilityListElement, Vector3.zero, Quaternion.identity);
				newAbilityListElement.name = "Ability Slot " + (i + 1);

				newAbilityListElement.transform.SetParent (fullAbilitiesUIEditorContent);
				newAbilityListElement.transform.localScale = Vector3.one;
				newAbilityListElement.transform.position = abilityListElement.transform.position;

				playerAbilitiesUISystem.abilitySlotInfo newAbilitySlotInfo = newAbilityListElement.GetComponent<abilitySlotElement> ().slotInfo;

				abilityInfo currentAbilityInfo = mainPlayerAbilitiesSystem.abilityInfoList [i];

				newAbilitySlotInfo.Name = currentAbilityInfo.Name;

				newAbilitySlotInfo.abilityIcon.texture = currentAbilityInfo.abilityTexture;

				newAbilitySlotInfo.abilityIndex = i;

				//add this element to the list
				abilitySlotInfoList.Add (newAbilitySlotInfo);
			}

			abilityListElement.SetActive (false);

			fullAbilitiesUIEditorContentCreated = true;
		}
			

		for (int i = 0; i < mainPlayerAbilitiesSystem.abilityInfoList.Count; i++) {

			abilityInfo currentAbilityInfo = mainPlayerAbilitiesSystem.abilityInfoList [i];

			if (currentAbilityInfo.abilityEnabled) {
				playerAbilitiesUISystem.abilitySlotInfo newAbilitySlotInfo = abilitySlotInfoList [i];

				if (!newAbilitySlotInfo.slotActive && currentAbilityInfo.abilityVisibleOnWheelSelection) {

					newAbilitySlotInfo.slotActive = true;

					newAbilitySlotInfo.slotVisible = !currentAbilityInfo.abilityCanBeShownOnWheelSelection;
				}
			}
		}

		for (int i = 0; i < abilitySlotInfoList.Count; i++) {
			playerAbilitiesUISystem.abilitySlotInfo newAbilitySlotInfo = abilitySlotInfoList [i];

			if (newAbilitySlotInfo.slot.activeSelf != newAbilitySlotInfo.slotVisible) {
				newAbilitySlotInfo.slot.SetActive (newAbilitySlotInfo.slotVisible);
			}
		}
	}

	public void resetScroll (Scrollbar scrollBarToReset)
	{
		StartCoroutine (resetScrollCoroutine (scrollBarToReset));
	}

	IEnumerator resetScrollCoroutine (Scrollbar scrollBarToReset)
	{
		yield return new WaitForEndOfFrame ();
		yield return new WaitForEndOfFrame ();

		scrollBarToReset.value = 1;
	}

	public void resetInventorySlotsRectTransform ()
	{
		StartCoroutine (resetRectTransformCoroutine ());
	}

	IEnumerator resetRectTransformCoroutine ()
	{
		LayoutRebuilder.ForceRebuildLayoutImmediate (fullAbilitiesUIEditorContent.GetComponent<RectTransform> ());

		yield return new WaitForEndOfFrame ();
		yield return new WaitForEndOfFrame ();

		if (fullAbilitiesUIEditorListScrollRect) {
			fullAbilitiesUIEditorListScrollRect.verticalNormalizedPosition = 1;
		}
	}
}
