using UnityEngine;
using System.Collections;

public class enemyLaser : laser
{
	[Space]
	[Header ("Main Settings")]
	[Space]

	public LayerMask layer;
	public GameObject hitParticles;
	public GameObject hitSparks;
	public float laserDamage = 0.2f;
	public bool ignoreShield;

	public int damageTypeID = -1;

	RaycastHit hit;
	GameObject owner;

	void Start ()
	{
		StartCoroutine (laserAnimation ());
	}

	void Update ()
	{
		//check the hit collider of the raycast
		if (Physics.Raycast (transform.position, transform.forward, out hit, Mathf.Infinity, layer)) {
			applyDamage.checkHealth (gameObject, hit.collider.gameObject, laserDamage, -transform.forward, (hit.point - (hit.normal / 4)),
				owner, true, true, ignoreShield, false, false, -1, damageTypeID);

			//set the sparks and .he smoke in the hit point
			laserDistance = hit.distance;
			hitSparks.SetActive (true);
			hitParticles.SetActive (true);

			hitParticles.transform.position = hit.point + (transform.position - hit.point) * 0.02f;
			hitParticles.transform.rotation = Quaternion.identity;
			hitSparks.transform.rotation = Quaternion.LookRotation (hit.normal, transform.up);
		} else {
			//if the laser does not hit anything, disable the particles and set the hit point
			hitParticles.SetActive (false);
			hitParticles.SetActive (false);
			laserDistance = 1000;	
		}

		//set the size of the laser, according to the hit position
		lRenderer.SetPosition (1, (laserDistance * Vector3.forward));
		animateLaser ();
	}

	public void setOwner (GameObject laserOwner)
	{
		owner = laserOwner;
	}
}