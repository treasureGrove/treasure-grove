using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class surfaceToSlice : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool cutSurfaceEnabled = true;

	public Material crossSectionMaterial;

	public bool setNewTagOnCut;
	public string newTagOnCut;

	public bool setNewLayerOnCut;
	public string newLayerOnCut;

	public bool useCustomForceAmount;
	public float customForceAmount;

	public bool useBoxCollider;

	[Space]
	[Header ("Particle Settings")]
	[Space]

	public bool useParticlesOnSlice;
	public GameObject particlesOnSlicePrefab;

	[Space]
	[Header ("Slice Limit Settings")]
	[Space]

	public bool cutMultipleTimesActive = true;

	public bool useLimitedNumberOfCuts;
	public int limitedNumberOfCuts;

	[Space]
	[Header ("Character Settings")]
	[Space]

	public bool objectIsCharacter;
	public simpleSliceSystem mainSimpleSliceSystem;

	[Space]
	[Header ("Bullet Time Settings")]
	[Space]

	public bool useTimeBulletOnSliceEnabled;
	public float timeBulletDuration = 3;
	public float timeScale = 0.2f;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool showDebugPrint;

	[Space]
	[Header ("Events Settings")]
	[Space]

	public bool useEventOnCut;
	public UnityEvent eventOnCut;

	float lastTimeSliced;

	public void checkEventOnCut ()
	{
		if (useEventOnCut) {
			eventOnCut.Invoke ();
		}
	}

	public void copySurfaceInfo (surfaceToSlice surfaceToCopy)
	{
		lastTimeSliced = Time.time;

		surfaceToCopy.crossSectionMaterial = crossSectionMaterial;

		surfaceToCopy.setNewTagOnCut = setNewTagOnCut;
		surfaceToCopy.newTagOnCut = newTagOnCut;

		surfaceToCopy.setNewLayerOnCut = setNewLayerOnCut;
		surfaceToCopy.newLayerOnCut = newLayerOnCut;

		surfaceToCopy.cutMultipleTimesActive = cutMultipleTimesActive;

		if (useLimitedNumberOfCuts) {
			surfaceToCopy.useLimitedNumberOfCuts = useLimitedNumberOfCuts;
			surfaceToCopy.limitedNumberOfCuts = limitedNumberOfCuts - 1;
		}

		surfaceToCopy.useCustomForceAmount = useCustomForceAmount;

		surfaceToCopy.customForceAmount = customForceAmount;

		surfaceToCopy.useBoxCollider = useBoxCollider;

		surfaceToCopy.lastTimeSliced = lastTimeSliced;
	}

	public bool isObjectCharacter ()
	{
		return objectIsCharacter;
	}

	public bool isCutSurfaceEnabled ()
	{
		return cutSurfaceEnabled;
	}

	public void setCutSurfaceEnabledState (bool state)
	{
		cutSurfaceEnabled = state;

		if (showDebugPrint) {
			print ("Set Cut Surface Enabled State " + cutSurfaceEnabled);
		}
	}

	public float getLastTimeSliced ()
	{
		return lastTimeSliced;
	}

	public bool sliceCanBeActivated (float minDelayToSliceSameObject)
	{
		if (useLimitedNumberOfCuts) {
			if (limitedNumberOfCuts <= 0) {
				return false;
			}
		}

		if (Time.time > lastTimeSliced + minDelayToSliceSameObject) {
			return true;
		}

		return false;
	}

	public simpleSliceSystem getMainSimpleSliceSystem ()
	{
		return mainSimpleSliceSystem;
	}

	public void setMainSimpleSliceSystem (GameObject newObject)
	{
		if (newObject != null) {
			mainSimpleSliceSystem = newObject.GetComponent <simpleSliceSystem> ();
		}
	}

	public void setUseTimeBulletValue (bool state)
	{
		useTimeBulletOnSliceEnabled = state;
	}

	public void checkTimeBulletOnCut ()
	{
		if (useTimeBulletOnSliceEnabled) {
			GKC_Utils.activateTimeBulletXSeconds (timeBulletDuration, timeScale);
		}
	}
}
