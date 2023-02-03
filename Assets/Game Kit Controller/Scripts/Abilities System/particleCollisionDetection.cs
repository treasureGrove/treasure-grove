using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class particleCollisionDetection : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool detectionEnabled = true;

	public bool useLayermaskOnObjectsDetected;
	public LayerMask layermaskToCheck;

	[Space]
	[Header ("Components")]
	[Space]

	public ParticleSystem mainParticleSystem;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool debugCollisionPrintActive;

	[Space]
	[Header ("Events Settings")]
	[Space]

	public bool useEventOnCollision = true;

	public eventParameters.eventToCallWithVector3 eventOnCollision;

	[Space]

	public bool useEventOnCollisionWithGameObject;

	public eventParameters.eventToCallWithGameObject eventOnCollisionWithGameObject;

	public List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent> ();

	void OnParticleCollision (GameObject obj)
	{
		if (!detectionEnabled) {
			return;
		}

		int numCollisionEvents = mainParticleSystem.GetCollisionEvents (obj, collisionEvents);

		if (debugCollisionPrintActive) {
			print (numCollisionEvents);
		}

		if (numCollisionEvents > 0) {

			if (useLayermaskOnObjectsDetected) {
				if ((1 << obj.layer & layermaskToCheck.value) != 1 << obj.layer) {
					return;
				}
			}

			if (useEventOnCollision) {
				eventOnCollision.Invoke (collisionEvents [0].intersection);
			}

			if (useEventOnCollisionWithGameObject) {
				eventOnCollisionWithGameObject.Invoke (obj);
			}

			if (debugCollisionPrintActive) {
				print (collisionEvents [0].intersection);

				print ("Particle Collision Detected with object " + obj.name);
			}
		}
	}

	public void setDetectionEnabledState (bool state)
	{
		detectionEnabled = state;
	}
}
