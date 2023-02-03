using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class damageInScreen : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	[Tooltip ("Show or hide the damage and healing numbers for the player and friends in the game window.")]
	public bool showScreenInfoEnabled;

	public int damageOnScreenId;

	public bool removeDamageInScreenOnDeath = true;

	public Vector3 iconOffset;

	public string mainManagerName = "Damage On Screen Info Manager";

	[Space]
	[Header ("Debug")]
	[Space]

	public bool pauseDamageInScreen;

	[Space]
	[Header ("Components")]
	[Space]

	public Transform targetTransform;

	bool damageOnScreenInfoManagerLocated;

	damageOnScreenInfoSystem damageOnScreenInfoManager;


	void Start ()
	{
		initializeDamageInScreenComponent ();
	}

	public void showScreenInfo (float amount, bool damage, Vector3 direction, float healthAmount, float criticalDamageProbability)
	{
		if (showScreenInfoEnabled && !pauseDamageInScreen) {
			if (showScreenInfoEnabled && !pauseDamageInScreen) {
				if (damageOnScreenInfoManagerLocated) {
					damageOnScreenInfoManager.setDamageInfo (damageOnScreenId, amount, damage, direction, 
						healthAmount, criticalDamageProbability);
				}
			}
		}
	}

	public void pauseOrPlayDamageInScreen (bool state)
	{
		pauseDamageInScreen = state;
	}

	public void setShowScreenInfoEnabledState (bool state)
	{
		showScreenInfoEnabled = state;
	}

	public void setShowScreenInfoEnabledStateFromEditor (bool state)
	{
		setShowScreenInfoEnabledState (state);

		updateComponent ();
	}

	public void initializeDamageInScreenComponent ()
	{
		if (targetTransform == null) {
			targetTransform = transform;
		}
			
		if (damageOnScreenInfoManager == null) {
			GKC_Utils.instantiateMainManagerOnSceneWithType (mainManagerName, typeof(damageOnScreenInfoSystem));

			damageOnScreenInfoManager = FindObjectOfType<damageOnScreenInfoSystem> ();
		}

		if (damageOnScreenInfoManager != null) {
			damageOnScreenId = damageOnScreenInfoManager.addNewTarget (targetTransform, removeDamageInScreenOnDeath, iconOffset);
		} else {
			showScreenInfoEnabled = false;
		}

		damageOnScreenInfoManagerLocated = damageOnScreenInfoManager != null;
	}

	void updateComponent ()
	{
		GKC_Utils.updateComponent (this);
	}
}