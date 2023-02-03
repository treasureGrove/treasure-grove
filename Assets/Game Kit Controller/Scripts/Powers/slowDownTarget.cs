using UnityEngine;
using System.Collections;

public class slowDownTarget : projectileSystem
{
	[Header ("Main Settings")]
	[Space]

	public string characterStateAffectedName = "Character Freeze Velocity";

	public float slowDownDuration = 6;

	[Range (0, 1)] public float slowValue = 0.2f;

	public bool useSlowObjectColor = true;
	public Color slowObjectsColor = Color.blue;

	void OnTriggerEnter (Collider col)
	{
		if (canActivateEffect (col)) {
			if (currentProjectileInfo.impactSoundEffect != null) {
				GetComponent<AudioSource> ().PlayOneShot (currentProjectileInfo.impactSoundEffect);
			}

			projectileUsed = true;
			objectToDamage = col.GetComponent<Collider> ().gameObject;

			characterDamageReceiver currentCharacterDamageReceiver = objectToDamage.GetComponent<characterDamageReceiver> ();

			if (currentCharacterDamageReceiver != null) {
				objectToDamage = currentCharacterDamageReceiver.character;
			} 

			slowObject currentSlowObject = objectToDamage.GetComponent<slowObject> ();

			if (currentSlowObject == null) {
				playerComponentsManager mainPlayerComponentsManager = objectToDamage.GetComponent<playerComponentsManager> ();

				if (mainPlayerComponentsManager != null) {
					characterPropertiesSystem currentCharacterPropertiesSystem = mainPlayerComponentsManager.getCharacterPropertiesSystem ();

					if (currentCharacterPropertiesSystem != null) {
						characterStateAffectedInfo currentCharacterStateAffectedInfo = currentCharacterPropertiesSystem.getCharacterStateAffectedInfoByName (characterStateAffectedName);

						if (currentCharacterStateAffectedInfo != null) {
							currentSlowObject = currentCharacterStateAffectedInfo as slowObject;
						}
					}
				}
			}

			if (currentSlowObject != null) {

				slowObjectsColor currentSlowObjectsColor = currentSlowObject.getObjectToCallFunction ().GetComponent<slowObjectsColor> ();

				if (currentSlowObjectsColor == null) {
					currentSlowObjectsColor = currentSlowObject.getObjectToCallFunction ().AddComponent<slowObjectsColor> ();
				}

				if (currentSlowObjectsColor != null) {
					currentSlowObjectsColor.startSlowObject (useSlowObjectColor, slowObjectsColor, slowValue, slowDownDuration, currentSlowObject);
				}
			}

			disableBullet (currentProjectileInfo.impactDisableTimer);
		}
	}

	public override void resetProjectile ()
	{
		base.resetProjectile ();


	}
}
