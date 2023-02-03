using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GKCInventoryConditionSystem : GKCConditionInfo
{
	[Header ("Custom Settings")]
	[Space]

	public List<inventoryConditionInfo> inventoryConditionInfoList = new List<inventoryConditionInfo> ();

	public bool checkIfInventoryIsEmpty;

	public bool checkIfAnyWeaponIsEquipped;

	public bool anyWeaponMustBeEquipped;

	public override void checkIfConditionComplete ()
	{
		if (!checkIfPlayerAssigned ()) {
			return;
		}

		bool conditionResult = false;

		playerComponentsManager mainPlayerComponentsManager = currentPlayer.GetComponent<playerComponentsManager> ();

		if (mainPlayerComponentsManager != null) {
			inventoryManager mainInventoryManager = mainPlayerComponentsManager.getInventoryManager ();

			if (mainInventoryManager != null) {
				bool currentConditionState = true;

				for (int i = 0; i < inventoryConditionInfoList.Count; i++) {
					if (currentConditionState) {
						bool objectLocated = 
							mainInventoryManager.getInventoryObjectAmountByName (inventoryConditionInfoList [i].inventoryObjectName) >=
							inventoryConditionInfoList [i].inventoryAmount;

						if (objectLocated) {
							if (inventoryConditionInfoList [i].inventoryIsEquipped) {
								if (!mainInventoryManager.checkIfInventoryObjectEquipped (inventoryConditionInfoList [i].inventoryObjectName)) {
									currentConditionState = false;
								}
							}
						} else {
							if (inventoryConditionInfoList [i].inventoryAmount > 0) {
								currentConditionState = false;
							}
						}
					}
				}

				if (checkIfInventoryIsEmpty) {
					if (!mainInventoryManager.isInventoryEmpty ()) {
						currentConditionState = false;
					}
				}

				if (checkIfAnyWeaponIsEquipped) {
					bool isAnyInventoryWeaponEquipped = mainInventoryManager.isAnyInventoryWeaponEquipped ();

					if (anyWeaponMustBeEquipped) {
						if (!isAnyInventoryWeaponEquipped) {
							currentConditionState = false;
						}
					} else {
						if (isAnyInventoryWeaponEquipped) {
							currentConditionState = false;
						}
					}
				}

				conditionResult = currentConditionState;
			}
		}

		setConditionResult (conditionResult);
	}

	[System.Serializable]
	public class inventoryConditionInfo
	{
		public string inventoryObjectName;
		public int inventoryAmount;
		public bool inventoryIsEquipped;
	}
}
