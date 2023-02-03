using UnityEngine;
using System.Collections;

public class fallingPlatform : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public float movementSpeed;
	public bool fallInTouch;
	public bool fallInTime;
	public float fallDelay;
	public bool resetDelayInExit;
	public float timeToBackInPosition;
	public float extraForceInFall;

	public bool keepFallCheckOnFirstContact;

	[Space]
	[Header ("Components")]
	[Space]

	public Rigidbody mainRigidbody;

	bool inside;
	bool platformFallen;
	bool movePlatformToPosition;
	float timeOnPlatform;
	float fallenTime;
	Vector3 originalPosition;
	Quaternion originalRotation;

	bool checkingToFallPlatform;


	void Start ()
	{
		if (mainRigidbody == null) {
			mainRigidbody = GetComponent<Rigidbody> ();
		}

		originalPosition = transform.position;
		originalRotation = transform.rotation;
	}

	void Update ()
	{
		if (!movePlatformToPosition) {
			if (platformFallen && mainRigidbody.velocity.magnitude < 1) {
				fallenTime += Time.deltaTime;

				if (fallenTime > timeToBackInPosition) {
					StartCoroutine (moveToOriginalPosition ());

				}
			} else {
				bool checkingToFall = false;

				if (inside) {
					checkingToFall = true;
				} else {
					if (keepFallCheckOnFirstContact && checkingToFallPlatform) {
						checkingToFall = true;
					}
				}

				if (checkingToFall) {
					if (fallInTouch) {
						mainRigidbody.isKinematic = false;

						mainRigidbody.AddForce (-transform.up * extraForceInFall);

						platformFallen = true;
						fallenTime = 0;
						inside = false;

						checkingToFallPlatform = false;
					}

					if (fallInTime) {
						timeOnPlatform += Time.deltaTime;

						if (timeOnPlatform > fallDelay) {
							mainRigidbody.isKinematic = false;

							mainRigidbody.AddForce (-transform.up * extraForceInFall);

							platformFallen = true;
							fallenTime = 0;
							inside = false;

							checkingToFallPlatform = false;
						}
					}
				}
			}
		}
	}

	void OnCollisionEnter (Collision col)
	{
		if (col.gameObject.CompareTag ("Player") && !inside && !platformFallen) {
			inside = true;

			if (!checkingToFallPlatform) {
				timeOnPlatform = 0;
			}

			checkingToFallPlatform = true;
		}
	}

	void OnCollisionExit (Collision col)
	{
		if (col.gameObject.CompareTag ("Player") && inside) {
			inside = false;

			if (resetDelayInExit) {
				timeOnPlatform = 0;
			}
		}
	}

	IEnumerator moveToOriginalPosition ()
	{
		platformFallen = false;

		mainRigidbody.isKinematic = true;

		movePlatformToPosition = true;

		while (GKC_Utils.distance (transform.position, originalPosition) > .01f) {
			transform.position = Vector3.MoveTowards (transform.position, originalPosition, Time.deltaTime * movementSpeed);
			transform.rotation = Quaternion.Slerp (transform.rotation, originalRotation, Time.deltaTime * movementSpeed);

			yield return null;
		}

		movePlatformToPosition = false;
	}
}