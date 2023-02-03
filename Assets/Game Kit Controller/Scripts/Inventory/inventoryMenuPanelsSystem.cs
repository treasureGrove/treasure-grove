using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class inventoryMenuPanelsSystem : MonoBehaviour
{
	public List<inventoryPanelsInfo> inventoryPanelsInfoList = new List<inventoryPanelsInfo> ();

	public bool setStateOnStart;
	public string stateOnStart;

	public string currentState;

	inventoryPanelsInfo currentInventoryPanelsInfo;

	inventoryPanelInfo currentInventoryPanelInfo;

	void Start ()
	{
		if (setStateOnStart) {
			setInventoryPanelByName (stateOnStart);
		}
	}

	public void setInventoryPanelByName (string panelName)
	{
		if (currentState.Equals (panelName)) {
			return;
		}

		for (int i = 0; i < inventoryPanelsInfoList.Count; i++) {
			currentInventoryPanelsInfo = inventoryPanelsInfoList [i];

			if (!currentInventoryPanelsInfo.Name.Equals (panelName)) {
				currentInventoryPanelsInfo.isCurrentState = false;

				for (int j = 0; j < currentInventoryPanelsInfo.extraElementList.Count; j++) {
					if (currentInventoryPanelsInfo.extraElementList [j].activeSelf) {
						currentInventoryPanelsInfo.extraElementList [j].SetActive (false);
					}
				}

				for (int j = 0; j < currentInventoryPanelsInfo.inventoryPanelInfoList.Count; j++) {
					currentInventoryPanelsInfo.inventoryPanelInfoList [j].inventoryPanel.transform.localScale = Vector3.one;
				}
			}
		}

		for (int i = 0; i < inventoryPanelsInfoList.Count; i++) {
			currentInventoryPanelsInfo = inventoryPanelsInfoList [i];

			if (currentInventoryPanelsInfo.Name.Equals (panelName)) {

				currentState = panelName;
			
				currentInventoryPanelsInfo.isCurrentState = true;

				if (currentInventoryPanelsInfo.useEventOnPanel) {
					currentInventoryPanelsInfo.eventOnPanel.Invoke ();
				}

				for (int j = 0; j < currentInventoryPanelsInfo.inventoryPanelInfoList.Count; j++) {

					currentInventoryPanelInfo = currentInventoryPanelsInfo.inventoryPanelInfoList [j];

					if (currentInventoryPanelInfo.inventoryPanel.gameObject.activeSelf != currentInventoryPanelInfo.isActive) {
						currentInventoryPanelInfo.inventoryPanel.gameObject.SetActive (currentInventoryPanelInfo.isActive);
					}

					if (currentInventoryPanelInfo.setParentOnOtherPanelBeforeAdjust) {
						currentInventoryPanelInfo.inventoryPanel.SetParent (currentInventoryPanelInfo.otherPanelBeforeAdjust);
					}

					if (currentInventoryPanelInfo.adjustPanelToRectTransform) {

						currentInventoryPanelInfo.inventoryPanel.anchorMin = currentInventoryPanelInfo.rectTransformToAdjust.anchorMin;
						currentInventoryPanelInfo.inventoryPanel.anchorMax = currentInventoryPanelInfo.rectTransformToAdjust.anchorMax;

						currentInventoryPanelInfo.inventoryPanel.anchoredPosition = currentInventoryPanelInfo.rectTransformToAdjust.anchoredPosition;

						currentInventoryPanelInfo.inventoryPanel.sizeDelta = currentInventoryPanelInfo.rectTransformToAdjust.sizeDelta;
					}

					if (currentInventoryPanelInfo.setPanelParent) {
						currentInventoryPanelInfo.inventoryPanel.SetParent (currentInventoryPanelInfo.panelParent);
					}

					if (currentInventoryPanelInfo.setNewScale) {
						currentInventoryPanelInfo.inventoryPanel.transform.localScale = 
							new Vector3 (currentInventoryPanelInfo.newScale.x, currentInventoryPanelInfo.newScale.y, 1);
					}
				}

				for (int j = 0; j < currentInventoryPanelsInfo.extraElementList.Count; j++) {
					if (!currentInventoryPanelsInfo.extraElementList [j].activeSelf) {
						currentInventoryPanelsInfo.extraElementList [j].SetActive (true);
					}
				}
			}
		}
	}

	[System.Serializable]
	public class inventoryPanelsInfo
	{
		public string Name;

		public bool isCurrentState;

		public List<inventoryPanelInfo> inventoryPanelInfoList = new List<inventoryPanelInfo> ();

		public List<GameObject> extraElementList = new List<GameObject> ();

		public bool useEventOnPanel;
		public UnityEvent eventOnPanel;
	}

	[System.Serializable]
	public class inventoryPanelInfo
	{
		public string Name;
		public RectTransform inventoryPanel;
		public bool isActive;

		public bool adjustPanelToRectTransform;
		public RectTransform rectTransformToAdjust;

		public bool setPanelParent;
		public RectTransform panelParent;

		public bool setParentOnOtherPanelBeforeAdjust;
		public RectTransform otherPanelBeforeAdjust;

		public bool setNewScale;
		public Vector2 newScale;
	}
}
