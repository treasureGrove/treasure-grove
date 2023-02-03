using UnityEngine;
using System.Collections;

public class vehicleLaser : laser
{
	[Space]
	[Header ("Main Settings")]
	[Space]

	public float laserDamage = 0.3f;
	public float laserDamageRate = 0.5f;
	public bool ignoreShield;

	public int damageTypeID = -1;

	[Space]
	[Header ("Components")]
	[Space]

	public LayerMask layer;
	public GameObject hitParticles;
	public GameObject hitSparks;
	public GameObject vehicle;
	public GameObject vehicleCamera;

	public vehicleCameraController vehicleCameraManager;
	public vehicleHUDManager vehicleHUD;

	RaycastHit hit;
	bool laserActive;
	Transform mainCameraTransform;

	GameObject currentDriver;
	Vector3 laserPosition;

	float lastTimeDamageActive;

	void Start ()
	{
		changeLaserState (false);
	}

	void Update ()
	{
		if (laserActive) {
			//check the hit collider of the raycast
			mainCameraTransform = vehicleCameraManager.getCurrentCameraTransform ();

			if (Physics.Raycast (mainCameraTransform.position, mainCameraTransform.forward, out hit, Mathf.Infinity, layer)) {
				Debug.DrawRay (transform.position, mainCameraTransform.forward * hit.distance, Color.red);

				transform.LookAt (hit.point);

				if (Time.time > laserDamageRate + lastTimeDamageActive) {
					applyDamage.checkHealth (gameObject, hit.collider.gameObject, laserDamage, -transform.forward, (hit.point - (hit.normal / 4)), 
						currentDriver, true, true, ignoreShield, false, false, -1, damageTypeID);

					lastTimeDamageActive = Time.time;
				}

				//set the sparks and .he smoke in the hit point
				laserDistance = hit.distance;

				if (!hitSparks.activeSelf) {
					hitSparks.SetActive (true);
				}

				if (!hitParticles.activeSelf) {
					hitParticles.SetActive (true);
				}

				hitParticles.transform.position = hit.point + (transform.position - hit.point) * 0.02f;
				hitParticles.transform.rotation = Quaternion.identity;
				hitSparks.transform.rotation = Quaternion.LookRotation (hit.normal, transform.up);
				laserPosition = hit.point;
			} else {
				//if the laser does not hit anything, disable the particles and set the hit point
				if (hitSparks.activeSelf) {
					hitSparks.SetActive (false);
				}

				if (hitParticles.activeSelf) {
					hitParticles.SetActive (false);
				}

				laserDistance = 1000;	

				Quaternion lookDir = Quaternion.LookRotation (mainCameraTransform.forward);

				transform.rotation = lookDir;
				laserPosition = (laserDistance * transform.forward);
			}

			//set the size of the laser, according to the hit position
			lRenderer.positionCount = 2;
			lRenderer.SetPosition (0, transform.position);
			lRenderer.SetPosition (1, laserPosition);

			animateLaser ();
		}
	}

	//enable or disable the vehicle laser
	public void changeLaserState (bool state)
	{
		lRenderer.enabled = state;

		laserActive = state;

		if (state) {
			StartCoroutine (laserAnimation ());

			lastTimeDamageActive = 0;
		} else {
			hitSparks.SetActive (false);
			hitParticles.SetActive (false);
		}

		if (state) {
			currentDriver = vehicleHUD.getCurrentDriver ();
		}
	}
}