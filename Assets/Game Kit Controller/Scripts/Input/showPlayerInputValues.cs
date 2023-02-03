using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class showPlayerInputValues : MonoBehaviour
{
	public Text inputText;
	public playerInputManager playerInput;

	void Update ()
	{
		inputText.text = playerInput.getPlayerMovementAxis ().ToString();
	}
}
