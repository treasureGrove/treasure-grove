using UnityEngine;
using System.Collections;

public class simpleTurretRobot : MonoBehaviour
{
	public GameObject machineGun;
	public GameObject cannon;

	Animation machineGunAnim;
	Animation cannonAnim;
	public bool activated;

	void Start ()
	{
		machineGunAnim = machineGun.GetComponent<Animation> ();
		cannonAnim = cannon.GetComponent<Animation> ();
	}

	public void pressButton ()
	{
		activated = !activated;
		//stopAnimations ();
		if (activated) {
			activateWeapon ();
		} else {
			deactivateWeapon ();
		}
	}

	public void stopAnimations ()
	{
		machineGunAnim.Stop ();
		cannonAnim.Stop ();
	}

	public void activateWeapon ()
	{
		machineGunAnim ["activateMachineGun"].speed = 1; 
		//cannonAnim ["activateCannon"].time = 0;
		machineGunAnim.CrossFade ("activateMachineGun");

		cannonAnim ["activateCannon"].speed = 1; 
		//cannonAnim ["activateCannon"].time = 0;
		cannonAnim.CrossFade ("activateCannon");
	}

	public void deactivateWeapon ()
	{
		cannonAnim ["activateCannon"].speed = -1; 
		if (!cannonAnim.IsPlaying ("activateCannon")) {
			cannonAnim ["activateCannon"].time = cannonAnim ["activateCannon"].length;
			cannonAnim.Play ("activateCannon");
		} else {
			cannonAnim.CrossFade ("activateCannon");
		}

		machineGunAnim ["activateMachineGun"].speed = -1; 
		if (!machineGunAnim.IsPlaying ("activateMachineGun")) {
			machineGunAnim ["activateMachineGun"].time = machineGunAnim ["activateMachineGun"].length;
			machineGunAnim.Play ("activateMachineGun");
		} else {
			machineGunAnim.CrossFade ("activateMachineGun");
		}
	}
}