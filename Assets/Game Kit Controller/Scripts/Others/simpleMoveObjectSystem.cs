using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class simpleMoveObjectSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool movementEnabled = true;

	public float movementSpeed = 5;

	[Space]
	[Header ("Components")]
	[Space]

	public Transform targetPositionTransform;

	public Transform objectToMove;


	Coroutine movementCoroutine;


	public void activateMovement ()
	{
		if (!movementEnabled) {
			return;
		}

		stopActivateMovement ();

		movementCoroutine = StartCoroutine (activateMovementCoroutine ());
	}

	void stopActivateMovement ()
	{
		if (movementCoroutine != null) {
			StopCoroutine (movementCoroutine);
		}
	}

	IEnumerator activateMovementCoroutine ()
	{
		float dist = GKC_Utils.distance (targetPositionTransform.position, objectToMove.position);

		float duration = dist / movementSpeed;

		Vector3 targetPosition = targetPositionTransform.position;
		Quaternion targetRotation = targetPositionTransform.rotation;

		float movementTimer = 0;

		float angleDifference = 0;

		float positionDifference = 0;

		bool targetReached = false;

		float t = 0;

		while (!targetReached) {
			t += Time.deltaTime / duration;

			objectToMove.position = Vector3.Lerp (objectToMove.position, targetPosition, t);
			objectToMove.rotation = Quaternion.Lerp (objectToMove.rotation, targetRotation, t);

			angleDifference = Quaternion.Angle (objectToMove.localRotation, targetRotation);

			positionDifference = GKC_Utils.distance (objectToMove.localPosition, targetPosition);

			movementTimer += Time.deltaTime;

			if ((positionDifference < 0.01f && angleDifference < 0.2f) || movementTimer > (duration + 1)) {
				targetReached = true;
			}

			yield return null;
		}
	}
}
