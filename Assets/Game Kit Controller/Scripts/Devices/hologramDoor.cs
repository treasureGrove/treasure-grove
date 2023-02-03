using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;

public class hologramDoor : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public string unlockedText;
	public string lockedText;
	public string openText;
	public string hologramIdle;
	public string hologramInside;

	public float fadeHologramSpeed = 4;

	public float openDelay;
	public Color lockedColor;
	public GameObject doorToOpen;
	public bool openOnTrigger;

	public List<string> tagListToOpen = new List<string> ();

	[Space]
	[Header ("Sounds Settings")]
	[Space]

	public AudioClip enterSound;
	public AudioClip exitSound;
	public AudioClip lockedSound;
	public AudioClip openSound;

	[Space]
	[Header ("Event Settings")]
	[Space]

	public bool useEventOnOpen;
	public UnityEvent eventOnOpen;

	[Space]
	[Header ("Hologram Elements")]
	[Space]

	public List<Text> hologramText = new List<Text> ();
	public List<GameObject> holograms = new List<GameObject> ();
	public List<GameObject> hologramCentralRing = new List<GameObject> ();

	[Space]
	[Header ("Components")]
	[Space]

	public doorSystem doorManager;
	public AudioSource audioSource;

	List<Image> otherHologramParts = new List<Image> ();
	List<RawImage> hologramParts = new List<RawImage> ();
	List<Color> originalImageColors = new List<Color> ();
	List<Color> originalRawImageColors = new List<Color> ();

	List<Animation> hologramsAnimations = new List<Animation> ();
	List<Animation> hologramsCentralRingAnimations = new List<Animation> ();

	bool insidePlayed;
	public bool doorLocked;
	bool inside;
	bool openingDoor;
	bool hologramOccupied;
	string regularStateText;

	Color newColor;

	Coroutine openDoorCoroutine;
	Coroutine changeTransparencyCoroutine;
	Coroutine setHologramColorsCoroutine;

	bool changingColors;


	void Start ()
	{
		//get the door system component of the door
		if (doorManager == null) {
			doorManager = doorToOpen.GetComponent<doorSystem> ();
		}

		//get all the raw images components in the hologram
		for (int i = 0; i < holograms.Count; i++) {
			Component[] hologramsParts = holograms [i].GetComponentsInChildren (typeof(RawImage));
			foreach (RawImage child in hologramsParts) {
				//store the raw images
				hologramParts.Add (child);

				//store the original color of every raw image
				originalRawImageColors.Add (child.color);

				//for every color, add a locked color
			}

			//store every animation component
			hologramsAnimations.Add (holograms [i].GetComponent<Animation> ());
		}

		for (int i = 0; i < hologramCentralRing.Count; i++) {
			hologramsCentralRingAnimations.Add (hologramCentralRing [i].GetComponent<Animation> ());
		}

		for (int i = 0; i < holograms.Count; i++) {
			//get the image components in the hologram
			Component[] hologramsParts = holograms [i].GetComponentsInChildren (typeof(Image));
			foreach (Image child in hologramsParts) {
				//store every component in the correct list
				otherHologramParts.Add (child);
				originalImageColors.Add (child.color);
			}
		}

		//check if the door that uses the hologram is locked or not, to set the text info in the door
		string newText = "";

		if (doorManager.locked) {
			doorLocked = true;
			newText = lockedText;

			for (int i = 0; i < hologramParts.Count; i++) {
				hologramParts [i].color = lockedColor;
			}
		} else {
			newText = unlockedText;
		}

		regularStateText = newText;

		//set the text in the hologram
		setHologramText (regularStateText);
	
		if (audioSource == null) {
			audioSource = GetComponent<AudioSource> ();
		}
			
		if (doorManager.isDoorOpened ()) {
			setHologramTransparencyAtOnce (true);
		}
	}

	void Update ()
	{
		//if the player is not inside, play the normal rotating animation
		if (!inside) {
			for (int i = 0; i < hologramsAnimations.Count; i++) {
				if (!hologramsAnimations [i].IsPlaying (hologramIdle)) {
					hologramsAnimations [i].Play (hologramIdle);
				}
			}
		} 

		//if the player is inside the trigger, play the open? animation of the hologram and stop the rotating animation
		if (inside && !insidePlayed) {
			for (int i = 0; i < hologramsAnimations.Count; i++) {
				hologramsAnimations [i].Stop ();
			}

			for (int i = 0; i < hologramsCentralRingAnimations.Count; i++) {
				hologramsCentralRingAnimations [i] [hologramInside].speed = 1;
				hologramsCentralRingAnimations [i].Play (hologramInside);
			}

			insidePlayed = true;
		}

		//if the door has been opened, and now it is closed and the player is not inside the trigger, set the alpha color of the hologram to its regular state
		if (openingDoor && doorManager.doorState == doorSystem.doorCurrentState.closed && !doorManager.doorIsMoving () && !inside) {
			openingDoor = false;
			startChangeTransparencyCoroutine (false);
		}
	}

	//if the player is inside the trigger and press the activate device button, check that the door is not locked and it is closed
	public void activateDevice ()
	{
		openCurrentDoor ();
	}

	public void openCurrentDoor ()
	{
		if (!doorLocked && doorManager.doorState == doorSystem.doorCurrentState.closed && !doorManager.doorIsMoving () && !hologramOccupied) {
			//fade the hologram colors and open the door
			audioSource.PlayOneShot (openSound);

			startChangeTransparencyCoroutine (true);

			startOpenDoorCoroutine ();

			if (useEventOnOpen) {
				eventOnOpen.Invoke ();
			}
		}
	}

	public void startChangeTransparencyCoroutine (bool state)
	{
		stopChangeTransparency ();

		stopSetHologramColors ();

		changeTransparencyCoroutine = StartCoroutine (changeTransparency (state));
	}

	public void stopChangeTransparency ()
	{
		if (changeTransparencyCoroutine != null) {
			StopCoroutine (changeTransparencyCoroutine);
		}
	}

	//this fades and turns back the alpha value of the colors in the hologram, according to if the door is opening or closing
	IEnumerator changeTransparency (bool state)
	{
		if (changingColors) {

			for (int i = 0; i < hologramParts.Count; i++) {
				hologramParts [i].color = originalRawImageColors [i];
			}

			changingColors = false;
		}

		hologramOccupied = true;

		int mult = 1;
		if (state) {
			mult = -1;
		} 

		Color alpha = new Color ();

		for (float t = 0; t < 1;) {
			t += Time.deltaTime * fadeHologramSpeed;

			for (int i = 0; i < hologramParts.Count; i++) {
				alpha = hologramParts [i].color;
				alpha.a += Time.deltaTime * mult * 3;

				alpha.a = Mathf.Clamp (alpha.a, 0, originalRawImageColors [i].a);

				hologramParts [i].color = alpha;
			}

			for (int i = 0; i < hologramText.Count; i++) {
				alpha = hologramText [i].color;
				alpha.a += Time.deltaTime * mult * 3;
				alpha.a = Mathf.Clamp01 (alpha.a);
				hologramText [i].color = alpha;
			}

			for (int i = 0; i < otherHologramParts.Count; i++) {
				alpha = otherHologramParts [i].color;
				alpha.a += Time.deltaTime * mult * 3;

				alpha.a = Mathf.Clamp (alpha.a, 0, originalImageColors [i].a);

				otherHologramParts [i].color = alpha;
			}

			yield return null;
		}

		hologramOccupied = false;
	}

	public void setHologramTransparencyAtOnce (bool state)
	{
		int mult = 1;
		if (state) {
			mult = -1;
		} 

		Color alpha = new Color ();

		for (int i = 0; i < hologramParts.Count; i++) {
			alpha = hologramParts [i].color;
			alpha.a = mult;
			hologramParts [i].color = alpha;
		}

		for (int i = 0; i < hologramText.Count; i++) {
			alpha = hologramText [i].color;
			alpha.a = mult;
			hologramText [i].color = alpha;
		}

		for (int i = 0; i < otherHologramParts.Count; i++) {
			alpha = otherHologramParts [i].color;
			alpha.a = mult;
			otherHologramParts [i].color = alpha;
		}
	}

	public void startSetHologramColorsCoroutine (bool useUnlockedColors)
	{
		stopSetHologramColors ();

		setHologramColorsCoroutine = StartCoroutine (setHologramColors (useUnlockedColors));
	}

	public void stopSetHologramColors ()
	{
		if (setHologramColorsCoroutine != null) {
			StopCoroutine (setHologramColorsCoroutine);
		}
	}

	//if the door is unlocked with a pass device or other way, change the locked colors in the hologram for the original unlocked colors
	IEnumerator setHologramColors (bool useUnlockedColors)
	{
		changingColors = true;

		Color alpha = new Color ();

		for (float t = 0; t < 1;) {
			t += Time.deltaTime * fadeHologramSpeed;

			for (int i = 0; i < hologramParts.Count; i++) {
				if (useUnlockedColors) {
					hologramParts [i].color = Color.Lerp (hologramParts [i].color, originalRawImageColors [i], t);
				} else {
					hologramParts [i].color = Color.Lerp (hologramParts [i].color, lockedColor, t);
				}
			}
				
			if (!hologramOccupied) {
				for (int i = 0; i < hologramText.Count; i++) {
					alpha = hologramText [i].color;
					alpha.a += t;
					alpha.a = Mathf.Clamp01 (alpha.a);
					hologramText [i].color = alpha;
				}
			}

			for (int i = 0; i < otherHologramParts.Count; i++) {
				if (useUnlockedColors) {
					otherHologramParts [i].color = Color.Lerp (otherHologramParts [i].color, originalImageColors [i], t);
				} else {
					otherHologramParts [i].color = Color.Lerp (otherHologramParts [i].color, lockedColor, t);
				}
			}
				
			yield return null;
		}

		changingColors = false;
	}

	//the door was locked and now it has been unlocked, to change the hologram colors
	public void unlockHologram ()
	{
		doorLocked = false;

		regularStateText = unlockedText;

		setHologramText (regularStateText);

		startSetHologramColorsCoroutine (true);
	}

	//the door was locked and now it has been unlocked, to change the hologram colors
	public void lockHologram ()
	{
		doorLocked = true;

		regularStateText = lockedText;

		setHologramText (regularStateText);

		startSetHologramColorsCoroutine (false);
	}

	public void startOpenDoorCoroutine ()
	{
		stopOpenDoor ();

		openDoorCoroutine = StartCoroutine (openDoor ());
	}

	public void stopOpenDoor ()
	{
		if (openDoorCoroutine != null) {
			StopCoroutine (openDoorCoroutine);
		}
	}

	//wait a delay and then open the door
	IEnumerator openDoor ()
	{
		yield return new WaitForSeconds (openDelay);

		doorManager.changeDoorsStateByButton ();

		openingDoor = true;
	}

	//chane the current text showed in the door, according to it is locked, unlocked or can be opened
	void setHologramText (string newState)
	{
		for (int i = 0; i < hologramText.Count; i++) {
			hologramText [i].text = newState;
		}
	}

	void OnTriggerEnter (Collider col)
	{
		checkTriggerInfo (col, true);
	}

	void OnTriggerExit (Collider col)
	{
		checkTriggerInfo (col, false);
	}

	public void checkTriggerInfo (Collider col, bool isEnter)
	{
		//if ((1 << col.gameObject.layer & layerForUsers.value) == 1 << col.gameObject.layer) {
		//}
		//if the player is entering in the trigger
		if (isEnter) {
			//if the player is inside the hologram trigger
			if (checkIfTagCanOpen (col.tag)) {
				enteringDoor ();
			}
		} else {
			//if the player exits the hologram trigger
			if (checkIfTagCanOpen (col.tag)) {
				exitingDoor ();
			}
		}
	}

	public void enteringDoor ()
	{
		//if the door is unlocked, set the open? text in the hologram
		if (!doorLocked) {
			setHologramText (openText);
		}

		inside = true;

		//set an audio when the player enters in the hologram trigger
		if (!openingDoor && doorManager.doorState == doorSystem.doorCurrentState.closed && !doorManager.doorIsMoving ()) {
			if (doorLocked) {
				audioSource.PlayOneShot (lockedSound);
			} else {
				audioSource.PlayOneShot (enterSound);
				if (openOnTrigger) {
					openCurrentDoor ();
				}
			}
		}
	}

	public void exitingDoor ()
	{
		if (doorManager.isDisableDoorOpenCloseActionActive ()) {
			return;
		}

		//set the current state text in the hologram
		setHologramText (regularStateText);
		inside = false;

		//stop the central ring animation and play it reverse and start the rotating animation again
		if (insidePlayed) {
			for (int i = 0; i < hologramsCentralRingAnimations.Count; i++) {
				hologramsCentralRingAnimations [i] [hologramInside].speed = -1; 
				hologramsCentralRingAnimations [i] [hologramInside].time = hologramsCentralRingAnimations [i] [hologramInside].length;
				hologramsCentralRingAnimations [i].Play (hologramInside);
			}

			for (int i = 0; i < hologramsAnimations.Count; i++) {
				hologramsAnimations [i] [hologramIdle].time = hologramsAnimations [i] [hologramIdle].length;
				hologramsAnimations [i].Play (hologramIdle);
			}
			insidePlayed = false;
		}

		if (!openingDoor) {
			audioSource.PlayOneShot (exitSound);
		}
	}

	public bool checkIfTagCanOpen (string tagToCheck)
	{
		if (tagListToOpen.Contains (tagToCheck)) {
			return true;
		}

		return false;
	}

	public void openHologramDoorByExternalInput ()
	{
		if (doorManager.doorState == doorSystem.doorCurrentState.closed) {
			bool previousOpenOnTriggerValue = openOnTrigger;
			openOnTrigger = true;

			enteringDoor ();

			openOnTrigger = previousOpenOnTriggerValue;
		}

		if (doorManager.doorState == doorSystem.doorCurrentState.opened) {
			exitingDoor ();

			doorManager.changeDoorsStateByButton ();
		}
	}
}
