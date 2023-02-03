using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pauseAnimationSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool pauseAnimationEnabled = true;

	public bool pauseAnimatorStateInsteadOfReduceFrames;

	[Range (1, 60)] public int numberOfFrames = 1;

	[Space]
	[Header ("Transform List Settings")]
	[Space]

	public List<Transform> transformList = new List<Transform> ();

	[Space]
	[Header ("Debug")]
	[Space]

	public bool pauseAnimationActive;

	[Space]
	[Header ("Components")]
	[Space]

	public playerController mainPlayerController;
	public Animator animator;

	private Dictionary<int, Snapshot> snapshots = new Dictionary<int, Snapshot> ();
	private float updateTime = 0f;

	Coroutine pauseCoroutine;

	private void LateUpdate ()
	{
		if (pauseAnimationActive) {
			if (Time.time - this.updateTime > 1f / this.numberOfFrames) {
				this.SaveSnapshot ();
				this.updateTime = Time.time;
			}

			foreach (KeyValuePair<int, Snapshot> item in this.snapshots) {
				if (item.Value.transform != null) {
					item.Value.transform.localPosition = item.Value.localPosition;
					item.Value.transform.localRotation = item.Value.localRotation;
				}
			}
		}
	}

	private void SaveSnapshot ()
	{
		for (int i = 0; i < transformList.Count; ++i) {
			Transform target = transformList [i];
			int uid = target.GetInstanceID ();

			this.snapshots [uid] = new Snapshot (target);
		}
	}

	public void setPauseAnimationActiveState (bool state)
	{
		if (state && !pauseAnimationEnabled) {
			return;
		}

		pauseAnimationActive = state;
	}

	public void activatPauseAnimationXSeconds (float newDuration)
	{
		if (pauseCoroutine != null) {
			StopCoroutine (pauseCoroutine);
		}

		if (!pauseAnimationEnabled) {
			return;
		}

		pauseCoroutine = StartCoroutine (activatPauseAnimationXSecondsCoroutine (newDuration));
	}

	IEnumerator activatPauseAnimationXSecondsCoroutine (float newDuration)
	{
		bool pauseActivatedOnThisAction = false;

		if (pauseAnimatorStateInsteadOfReduceFrames) {
			if (!mainPlayerController.overrideAnimationSpeedActive) {
				mainPlayerController.setOverrideAnimationSpeedActiveState (true);
				mainPlayerController.setReducedVelocity (0);

				pauseActivatedOnThisAction = true;
			}
		} else {
			pauseAnimationActive = true;
		}

		yield return new WaitForSeconds (newDuration);

		if (pauseAnimatorStateInsteadOfReduceFrames) {
			if (pauseActivatedOnThisAction || mainPlayerController.getAnimSpeedMultiplier () == 0) {
				mainPlayerController.setOverrideAnimationSpeedActiveState (false);
				mainPlayerController.setReducedVelocity (1);
			}
		} else {
			pauseAnimationActive = false;
		}
	}

	public void setNumberOfFrames (int newValue)
	{
		numberOfFrames = newValue;
	}

	public void storeCharacterBones ()
	{
		transformList.Clear ();

		transformList.Add (animator.GetBoneTransform (HumanBodyBones.Head));
		transformList.Add (animator.GetBoneTransform (HumanBodyBones.Neck));
	
		transformList.Add (animator.GetBoneTransform (HumanBodyBones.Chest));
		transformList.Add (animator.GetBoneTransform (HumanBodyBones.Spine));

		transformList.Add (animator.GetBoneTransform (HumanBodyBones.Hips));

		transformList.Add (animator.GetBoneTransform (HumanBodyBones.RightLowerArm));
		transformList.Add (animator.GetBoneTransform (HumanBodyBones.LeftLowerArm));
		transformList.Add (animator.GetBoneTransform (HumanBodyBones.RightHand));
		transformList.Add (animator.GetBoneTransform (HumanBodyBones.LeftHand));

		transformList.Add (animator.GetBoneTransform (HumanBodyBones.RightLowerLeg));
		transformList.Add (animator.GetBoneTransform (HumanBodyBones.LeftLowerLeg));
		transformList.Add (animator.GetBoneTransform (HumanBodyBones.RightFoot));
		transformList.Add (animator.GetBoneTransform (HumanBodyBones.LeftFoot));
		transformList.Add (animator.GetBoneTransform (HumanBodyBones.RightToes));
		transformList.Add (animator.GetBoneTransform (HumanBodyBones.LeftToes));

		updateComponent ();
	}

	void updateComponent ()
	{
		GKC_Utils.updateComponent (this);

		GKC_Utils.updateDirtyScene ("Update Pause Animation System transform list", gameObject);
	}

	[System.Serializable]
	public class Snapshot
	{
		public Transform transform;
		public Vector3 localPosition;
		public Quaternion localRotation;

		public Snapshot (Transform transform)
		{
			this.transform = transform;
			this.Update ();
		}

		public void Update ()
		{
			this.localPosition = this.transform.localPosition;
			this.localRotation = this.transform.localRotation;
		}
	}
}