using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tutorialUISystem : MonoBehaviour
{
	public List<tutorialInfo> tutorialInfoList = new List<tutorialInfo> ();

	public Transform tutorialsPanel;
	public AudioSource videoAudioSource;
	public Transform mainVideoPlayerPanel;

	public playerTutorialSystem mainPlayerTutorialSystem;

	public Transform getTutorialsPanel ()
	{
		return tutorialsPanel;
	}

	public AudioSource getVideoAudioSource ()
	{
		return videoAudioSource;
	}

	public Transform getMainVideoPlayerPanel ()
	{
		return mainVideoPlayerPanel;
	}

	public  List<tutorialInfo> getTutorialInfoList ()
	{
		return tutorialInfoList;
	}

	public void activateTutorialByName (string tutorialName)
	{
		if (mainPlayerTutorialSystem != null) {
			mainPlayerTutorialSystem.activatingTutorialByNameFromEditor ();
			mainPlayerTutorialSystem.activateTutorialByName (tutorialName);
		}
	}

	public void searchPlayerTutorialSystem ()
	{
		if (mainPlayerTutorialSystem == null) {
			mainPlayerTutorialSystem = FindObjectOfType<playerTutorialSystem> ();
		}

		if (mainPlayerTutorialSystem != null) {
			mainPlayerTutorialSystem.setTutorialUISystem (this);
		}
	
		updateComponent ();
	}

	public void updateComponent ()
	{
		GKC_Utils.updateComponent (this);
	}
}