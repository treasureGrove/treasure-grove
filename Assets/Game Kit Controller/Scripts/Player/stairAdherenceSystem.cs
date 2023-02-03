using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stairAdherenceSystem : MonoBehaviour
{
	public bool modifyStairsValuesOnPlayer = true;
	public float stairsMinValue = 0.08f;
	public float stairsMaxValue = 0.25f;
	public float stairsGroundAdherence = 6.5f;

	public void setStairsValuesOnPlayer (playerController newPlayerController)
	{
		newPlayerController.setStairsValues (stairsMinValue, stairsMaxValue, stairsGroundAdherence);
	}
}
