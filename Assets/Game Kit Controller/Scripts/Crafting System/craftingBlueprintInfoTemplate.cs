using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "Crafting Blueprint Template", menuName = "GKC/Create Crafting Blueprint Template", order = 51)]
public class craftingBlueprintInfoTemplate : ScriptableObject
{
	public string Name;

	public int ID = 0;

	[Space]

	public int amountObtained = 1;

	[Space]

	public bool isRawMaterial;

	[Space]
	[Header ("Object To Place Settings")]
	[Space]

	public bool useObjectToPlace;

	public GameObject objectToPlace;

	public Vector3 objectToPlacePositionOffset;

	[Space]

	public bool useCustomLayerMaskToPlaceObject;

	public LayerMask customLayerMaskToPlaceObject;

	public LayerMask layerMaskToAttachObject;

	[Space]

	public bool objectCanBeRotatedOnYAxis = true;
	public bool objectCanBeRotatedOnXAxis;

	[Space]
	[Header ("Craft Object In Time Settings")]
	[Space]

	public bool craftObjectInTime;
	public float timeToCraftObject;

	[Space]

	[Space]
	[Header ("Craft Ingredients Settings")]
	[Space]

	public List<craftingIngredientObjectInfo> craftingIngredientObjectInfoList = new List<craftingIngredientObjectInfo> ();

	[Space]
	[Header ("Repair Ingredients Settings")]
	[Space]

	public List<craftingIngredientObjectInfo> repairIngredientObjectInfoList = new List<craftingIngredientObjectInfo> ();

	[Space]
	[Header ("Broken Ingredients Settings")]
	[Space]

	public List<craftingIngredientObjectInfo> brokenIngredientObjectInfoList = new List<craftingIngredientObjectInfo> ();

	[Space]
	[Header ("Disassemble Ingredients Settings")]
	[Space]

	public List<craftingIngredientObjectInfo> disassembleIngredientObjectInfoList = new List<craftingIngredientObjectInfo> ();

	[Space]

	[Space]
	[Header ("Stats Settings")]
	[Space]

	public bool checkStatsInfoToCraft;

	[Space]

	public List<craftingStatInfo> craftingStatInfoToCraftList = new List<craftingStatInfo> ();

	[Space]
	[Header ("Process Material Settings")]
	[Space]

	public List<processMaterialInfo> processMaterialInfoList = new List<processMaterialInfo> ();
}