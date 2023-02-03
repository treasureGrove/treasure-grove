using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class energyStationSystem : craftingStationSystem
{
	[Header ("Main Settings")]
	[Space]

	public bool energyEnabled = true;

	public string energyName;

	public float maxEnergyAmount;

	public float currentEnergyAmount;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool energyActive;


	public float getCurrentEnergyAmount ()
	{
		return currentEnergyAmount;
	}

	public string getEnergyName ()
	{
		return energyName;
	}

	public void setEnergyActiveState (bool state)
	{
		if (!energyEnabled) {
			return;
		}

		energyActive = state;
	}

	public override void checkStateOnSetOuput ()
	{
		if (outputSocket != null) {
			if (outputSocket.currentCraftingStationSystemAssigned != null) {
				outputSocket.currentCraftingStationSystemAssigned.sendEnergyValue (currentEnergyAmount);

				outputSocket.currentCraftingStationSystemAssigned.setInfiniteEnergyState (useInfiniteEnergy);

				outputSocket.currentCraftingStationSystemAssigned.setCurrentEnergyStationSystem (this);
			}
		}
	}

	public override void checkStateOnRemoveOuput ()
	{
		if (outputSocket != null) {
			if (outputSocket.currentCraftingStationSystemAssigned != null) {
				outputSocket.currentCraftingStationSystemAssigned.setInfiniteEnergyState (false);

				outputSocket.currentCraftingStationSystemAssigned.setCurrentEnergyStationSystem (null);
			}
		}
	}
}
