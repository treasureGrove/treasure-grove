using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class bowHolderSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public int bowWeaponID;

	public bool canUseMultipleArrowType;

	public List<int> bowWeaponIDList = new List<int> ();

	[Space]
	[Header ("Events Settings")]
	[Space]

	public UnityEvent eventOnLoadArrow;

	public UnityEvent eventOnFireArrow;

	public UnityEvent eventOnStartAim;

	public UnityEvent eventOnEndAim;

	public UnityEvent eventOnCancelBowAction;

	[Space]
	[Header ("Debug Settings")]
	[Space]

	public bool showDebugPrint;


	public void checkEventOnLoadArrow ()
	{
		eventOnLoadArrow.Invoke ();

		if (showDebugPrint) {
			print ("Event On Load Arrow");
		}
	}

	public void checkEventOnFireArrow ()
	{
		eventOnFireArrow.Invoke ();

		if (showDebugPrint) {
			print ("Event On Fire Arrow");
		}
	}

	public void checkEventOnStartAim ()
	{
		eventOnStartAim.Invoke ();

		if (showDebugPrint) {
			print ("Event On Start Aim");
		}
	}

	public void checkEventOnEndAim ()
	{
		eventOnEndAim.Invoke ();

		if (showDebugPrint) {
			print ("Event On End Aim");
		}
	}

	public void checkEventOnCancelBowAction ()
	{
		eventOnCancelBowAction.Invoke ();

		if (showDebugPrint) {
			print ("Event On Cancel Bow Action");
		}
	}

	public int getBowWeaponID ()
	{
		return bowWeaponID;
	}
}
