using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class setCanvasParentSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool setNewPanelScale;
	public Vector3 newPanelScale;

	[Space]
	[Header ("Events Settings")]
	[Space]

	public bool useEventOnParentChanged;

	public UnityEvent eventOnParentChanged;

	[Space]
	[Header ("Components")]
	[Space]

	public RectTransform panelToChange;

	public RectTransform newPanelParent;


	public void activateParentChange ()
	{
		panelToChange.SetParent (newPanelParent);

		panelToChange.localPosition = Vector3.zero;

		panelToChange.localRotation = Quaternion.identity;

		if (setNewPanelScale) {
			panelToChange.localScale = newPanelScale;
		} else {
			panelToChange.localScale = Vector3.one;
		}

		if (useEventOnParentChanged) {
			eventOnParentChanged.Invoke ();
		}
	}
}
