using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class editButtonInput : MonoBehaviour
{
	public Text actionNameText;
	public Text actionKeyText;

	public string keyboardActionKey;
	public string gamepadActionKey;

	public int multiAxesIndex;
	public int axesIndex;

	public GameObject editButtonGameObject;

	public bool editButtonInputActive;
}