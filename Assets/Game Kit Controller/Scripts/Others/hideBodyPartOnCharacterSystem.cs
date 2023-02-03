using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hideBodyPartOnCharacterSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool hideBodyPartEnabled = true;

	public bool useTimeBullet = true;
	public float timeBulletDuration = 3;
	public float timeScale = 0.2f;

	public Transform currentBodyPartToHide;

	public void hideBodyPart ()
	{
		if (useTimeBullet) {
			GKC_Utils.activateTimeBulletXSeconds (timeBulletDuration, timeScale);
		}

		setBodyPartScale ();
	}

	public void hideBodyPart (Transform newBodyPart)
	{
		currentBodyPartToHide = newBodyPart;

		hideBodyPart ();
	}

	public void hideBodyPartWithoutBulletTimeCheck ()
	{
		setBodyPartScale ();
	}

	public void hideBodyPartWithoutBulletTimeCheck (Transform newBodyPart)
	{
		currentBodyPartToHide = newBodyPart;

		setBodyPartScale ();
	}

	public void setBodyPartScale ()
	{
		if (!hideBodyPartEnabled) {
			return;
		}

		if (currentBodyPartToHide != null) {
			currentBodyPartToHide.localScale = Vector3.zero;
		}
	}

	public void setUseTimeBulletValue (bool state)
	{
		useTimeBullet = state;
	}

	public void setHideBodyPartEnabledState (bool state)
	{
		hideBodyPartEnabled = state;
	}

	public void setUseTimeBulletValueFromEditor (bool state)
	{
		setUseTimeBulletValue (state);

		updateComponent ();
	}

	public void setHideBodyPartEnabledStateFromEditor (bool state)
	{
		setHideBodyPartEnabledState (state);

		updateComponent ();
	}

	void updateComponent ()
	{
		GKC_Utils.updateComponent (this);
	}
}
