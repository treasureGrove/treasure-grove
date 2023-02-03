using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//using EzySlice;

//using NobleMuffins.LimbHacker.Guts;
//using NobleMuffins.LimbHacker;

public class sliceSystemUtils : MonoBehaviour
{
	
	public static void sliceObject (Vector3 slicePosition, GameObject objectToSlice, Vector3 cutDirection, Material crossSectionMaterial, ref bool objectSliced, ref GameObject object1, ref GameObject object2)
	{
		//SlicedHull hull = objectToSlice.SliceObject (slicePosition, cutDirection, crossSectionMaterial);

		//if (hull != null) {

		//	objectSliced = true;

		//	object1 = hull.CreateLowerHull (objectToSlice, crossSectionMaterial);
		//	object2 = hull.CreateUpperHull (objectToSlice, crossSectionMaterial);
		//}
	}

	public static surfaceToSlice getSurfaceToSlice (GameObject currentSurface)
	{
		//ChildOfHackable currentChildOfHackable = currentSurface.GetComponent<ChildOfHackable> ();

		//if (currentChildOfHackable != null) {
		//	return currentChildOfHackable.parentHackable.mainSurfaceToSlice;
		//}

		return null;
	}

	public static void initializeValuesOnHackableComponent (GameObject objectToUse, simpleSliceSystem currentSimpleSliceSystem)
	{
		//Hackable currentHackable = objectToUse.GetComponent<Hackable> ();

		//if (currentHackable == null) {
		//	currentHackable = objectToUse.AddComponent<Hackable> ();
		//}

		//currentHackable.mainSurfaceToSlice = currentSimpleSliceSystem.mainSurfaceToSlice;

		//currentHackable.alternatePrefab = currentSimpleSliceSystem.getMainAlternatePrefab ();

		//currentHackable.objectToSlice = currentSimpleSliceSystem.objectToSlice;

		//currentHackable.infillMaterial = currentSimpleSliceSystem.infillMaterial;

		//currentHackable.severables = currentSimpleSliceSystem.getSeverables ();

		//currentHackable.initializeValues ();
	}

	public static void sliceCharacter (GameObject objectToSlice, Vector3 point, Vector3 newNormalInWorldSpaceValue)
	{
		//if (objectToSlice == null) {
		//	return;
		//}

		//Hackable currentHackable = objectToSlice.GetComponent<Hackable> ();

		//currentHackable.activateSlice (objectToSlice, point, newNormalInWorldSpaceValue);
	}


	public static GameObject[] shatterObject (GameObject obj, Vector3 shatterPosition, Material crossSectionMaterial = null, bool shaterActive = true)
	{
		//if (shaterActive) {
		//	return obj.SliceInstantiate (getRandomPlane (shatterPosition, obj.transform.localScale),
		//		new TextureRegion (0.0f, 0.0f, 1.0f, 1.0f),
		//		crossSectionMaterial);
		//} 

		return null;
	}

	//public static EzySlice.Plane getRandomPlane (Vector3 positionOffset, Vector3 scaleOffset)
	//{
	//	Vector3 randomPosition = Random.insideUnitSphere;
	//	Vector3 randomDirection = Random.insideUnitSphere.normalized;

	//	return new EzySlice.Plane (randomPosition, randomDirection);
	//}
}