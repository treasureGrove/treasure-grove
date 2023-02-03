using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class simpleSwitch : MonoBehaviour
{
	public bool buttonEnabled = true;

	public AudioClip pressSound;
	public bool sendCurrentUser;
	public bool notUsableWhileAnimationIsPlaying = true;
	public bool useSingleSwitch = true;

	public bool buttonUsesAnimation = true;
	public string switchAnimationName = "simpleSwitch";

	public float animationSpeed = 1;

	public bool useUnityEvents = true;
	public UnityEvent objectToCallFunctions = new UnityEvent ();

	public UnityEvent turnOnEvent = new UnityEvent ();
	public UnityEvent turnOffEvent = new UnityEvent ();

	public GameObject objectToActive;
	public string activeFunctionName;
	public bool sendThisButton;

	public bool switchTurnedOn;

	public AudioSource audioSource;
	public Animation buttonAnimation;
	public deviceStringAction deviceStringActionManager;

	GameObject currentPlayer;
	bool firstAnimationPlay = true;

	void Start ()
	{
		if (audioSource == null) {
			audioSource = GetComponent<AudioSource> ();
		}

		if (buttonAnimation == null) {
			if (buttonUsesAnimation && switchAnimationName != "") {
				buttonAnimation = GetComponent<Animation> ();
			}
		}

		if (deviceStringActionManager == null) {
			deviceStringActionManager = GetComponent<deviceStringAction> ();
		}
	}

	public void setCurrentPlayer (GameObject newPlayer)
	{
		currentPlayer = newPlayer;
	}

	public void setCurrentUser (GameObject newPlayer)
	{
		currentPlayer = newPlayer;
	}

	public void turnSwitchOff ()
	{
		if (switchTurnedOn) {
			activateDevice ();
		}
	}

	public void turnSwitchOn ()
	{
		if (!switchTurnedOn) {
			activateDevice ();
		}
	}

	public void activateDevice ()
	{
		if (!buttonEnabled) {
			return;
		}

		bool canUseButton = false;

		if (buttonUsesAnimation) {
			if (buttonAnimation != null) {
				if ((!buttonAnimation.IsPlaying (switchAnimationName) && notUsableWhileAnimationIsPlaying)
				    || !notUsableWhileAnimationIsPlaying) {
					canUseButton = true;
				}
			}
		} else {
			canUseButton = true;
		}

		//check if the player is inside the trigger, and if he press the button to activate the devide
		if (canUseButton) {
			if (useSingleSwitch) {
				playSingleAnimation ();
			} else {
				switchTurnedOn = !switchTurnedOn;

				playDualAnimation (switchTurnedOn);

				setDeviceStringActionState (switchTurnedOn);
			}

			if (sendCurrentUser && currentPlayer != null) {
				objectToActive.SendMessage ("setCurrentUser", currentPlayer, SendMessageOptions.DontRequireReceiver);
			}

			if (useUnityEvents) {
				if (useSingleSwitch) {
					objectToCallFunctions.Invoke ();
				} else {
					if (switchTurnedOn) {
						turnOnEvent.Invoke ();
					} else {
						turnOffEvent.Invoke ();
					}
				}
			} else {
				if (objectToActive) {
					if (sendThisButton) {
						objectToActive.SendMessage (activeFunctionName, gameObject, SendMessageOptions.DontRequireReceiver);
					} else {
						objectToActive.SendMessage (activeFunctionName, SendMessageOptions.DontRequireReceiver);
					}
				}
			}
		}
	}

	public void setButtonEnabledState (bool state)
	{
		buttonEnabled = state;
	}

	public void triggerButtonEventFromEditor ()
	{
		activateDevice ();
	}

	public void playSingleAnimation ()
	{
		if (buttonUsesAnimation) {
			buttonAnimation [switchAnimationName].speed = animationSpeed;
			buttonAnimation.Play (switchAnimationName);
		}

		if (audioSource != null) {
			audioSource.PlayOneShot (pressSound);
		}
	}

	public void playDualAnimation (bool playForward)
	{
		if (buttonUsesAnimation) {
			if (playForward) {
				if (!buttonAnimation.IsPlaying (switchAnimationName)) {
					buttonAnimation [switchAnimationName].normalizedTime = 0;
				}

				buttonAnimation [switchAnimationName].speed = 1;
			} else {
				if (!buttonAnimation.IsPlaying (switchAnimationName)) {
					buttonAnimation [switchAnimationName].normalizedTime = 1;
				}

				buttonAnimation [switchAnimationName].speed = -1; 
			}

			if (firstAnimationPlay) {
				buttonAnimation.Play (switchAnimationName);
				firstAnimationPlay = false;
			} else {
				buttonAnimation.CrossFade (switchAnimationName);
			}
		}

		if (audioSource != null) {
			audioSource.PlayOneShot (pressSound);
		}
	}

	public void setDeviceStringActionState (bool state)
	{
		if (deviceStringActionManager != null) {
			deviceStringActionManager.changeActionName (state);
		}
	}
}