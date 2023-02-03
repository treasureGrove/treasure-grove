using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class inventoryQuickAccessSlotElement : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public quickAccessSlotInfo mainQuickAccessSlotInfo;

	[System.Serializable]
	public class quickAccessSlotInfo
	{
		public string Name;
		public bool slotActive;
		public GameObject slot;

		[Space]

		public string slotCategoryName;

		[Space]

		public Text amountText;
		public RawImage slotIcon;

		[Space]

		public RawImage rightSecondarySlotIcon;
		public RawImage leftSecondarySlotIcon;

		[Space]

		public bool secondarySlotActive;

		public string firstElementName;
		public string secondElementName;

		[Space]

		public Text iconNumberKeyText;

		public Transform slotSelectedIconPosition;

		public GameObject currentlySelectedIcon;

		public Image backgroundImage;

		public GameObject slotMainSingleContent;
		public GameObject slotMainDualContent;

		public GameObject amountTextContent;

		[Space]
		[Space]

		public inventoryInfo inventoryInfoAssigned;
	}
}