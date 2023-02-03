using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class UIMouseHoverEvent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	public UnityEvent eventOnTriggerEnter;
	public UnityEvent eventOnTriggerExit;

	//Detect if the Cursor starts to pass over the GameObject
	public void OnPointerEnter (PointerEventData pointerEventData)
	{
		eventOnTriggerEnter.Invoke();
	}

	//Detect when Cursor leaves the GameObject
	public void OnPointerExit (PointerEventData pointerEventData)
	{
		eventOnTriggerExit.Invoke();
	}
}
