using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class headTrackTarget : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool targetEnabled = true;

	public Vector3 positionOffset;
	public float minDistanceToLook = 4;
	public targetVisibilityTypes visibilityTypes;
	public List<string> tagsToLocate = new List<string> ();

	public bool useCustomLayer;
	public LayerMask customLayer;

	public bool storeHeadTrackFound;

	[Space]
	[Header ("Debug")]
	[Space]

	public List<headTrack> headTrackFoundList = new List<headTrack> ();

	[Space]
	[Header ("Gizmo Settings")]
	[Space]

	public bool showGizmo;
	public Color gizmoColor = Color.yellow;

	[Space]
	[Header ("Components")]
	[Space]

	public Transform targetToLook;
	public Collider mainCollider;

	public enum targetVisibilityTypes
	{
		None,
		Raycast
	}

	headTrack currentHeadTrack;

	LayerMask currentLayerMask;
	Vector3 positionToLook;
	RaycastHit hit;

	bool targetToLookLocated;

	bool targetLoLookChecked;

	public Vector3 getLookPositon ()
	{
		if (!targetLoLookChecked) {
			if (targetToLook != null) {
				targetToLookLocated = true;
			}

			targetLoLookChecked = true;
		}

		if (targetToLookLocated) {
			return targetToLook.position + positionOffset;
		} else {
			return transform.TransformPoint (positionOffset);
		}
	}

	public bool lookTargetVisible (Vector3 headPosition, LayerMask layer)
	{
		if (!targetEnabled) {
			return false;
		}

		positionToLook = getLookPositon ();

		if (visibilityTypes == targetVisibilityTypes.None) {
			if (GKC_Utils.distance (headPosition, positionToLook) > minDistanceToLook) {
				return false;
			} else {
				return true;
			}
		} else if (visibilityTypes == targetVisibilityTypes.Raycast) {
			if (GKC_Utils.distance (headPosition, positionToLook) > minDistanceToLook) {
				return false;
			} else {
				if (useCustomLayer) {
					currentLayerMask = customLayer;
				} else {
					currentLayerMask = layer;
				}

				if (Physics.Linecast (headPosition, positionToLook, out hit, currentLayerMask)) {
					if (hit.transform != transform) {
						if (showGizmo) {
							drawLine (headPosition, hit.point, Color.red);
						}

						return false;
					} else {
						if (showGizmo) {
							drawLine (headPosition, hit.point, Color.green);
						}

						return true;
					}
				} else {
					if (showGizmo) {
						drawLine (headPosition, positionToLook, Color.green);
					}

					return true;
				}
			}
		}
		return false;
	}

	public void drawLine (Vector3 startPosition, Vector3 endPosition, Color color)
	{
		if (showGizmo) {
			Debug.DrawLine (startPosition, endPosition, color);
		}
	}

	void OnTriggerEnter (Collider other)
	{
		checkTriggerInfo (other.gameObject, true);
	}

	void OnTriggerExit (Collider other)
	{
		checkTriggerInfo (other.gameObject, false);
	}

	void checkTriggerInfo (GameObject objectToCheck, bool isEnter)
	{
		if (tagsToLocate.Contains (objectToCheck.tag)) {
			currentHeadTrack = objectToCheck.GetComponent<headTrack> ();

			if (currentHeadTrack != null) {
				if (!currentHeadTrack.isHeadTrackEnabled ()) {
					return;
				}

				if (isEnter) {

					currentHeadTrack.checkHeadTrackTarget (this);

					if (storeHeadTrackFound) {
						if (!headTrackFoundList.Contains (currentHeadTrack)) {
							headTrackFoundList.Add (currentHeadTrack);
						}
					}
				} else {

					currentHeadTrack.removeHeadTrackTarget (this);

					if (storeHeadTrackFound) {
						if (headTrackFoundList.Contains (currentHeadTrack)) {
							headTrackFoundList.Remove (currentHeadTrack);
						}
					
					}
				}
			}
		}
	}

	public void setEnableState (bool state)
	{
		targetEnabled = state;
	}

	public void disableState ()
	{
		setEnableState (false);
	}

	public void addTagToLocate (string tagToAdd)
	{
		if (!tagsToLocate.Contains (tagToAdd)) {
			tagsToLocate.Add (tagToAdd);

			getMainCollider ();

			if (mainCollider != null) {
				mainCollider.enabled = false;

				mainCollider.enabled = true;
			}
		}
	}

	public void removeTagToLocate (string tagToAdd)
	{
		if (tagsToLocate.Contains (tagToAdd)) {

			if (storeHeadTrackFound) {

				for (int i = headTrackFoundList.Count - 1; i >= 0; i--) {
		
					if (headTrackFoundList [i].gameObject.CompareTag (tagToAdd)) {

						headTrackFoundList [i].removeHeadTrackTarget (this);
	
						headTrackFoundList.RemoveAt (i);
					}
				}
			}

			tagsToLocate.Remove (tagToAdd);
		}
	}

	public void removeAllHeadTracksFoundAndDisableHeadTrack ()
	{
		for (int i = 0; i < headTrackFoundList.Count; i++) {

			headTrackFoundList [i].removeHeadTrackTarget (this);
		}

		headTrackFoundList.Clear ();

		getMainCollider ();

		if (mainCollider != null) {
			mainCollider.enabled = false;
		}

		targetEnabled = false;
	}

	public void enableHeadTrack ()
	{
		targetEnabled = true;

		getMainCollider ();

		if (mainCollider != null) {
			mainCollider.enabled = true;
		}
	}

	void getMainCollider ()
	{
		if (mainCollider == null) {
			mainCollider = GetComponent<Collider> ();
		}
	}

	void OnDrawGizmos ()
	{
		if (!showGizmo) {
			return;
		}

		if (GKC_Utils.isCurrentSelectionActiveGameObject (gameObject)) {
			DrawGizmos ();
		}
	}

	void OnDrawGizmosSelected ()
	{
		DrawGizmos ();
	}

	void DrawGizmos ()
	{
		if (showGizmo && targetEnabled) {
			Gizmos.color = gizmoColor;
			Vector3 position = getLookPositon ();
			Gizmos.DrawWireSphere (position, minDistanceToLook);
			Gizmos.color = Color.red;
			Gizmos.DrawSphere (position, 0.2f);
		}
	}
}