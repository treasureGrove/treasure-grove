using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class oxygenSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool oxygenSystemEnabled = true;

	public float oxygenUseRate;

	public Color regularOxygenColor;
	public Color closeToEmptyOxygenColor;
	public Color regularOxygenTextColor;
	public Color closeToEmptyOxygenTextColor;

	public float closeToEmptyOxygenAmount;

	public float oxygenAmount;

	public float refillOxygenRate = 18;
	public float refillOxygenDelayAfterUse;

	public bool oxygenObjectAlwaysActive;
	public float timeToDisableOxygenObject;

	public bool waitToOxygenFilledToUseAgain;

	public float eventOnOxygenDepletedRate;

	public bool useOxygenUIOnScreenOnAnyView;

	[Space]
	[Header ("Oxygen States Settings")]
	[Space]

	public List<oxygenStateInfo> oxygenStateInfoList = new List<oxygenStateInfo> ();

	public string oxygenStateNameByDefault = "Oxygen On Space";

	[Space]
	[Header ("Debug")]
	[Space]

	public bool oxygenEmpty;
	public bool refillingOxygen;
	public bool usingOxygen;
	public bool oxygenRefilled = true;
	public bool firstPersonActive;

	public oxygenStateInfo currentOxygenStateInfo;

	[Space]
	[Header ("Event Settings")]
	[Space]

	public UnityEvent eventOnOxygenRefilled;
	public UnityEvent eventOnOxygenEmpty;
	public UnityEvent eventOnOxygenDepleted;

	public UnityEvent eventOnOxygenActive;
	public UnityEvent eventOnOxygenDeactivate;

	public UnityEvent eventOnOxygenEmptyDeath;

	[Space]
	[Header ("Components")]
	[Space]

	public Image oxygenSlider;

	public Text oxygenAmountText;

	public GameObject oxygenObject;

	public GameObject firstPersonOxygenPanel;
	public Text firstPersonOxygenAmountText;

	public health mainHealth;

	float originalOxygenAmount;

	float lastTimeOxygenRefilled;

	Coroutine refillCoroutine;

	bool oxygenSliderHidden;

	float lastTimeOxygenDepleted;

	float auxOxygenAmount;

	float lastTimeOxygenSoundPlayed;
	float curretnOxygenSoundRate;

	float lastTimeOxygenStateDepleted;


	void Start ()
	{
		originalOxygenAmount = oxygenAmount;
		auxOxygenAmount = oxygenAmount;

		if (oxygenObject.activeSelf) {
			oxygenObject.SetActive (false);
		}

		setOxygenStateByName (oxygenStateNameByDefault);
	}

	void FixedUpdate ()
	{
		if (!oxygenSystemEnabled) {
			return;
		}

		if (!oxygenObjectAlwaysActive) {

			if (!refillingOxygen) {
				if (oxygenRefilled && !oxygenSliderHidden) {
					if (Time.time > lastTimeOxygenRefilled + timeToDisableOxygenObject) {
						if (!oxygenSliderHidden) {
							if (oxygenObject.activeSelf) {
								oxygenObject.SetActive (false);
							}

							oxygenSliderHidden = true;
						}
					}
				}
			}
		}

		if (!refillingOxygen && usingOxygen) {

			if (mainHealth.isDead ()) {
				disableOxygenOnDeath ();
			}

			if (useOxygenUIOnScreenOnAnyView) {
				if (!firstPersonOxygenPanel.activeSelf) {
					firstPersonOxygenPanel.SetActive (true);
				}
			} else {
				if (firstPersonActive) {
					if (oxygenObject.activeSelf) {
						oxygenObject.SetActive (false);
					}
				} else {
					if (!oxygenObject.activeSelf) {
						oxygenObject.SetActive (true);
					}
				}
			}
		
			oxygenSliderHidden = false;

			oxygenAmount -= oxygenUseRate * Time.fixedDeltaTime;

			oxygenAmount = Mathf.Clamp (oxygenAmount, 0, originalOxygenAmount);

			if (oxygenAmount <= closeToEmptyOxygenAmount) {
				oxygenSlider.color = closeToEmptyOxygenColor;
				oxygenAmountText.color = closeToEmptyOxygenTextColor;
			}

			oxygenSlider.fillAmount = oxygenAmount / originalOxygenAmount;

			if (oxygenAmountText) {
				string oxygenAmountString = oxygenAmount.ToString ("##.0");

				if (oxygenAmount == 0) {
					oxygenAmountString = "0";
				}

				oxygenAmountText.text = oxygenAmountString;

				if (useOxygenUIOnScreenOnAnyView) {
					if (!firstPersonOxygenPanel.activeSelf) {
						firstPersonOxygenPanel.SetActive (true);
					}

					firstPersonOxygenAmountText.text = oxygenAmountString;
				} else {
					if (firstPersonActive) {
						if (!firstPersonOxygenPanel.activeSelf) {
							firstPersonOxygenPanel.SetActive (true);
						}

						firstPersonOxygenAmountText.text = oxygenAmountString;
					} else {
						if (firstPersonOxygenPanel.activeSelf) {
							firstPersonOxygenPanel.SetActive (false);
						}
					}
				}
			}

			auxOxygenAmount = oxygenAmount;

			if (oxygenAmount <= 0) {

				if (!oxygenEmpty) {
					eventOnOxygenEmpty.Invoke ();

					currentOxygenStateInfo.eventOnOxygenEmpty.Invoke ();
				}

				oxygenEmpty = true;

				if (Time.time > eventOnOxygenDepletedRate + lastTimeOxygenDepleted) {
					eventOnOxygenDepleted.Invoke ();

					lastTimeOxygenDepleted = Time.time;
				}

				if (currentOxygenStateInfo.killPlayerOnOxygenEmpty) {
					mainHealth.killCharacter ();
				} else {
					if (Time.time > currentOxygenStateInfo.damagePlayerOnOxygenEmptyRate + lastTimeOxygenStateDepleted) {
						mainHealth.takeConstantDamageWithoutShield (currentOxygenStateInfo.damagePlayerOnOxygenEmptyAmount);
					
						lastTimeOxygenStateDepleted = Time.time;
					}
				}
			}

			if (currentOxygenStateInfo.useOxygenSound) {
				if (oxygenAmount > currentOxygenStateInfo.oxygenAmountToIncreaseSoundRate) {
					curretnOxygenSoundRate = currentOxygenStateInfo.oxygenSoundRate;
				} else {
					curretnOxygenSoundRate = currentOxygenStateInfo.oxygenSoundIncreasedRate;
				}

				if (Time.time > lastTimeOxygenSoundPlayed + curretnOxygenSoundRate) {
					lastTimeOxygenSoundPlayed = Time.time;

					if (currentOxygenStateInfo.oxygenSoundSource != null) {
						currentOxygenStateInfo.oxygenSoundSource.PlayOneShot (currentOxygenStateInfo.clipToUseOnOxygenSound);
					}
				}
			}
		}
	}

	void setOxygenStateByName (string oxygenStateName)
	{
		for (int i = 0; i < oxygenStateInfoList.Count; i++) {
			if (oxygenStateInfoList [i].stateEnabled && oxygenStateInfoList [i].Name.Equals (oxygenStateName)) {
				currentOxygenStateInfo = oxygenStateInfoList [i];
			}
		}
	}

	public void activateOxygenState (string oxygenStateName)
	{
		setOxygenStateByName (oxygenStateName);

		activeOxygenState ();
	}

	public void disableOxygenState (string oxygenStateName)
	{
		setOxygenStateByName (oxygenStateName);

		disableOxygenState ();
	}

	public void activeOxygenState ()
	{
		if (!oxygenSystemEnabled) {
			return;
		}

		if (usingOxygen) {
			return;
		}

		if (oxygenEmpty && (waitToOxygenFilledToUseAgain || oxygenAmount <= 0)) {
			return;
		}

		stopRefillCoroutine ();

		refillingOxygen = false;

		oxygenRefilled = false;

		usingOxygen = true;

		eventOnOxygenActive.Invoke ();

		currentOxygenStateInfo.eventOnOxygenActive.Invoke ();
	}

	public void disableOxygenState ()
	{
		if (!oxygenSystemEnabled) {
			return;
		}

		if (!usingOxygen) {
			return;
		}

		lastTimeOxygenDepleted = 0;

		lastTimeOxygenStateDepleted = 0;

		if (usingOxygen) {
			refillOxygen (refillOxygenDelayAfterUse);
		}

		usingOxygen = false;

		eventOnOxygenDeactivate.Invoke ();

		if (firstPersonOxygenPanel.activeSelf) {
			firstPersonOxygenPanel.SetActive (false);
		}

		currentOxygenStateInfo.eventOnOxygenDeactivate.Invoke ();
	}

	public void refillOxygen (float refillDelay)
	{
		stopRefillCoroutine ();

		refillCoroutine = StartCoroutine (refillOxygenCoroutine (refillDelay));
	}

	public void stopRefillCoroutine ()
	{
		if (refillCoroutine != null) {
			StopCoroutine (refillCoroutine);
		}
	}

	IEnumerator refillOxygenCoroutine (float refillDelay)
	{
		refillingOxygen = true;

		yield return new WaitForSeconds (refillDelay);

		if (!waitToOxygenFilledToUseAgain) {
			eventOnOxygenRefilled.Invoke ();

			currentOxygenStateInfo.eventOnOxygenRefilled.Invoke ();
		}

		while (oxygenAmount < originalOxygenAmount) {
			oxygenAmount += Time.deltaTime * refillOxygenRate;

			updateOxygenState ();

			yield return null;
		}

		if (waitToOxygenFilledToUseAgain) {
			eventOnOxygenRefilled.Invoke ();

			currentOxygenStateInfo.eventOnOxygenRefilled.Invoke ();
		}

		refillingOxygen = false;

		lastTimeOxygenRefilled = Time.time;

		oxygenRefilled = true;

		oxygenEmpty = false;
	}

	public void increaseTotalOxygenAmount (float extraValue)
	{
		oxygenAmount = originalOxygenAmount + extraValue;

		originalOxygenAmount = oxygenAmount;

		auxOxygenAmount = oxygenAmount;
	}

	public void initializeOxygenAmount (float newValue)
	{
		oxygenAmount = newValue;
	}

	public void initializeOxygenUseRate (float newValue)
	{
		oxygenUseRate = newValue;
	}

	public void increaseOxygenUseRate (float newValue)
	{
		oxygenUseRate += newValue;
	}

	public void getOxygen (float amount)
	{
		if (!waitToOxygenFilledToUseAgain) {
			eventOnOxygenRefilled.Invoke ();

			currentOxygenStateInfo.eventOnOxygenRefilled.Invoke ();
		}

		oxygenAmount += amount;

		updateOxygenState ();

		if (waitToOxygenFilledToUseAgain) {
			eventOnOxygenRefilled.Invoke ();

			currentOxygenStateInfo.eventOnOxygenRefilled.Invoke ();
		}

		oxygenEmpty = false;
	}

	void updateOxygenState ()
	{
		oxygenSlider.fillAmount = oxygenAmount / originalOxygenAmount;

		if (oxygenAmountText) {
			oxygenAmountText.text = Mathf.RoundToInt (oxygenAmount).ToString ();
		}

		if (oxygenAmount > closeToEmptyOxygenAmount) {
			oxygenSlider.color = regularOxygenColor;
			oxygenAmountText.color = regularOxygenTextColor;
		}

		if (oxygenAmount >= originalOxygenAmount) {
			oxygenAmount = originalOxygenAmount;
		}

		auxOxygenAmount = oxygenAmount;
	}

	public float getOxygenAmountToLimit ()
	{
		return originalOxygenAmount - auxOxygenAmount;
	}

	public void addAuxOxygenAmount (float amount)
	{
		auxOxygenAmount += amount;
	}

	public void disableOxygenOnDeath ()
	{
		lastTimeOxygenDepleted = 0;

		lastTimeOxygenStateDepleted = 0;

		usingOxygen = false;

		eventOnOxygenDeactivate.Invoke ();

		currentOxygenStateInfo.eventOnOxygenDeactivate.Invoke ();

		eventOnOxygenEmptyDeath.Invoke ();
	}

	public void setFirstPersonActiveState (bool state)
	{
		firstPersonActive = state;
	}

	public void setOxygenSystemEnabledState (bool state)
	{
		oxygenSystemEnabled = state;
	}

	[System.Serializable]
	public class oxygenStateInfo
	{
		[Header ("Main Settings")]
		[Space]

		public string Name;
		public bool stateEnabled = true;

		public float damagePlayerOnOxygenEmptyRate;
		public float damagePlayerOnOxygenEmptyAmount;
		public bool killPlayerOnOxygenEmpty;
	
		[Space]
		[Header ("Events Settings")]
		[Space]

		public UnityEvent eventOnOxygenActive;
		public UnityEvent eventOnOxygenDeactivate;

		public UnityEvent eventOnOxygenEmpty;
		public UnityEvent eventOnOxygenRefilled;

		[Space]
		[Header ("Sounds Settings")]
		[Space]

		public bool useOxygenSound = true;
		public float oxygenSoundRate = 2;
		public float oxygenAmountToIncreaseSoundRate = 15;
		public float oxygenSoundIncreasedRate = 1;
		public AudioSource oxygenSoundSource;
		public AudioClip clipToUseOnOxygenSound;
	}
}
