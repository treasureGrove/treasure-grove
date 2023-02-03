using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class craftingSocket : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool isInputSocket;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool socketAssigned;

	public Transform currentTargetTransform;

	[Space]
	[Header ("Components")]
	[Space]

	public craftingStationSystem currentCraftingStationSystemAssigned;

	public LineRenderer mainLineRenderer;



	public void assignCraftingStationSystem (craftingStationSystem newCraftingStationSystem)
	{
		currentCraftingStationSystemAssigned = newCraftingStationSystem;

		checkIfSocketAssigned ();
	}

	public void removeCraftingStationSystem ()
	{
		currentCraftingStationSystemAssigned = null;
	}

	public void checkIfSocketAssigned ()
	{
		socketAssigned = currentCraftingStationSystemAssigned != null;
	}

	public void enableOrDisableLineRenderer (bool state)
	{
		if (mainLineRenderer != null) {
			if (mainLineRenderer.enabled != state) {
				mainLineRenderer.enabled = state;
			}
		}
	}

	public void enableLineRendererIfSocketAssigned ()
	{
		if (socketAssigned) {
			enableOrDisableLineRenderer (true);
		}
	}

	public void updateLinerenderPositions ()
	{
		if (currentTargetTransform != null) {
			setLineRendererTargetPosition (currentTargetTransform.position);
		}
	}

	public void setLineRendererTargetPosition (Vector3 targetPosition)
	{
		if (mainLineRenderer != null) {
			mainLineRenderer.SetPosition (0, transform.position);
			mainLineRenderer.SetPosition (1, targetPosition);
		}
	}

	public void setLineRendererTargetPosition (Transform targetTransform)
	{
		if (mainLineRenderer != null) {
			currentTargetTransform = targetTransform;

			mainLineRenderer.SetPosition (0, transform.position);
			mainLineRenderer.SetPosition (1, targetTransform.position);
		}
	}
}
