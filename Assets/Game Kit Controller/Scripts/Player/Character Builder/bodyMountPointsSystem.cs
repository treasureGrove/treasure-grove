using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bodyMountPointsSystem : MonoBehaviour
{
	[Header ("Main Setting")]
	[Space]

	public bool useBoneTransformToLocateBone = true;

	public List<bodyMountPointInfo> bodyMountPointInfoList = new List<bodyMountPointInfo> ();
	public Animator mainAnimator;

	public GameObject mountPointObjectReferencePrefab;

	[Space]
	[Header ("Editor Setting")]
	[Space]

	public string mountPointToEditName;

	public Vector3 mountPointInitialPositionOffset = new Vector3 (-0.008f, -0.1237f, -0.0205f);
	public Vector3 mountPointInitialEulerOffset = new Vector3 (47.453f, 0, -15.026f);

	public bool showGizmo;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool editingMountPoint;

	public Transform temporalMountPointTransform;

	public GameObject temporalMountPointObjectReference;


	public void setCharacterBodyMountPointsInfoList ()
	{
		for (int i = 0; i < bodyMountPointInfoList.Count; i++) {

			bodyMountPointInfo currentBodyMountPointInfo = bodyMountPointInfoList [i];

			Transform currentBone = mainAnimator.GetBoneTransform (currentBodyMountPointInfo.boneToAttach);

			if (currentBone == null) {
				currentBone = mainAnimator.GetBoneTransform (currentBodyMountPointInfo.alternativeBoneToAttach);
			}

			if (currentBone == null) {
				currentBone = mainAnimator.GetBoneTransform (HumanBodyBones.Hips);

				if (currentBone == null) {
					currentBone = mainAnimator.transform;

					print ("WARNING: No bone found on character body elements list for " + currentBodyMountPointInfo.Name + "" +
					" setting that character element inside the character model " + currentBone.name);
				}
			}

			if (!useBoneTransformToLocateBone) {
				if (currentBone == null && mainAnimator != null) {
					currentBone = mainAnimator.transform;
				}
			}

			if (currentBone != null) {
				for (int j = 0; j < currentBodyMountPointInfo.objectPointInfoList.Count; j++) {
					objectPointInfo currentObjectPointInfo = currentBodyMountPointInfo.objectPointInfoList [j];

					if (currentObjectPointInfo.objectTransform != null) {
						Vector3 targetPosition = Vector3.zero;
						Vector3 targerEuler = Vector3.zero;

						if (currentObjectPointInfo.setPreviousLocalValues) {
							targetPosition = currentObjectPointInfo.objectTransform.localPosition;
							targerEuler = currentObjectPointInfo.objectTransform.localEulerAngles;
						}

						currentObjectPointInfo.objectTransform.SetParent (currentBone);

						if (currentObjectPointInfo.setPreviousLocalValues) {
							currentObjectPointInfo.objectTransform.localPosition = targetPosition;
							currentObjectPointInfo.objectTransform.localEulerAngles = targerEuler;
						}

						if (currentObjectPointInfo.usePositionOffset) {
							currentObjectPointInfo.objectTransform.localPosition = currentObjectPointInfo.positionOffset;
						}

						if (currentObjectPointInfo.useEulerOffset) {
							currentObjectPointInfo.objectTransform.localEulerAngles = currentObjectPointInfo.eulerOffset;
						}
					}
				}
			}
		}
	}

	public Transform getMountPointTransformByName (string mountPointName)
	{
		for (int i = 0; i < bodyMountPointInfoList.Count; i++) {

			bodyMountPointInfo currentBodyMountPointInfo = bodyMountPointInfoList [i];

			if (currentBodyMountPointInfo.Name.Equals (mountPointName)) {
				return currentBodyMountPointInfo.objectPointInfoList [0].objectTransform;
			}
		}

		return null;
	}

	public void toggleShowHandleGizmo ()
	{
		showGizmo = !showGizmo;
	}

	public void toggleEditMountPoint ()
	{
		editingMountPoint = !editingMountPoint;

		if (editingMountPoint) {
			int mountPointIndex = bodyMountPointInfoList.FindIndex (s => s.Name.ToLower () == mountPointToEditName.ToLower ());

			if (mountPointIndex > -1) {
				bodyMountPointInfo temporalBodyMountPointInfo = bodyMountPointInfoList [mountPointIndex];

				temporalMountPointTransform = temporalBodyMountPointInfo.objectPointInfoList [0].objectTransform;

				if (temporalMountPointObjectReference == null) {
					temporalMountPointObjectReference = (GameObject)Instantiate (mountPointObjectReferencePrefab, Vector3.zero, Quaternion.identity, temporalMountPointTransform);
				}

				temporalMountPointObjectReference.transform.SetParent (temporalMountPointTransform);

				temporalMountPointObjectReference.transform.localPosition = mountPointInitialPositionOffset;

				temporalMountPointObjectReference.transform.localEulerAngles = mountPointInitialEulerOffset;


			} else {
				print ("No mount point found");
			}
		} else {
			if (temporalMountPointObjectReference != null) {
				DestroyImmediate (temporalMountPointObjectReference);
			}

			temporalMountPointTransform = null;
		}
	}

	public void setNewAnimator (Animator newAnimator)
	{
		mainAnimator = newAnimator;

		updateComponent ();
	}

	void updateComponent ()
	{
		GKC_Utils.updateComponent (this);
		GKC_Utils.updateDirtyScene ("Update Body Mount Point System", gameObject);
	}

	[System.Serializable]
	public class bodyMountPointInfo
	{
		public string Name;

		public HumanBodyBones boneToAttach;
		public HumanBodyBones alternativeBoneToAttach;

		[Space]

		public List<objectPointInfo> objectPointInfoList = new List<objectPointInfo> ();
	}

	[System.Serializable]
	public class objectPointInfo
	{
		public Transform objectTransform;

		public bool usePositionOffset;
		public Vector3 positionOffset;

		public bool useEulerOffset;
		public Vector3 eulerOffset;

		public bool setPreviousLocalValues;
	}
}
