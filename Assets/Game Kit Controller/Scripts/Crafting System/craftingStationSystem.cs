using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class craftingStationSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public string stationName;

	public bool inputSockedEnabled = true;
	public bool outputSocketEnabled = true;

	public craftingSocket inputSocket;

	public craftingSocket outputSocket;

	public bool chechSocketsOnStart;

	[Space]
	[Header ("Stations To Connect Settings")]
	[Space]

	public List<string> stationToConnectNameList = new List<string> ();

	[Space]
	[Header ("Other Settings")]
	[Space]

	public energyStationSystem currentEnergyStationSystem;

	public bool removeRemainEnergyOnRemoveEnergyStation;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool useInfiniteEnergy;

	public bool energyStationAssigned;

	public List<craftingStationSystem> craftingStationSystemConnectedList = new List<craftingStationSystem> ();

	[Space]
	[Header ("Events Settings")]
	[Space]

	public bool useEventOnInputConnected;
	public UnityEvent eventOnInputConnected;

	[Space]

	public bool useEventOnInputRemoved;
	public UnityEvent eventOnInputRemoved;

	[Space]

	public bool useEventOnOutputConnected;
	public UnityEvent eventOnOutputConnected;

	[Space]

	public bool useEventOnOutputRemoved;
	public UnityEvent eventOnOutputRemoved;



	void Start ()
	{
		if (chechSocketsOnStart) {
			if (inputSocket != null) {
				setInputSocket (inputSocket);
			}

			if (outputSocket != null) {
				setOutputSocket (outputSocket);
			}
		}
	}

	public virtual void setInputSocket (craftingSocket newSocket)
	{
		if (inputSockedEnabled) {
			inputSocket = newSocket;

			if (useEventOnInputConnected) {
				eventOnInputConnected.Invoke ();
			}

			checkStateOnSetInput ();
		}
	}

	public virtual void removeInputSocket ()
	{
		if (inputSockedEnabled) {
			checkStateOnRemoveInput ();

			inputSocket = null;

			if (useEventOnInputRemoved) {
				eventOnInputRemoved.Invoke ();
			}
		}
	}

	public virtual void setOutputSocket (craftingSocket newSocket)
	{
		if (inputSockedEnabled) {
			outputSocket = newSocket;

			if (useEventOnOutputConnected) {
				eventOnOutputConnected.Invoke ();
			}

			checkStateOnSetOuput ();
		}
	}

	public virtual void removeOutputSocket ()
	{
		if (inputSockedEnabled) {
			checkStateOnRemoveOuput ();

			outputSocket = null;

			if (useEventOnOutputConnected) {
				eventOnOutputRemoved.Invoke ();
			}
		}
	}

	public virtual void sendEnergyValue (float newAmount)
	{

	}

	public virtual void setInfiniteEnergyState (bool state)
	{
		useInfiniteEnergy = state;
	}

	public virtual void checkStateOnSetInput ()
	{

	}

	public virtual void checkStateOnSetOuput ()
	{

	}

	public virtual void checkStateOnRemoveInput ()
	{

	}

	public virtual void checkStateOnRemoveOuput ()
	{

	}

	public virtual void setCurrentEnergyStationSystem (energyStationSystem newEnergyStationSystem)
	{
		currentEnergyStationSystem = newEnergyStationSystem;

		energyStationAssigned = currentEnergyStationSystem != null;

		checkEnergyStationOnStateChange ();
	}

	public virtual void checkEnergyStationOnStateChange ()
	{

	}

	public virtual void checkStateOnTakeObjectBack ()
	{
		if (outputSocket.currentCraftingStationSystemAssigned != null) {
			outputSocket.currentCraftingStationSystemAssigned.checkStateOnRemoveInput ();
			outputSocket.currentCraftingStationSystemAssigned.checkStateOnRemoveOuput ();
		}

		if (inputSocket.currentCraftingStationSystemAssigned != null) {
			inputSocket.currentCraftingStationSystemAssigned.checkStateOnRemoveInput ();
			inputSocket.currentCraftingStationSystemAssigned.checkStateOnRemoveOuput ();
		}

		checkStateOnRemoveInput ();

		checkStateOnRemoveOuput ();
	}

	public string getStationName ()
	{
		return stationName;
	}

	public void addCraftingStationSystem (craftingStationSystem newCraftingStationSystem)
	{
		if (!craftingStationSystemConnectedList.Contains (newCraftingStationSystem)) {
			craftingStationSystemConnectedList.Add (newCraftingStationSystem);
		}
	}

	public void removeCraftingStationSystem (craftingStationSystem newCraftingStationSystem)
	{
		if (craftingStationSystemConnectedList.Contains (newCraftingStationSystem)) {
			craftingStationSystemConnectedList.Remove (newCraftingStationSystem);
		}
	}

	public void clearCraftingStationSystemList ()
	{
		craftingStationSystemConnectedList.Clear ();
	}
}
