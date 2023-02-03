using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerInputPauseSystem : MonoBehaviour
{
	public bool pauseInputListDuringActionActiveAlways = true;
	public List<inputCategoryInfo> inputCategoryInfoList = new List<inputCategoryInfo> ();

	public playerInputManager mainPlayerInputManager;

	inputInfo currentInputInfo;

	public void pauseInputByCategory (string categoryName)
	{
		for (int i = 0; i < inputCategoryInfoList.Count; i++) {
			if (inputCategoryInfoList [i].Name.Equals (categoryName)) {
				for (int j = 0; j < inputCategoryInfoList [i].inputInfoList.Count; j++) {

					currentInputInfo = inputCategoryInfoList [i].inputInfoList [j];

					currentInputInfo.previousActiveState = 
					mainPlayerInputManager.setPlayerInputMultiAxesStateAndGetPreviousState (false, currentInputInfo.inputName);
				}
			}
		}
	}

	public void resumeInputByCategory (string categoryName)
	{
		for (int i = 0; i < inputCategoryInfoList.Count; i++) {
			if (inputCategoryInfoList [i].Name.Equals (categoryName)) {
				for (int j = 0; j < inputCategoryInfoList [i].inputInfoList.Count; j++) {

					currentInputInfo = inputCategoryInfoList [i].inputInfoList [j];

					if (currentInputInfo.previousActiveState) {
						mainPlayerInputManager.setPlayerInputMultiAxesState (currentInputInfo.previousActiveState, currentInputInfo.inputName);
					}
				}
			}
		}
	}

	[System.Serializable]
	public class inputCategoryInfo
	{
		public string Name;

		public List<inputInfo> inputInfoList = new List<inputInfo> ();
	}

	[System.Serializable]
	public class inputInfo
	{
		public string inputName;
		public bool previousActiveState;
	}
}
