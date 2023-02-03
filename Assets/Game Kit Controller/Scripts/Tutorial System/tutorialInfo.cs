using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE_OSX
using UnityEngine.Video;
#endif

[System.Serializable]
public class tutorialInfo
{
	public string Name;
	public GameObject panelGameObject;
	public List<panelInfo> tutorialPanelList = new List<panelInfo> ();
	public bool unlockCursorOnTutorialActive;
	public bool useActionButtonToMoveThroughTutorial;
	public bool pressAnyButtonToNextTutorial;
	public float timeToEnableKeys = 1;

	public float openTutorialDelay;

	public bool setCustomTimeScale;
	public float customTimeScale;
	public bool useSoundOnTutorialOpen;
	public AudioClip soundOnTutorialOpen;

	public bool playTutorialOnlyOnce;
	public bool tutorialPlayed;

	#if  UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE_OSX
	public bool containsVideo;
	public List<videoInfo> videoInfoList = new List<videoInfo> ();
	public bool setNextPanelWhenVideoEnds;
	public int currentVideoIndex;
	#endif
}

#if  UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE_OSX
[System.Serializable]
public class videoInfo
{
	public string Name = "New Video";
	public RawImage videoRawImage;
	public VideoClip videoFile;

	public bool useVideoAudio = true;
	public float videoAudioVolume = 1;

	public bool loopVideo = true;

	public Coroutine currentTutorialVideoCoroutine;
}
#endif

[System.Serializable]
public class panelInfo
{
	public string Name;
	public GameObject panelGameObject;
}