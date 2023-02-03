using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class staminaSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool useStaminaEnabled = true;

	public Color regularStaminaColor;
	public Color closeToEmptyStaminaColor;

	public float closeToEmptyStaminaAmount;

	public float staminaAmount;

	public float refillStaminaRate = 18;
	public float refillStaminaDelayAfterUse;
	public float refillStaminaDelayAfterEmpty;

	public bool staminaObjectAlwaysActive;
	public float timeToDisableStaminaObject;

	public bool waitToStaminaFilledToUseAgain;

	public bool useFade;
	public float fadeSpeed;
	public float enabledAlphaValue;

	public bool useSliderCircle = true;

	[Space]
	[Header ("Stamina States Settings")]
	[Space]

	public List<staminaStateInfo> staminaStateInfoList = new List<staminaStateInfo> ();

	[Space]
	[Header ("Debug")]
	[Space]

	public bool staminaEmpty;
	public bool refillingStamina;
	public bool usingStamina;
	public bool staminaRefilled = true;

	public bool staminaUsePaused;

	public float totalStaminaRateAmount;

	public float customRefillStaminaDelayAfterUse;

	[Space]
	[Header ("Components")]
	[Space]

	public bool UIElementsAssigned = true;
	public Image circleStaminaSlider;

	public Text staminaAmountText;

	public GameObject staminaPanel;

	public Slider regularStaminaSlider;

	public CanvasGroup staminaCanvasGroup;

	float originalStaminaAmount;

	float lastTimeStaminaRefilled;
	float lastTimeStaminaUsed;

	Coroutine refillCoroutine;

	bool staminaSliderHidden;

	float auxStaminaAmount;

	bool useStaminaStateOnce;


	void Start ()
	{
		originalStaminaAmount = staminaAmount;
		auxStaminaAmount = staminaAmount;

		if (UIElementsAssigned) {
			if (!staminaObjectAlwaysActive) {
				if (staminaPanel.activeSelf) {
					staminaPanel.SetActive (false);
				}
			}

			regularStaminaColor.a = enabledAlphaValue;
			closeToEmptyStaminaColor.a = enabledAlphaValue;

			if (!useSliderCircle) {
				regularStaminaSlider.maxValue = staminaAmount;
			}
		}
	}

	void FixedUpdate ()
	{
		if (!useStaminaEnabled) {
			return;
		}

		if (!staminaObjectAlwaysActive) {

			if (!refillingStamina) {
				if (UIElementsAssigned) {
					if (staminaRefilled && !staminaSliderHidden) {
						if (Time.time > lastTimeStaminaRefilled + timeToDisableStaminaObject || useFade) {
							if (useFade) {
								staminaCanvasGroup.alpha -= Time.deltaTime / fadeSpeed;

								if (staminaCanvasGroup.alpha <= 0) {
									staminaSliderHidden = true;
								}
							} else {
								if (!staminaSliderHidden) {
									if (staminaPanel.activeSelf) {
										staminaPanel.SetActive (false);
									}

									staminaSliderHidden = true;
								}
							}
						}
					}
				}
			}
		}

		if (!refillingStamina) {
			if (!staminaRefilled && !usingStamina) {
				if (Time.time > lastTimeStaminaUsed + refillStaminaDelayAfterUse && Time.time > lastTimeStaminaUsed + customRefillStaminaDelayAfterUse) {
					if (!anyStaminaStateInUse ()) {
						refillStamina (0);
					}
				}
			}
		}

		if (!refillingStamina && usingStamina && !staminaUsePaused) {
			useStaminaAmount (totalStaminaRateAmount * Time.fixedDeltaTime);
		}
	}

	public bool anyStaminaStateInUse ()
	{
		for (int i = 0; i < staminaStateInfoList.Count; i++) {
			if (staminaStateInfoList [i].staminaStateInUse) {
				return true;
			}
		}

		return false;
	}

	public void activeStaminaStateWithCustomAmount (string staminaName, float customAmountToUse, float newDelayAfterUse)
	{
		if (!useStaminaEnabled) {
			return;
		}

		if (staminaEmpty && (waitToStaminaFilledToUseAgain || staminaAmount <= 0) || customAmountToUse <= 0) {
			return;
		}

		for (int i = 0; i < staminaStateInfoList.Count; i++) {
			staminaStateInfo currentStaminaStateInfoToCheck = staminaStateInfoList [i];

			if (currentStaminaStateInfoToCheck.Name.Equals (staminaName)) {
				if (!currentStaminaStateInfoToCheck.stateEnabled) {
					return;
				}

				stopRefillCoroutine ();

				refillingStamina = false;

				staminaRefilled = false;

				if (currentStaminaStateInfoToCheck.useCustomRefillStaminaDelayAfterUse) {
					if (newDelayAfterUse > 0) {
						if (newDelayAfterUse > customRefillStaminaDelayAfterUse) {
							customRefillStaminaDelayAfterUse = newDelayAfterUse;
						}
					} else {
						if (currentStaminaStateInfoToCheck.customRefillStaminaDelayAfterUse > customRefillStaminaDelayAfterUse) {
							customRefillStaminaDelayAfterUse = currentStaminaStateInfoToCheck.customRefillStaminaDelayAfterUse;
						}
					}
				} else {
					if (newDelayAfterUse > 0) {
						if (newDelayAfterUse > customRefillStaminaDelayAfterUse) {
							customRefillStaminaDelayAfterUse = newDelayAfterUse;
						}
					} else {
						customRefillStaminaDelayAfterUse = 0;
					}
				}

				useStaminaAmount (customAmountToUse);
			}
		}
	}

	public void activeStaminaState (string staminaName)
	{
		if (!useStaminaEnabled) {
			return;
		}

		if (staminaEmpty && (waitToStaminaFilledToUseAgain || staminaAmount <= 0)) {
			for (int i = 0; i < staminaStateInfoList.Count; i++) {
				staminaStateInfo currentStaminaStateInfoToCheck = staminaStateInfoList [i];

				if (currentStaminaStateInfoToCheck.stateEnabled &&
				    !currentStaminaStateInfoToCheck.stateInPause &&
				    currentStaminaStateInfoToCheck.Name.Equals (staminaName)) {
				
					currentStaminaStateInfoToCheck.eventOnStaminaEmpty.Invoke ();

					return;
				}
			}

			return;
		}

		int stateIndex = staminaStateInfoList.FindIndex (s => s.Name == staminaName);

		if (stateIndex > -1) {
			staminaStateInfo currentStaminaStateInfoToCheck = staminaStateInfoList [stateIndex];

			if (!currentStaminaStateInfoToCheck.stateInPause) {
				if (!currentStaminaStateInfoToCheck.stateEnabled) {
					return;
				}

				if (!useStaminaStateOnce) {
					currentStaminaStateInfoToCheck.staminaStateInUse = true;
				}

				totalStaminaRateAmount += currentStaminaStateInfoToCheck.staminaUseRate;

				if (useStaminaStateOnce) {
					useStaminaAmount (currentStaminaStateInfoToCheck.staminaAmountToUseOnce);
				}

				stopRefillCoroutine ();

				refillingStamina = false;

				staminaRefilled = false;

				if (currentStaminaStateInfoToCheck.useCustomRefillStaminaDelayAfterUse) {
					if (currentStaminaStateInfoToCheck.customRefillStaminaDelayAfterUse > customRefillStaminaDelayAfterUse) {
						customRefillStaminaDelayAfterUse = currentStaminaStateInfoToCheck.customRefillStaminaDelayAfterUse;
					}
				} else {
					customRefillStaminaDelayAfterUse = 0;
				}

				if (!useStaminaStateOnce) {
					usingStamina = true;
				}
			}
		}
	}

	public void useStaminaStateByName (string staminaName)
	{
		useStaminaStateOnce = true;

		activeStaminaState (staminaName);

		useStaminaStateOnce = false;
	}

	public void setStaminaStateAsPaused (string staminaName)
	{
		setStaminaPausedState (true, staminaName);
	}

	public void setStaminaStateAsNotPaused (string staminaName)
	{
		setStaminaPausedState (false, staminaName);
	}

	public void setStaminaPausedState (bool state, string staminaName)
	{
		int stateIndex = staminaStateInfoList.FindIndex (s => s.Name == staminaName);

		if (stateIndex > -1) {
			staminaStateInfo currentStaminaStateInfoToCheck = staminaStateInfoList [stateIndex];

			currentStaminaStateInfoToCheck.stateInPause = state;

			if (state) {
				if (currentStaminaStateInfoToCheck.stopStaminaStateIfPaused && currentStaminaStateInfoToCheck.staminaStateInUse) {
					disableStaminaState (staminaName);
				}
			}
		}
	}

	public void useAllRemainStamina ()
	{
		if (staminaAmount == 0) {
			return;
		}

		useStaminaAmountExternally (staminaAmount);
	}

	public void useStaminaAmountExternally (float amount)
	{
		if (amount <= 0) {
			return;
		}

		stopRefillCoroutine ();

		refillingStamina = false;

		staminaRefilled = false;

		customRefillStaminaDelayAfterUse = 1;

		useStaminaAmount (amount);
	}

	public void setCustomRefillStaminaDelayAfterUseValue (float newValue)
	{
		customRefillStaminaDelayAfterUse = newValue;
	}

	void useStaminaAmount (float amount)
	{
		lastTimeStaminaUsed = Time.time;

		if (UIElementsAssigned) {
			if (!staminaPanel.activeSelf) {
				staminaPanel.SetActive (true);
			}
		}

		if (useFade) {
			setStaminaSliderAlphaValue (enabledAlphaValue);
		} 

		staminaSliderHidden = false;

		staminaAmount -= amount;

		if (staminaAmount < 0) {
			staminaAmount = 0;
		}

		if (UIElementsAssigned) {
			if (useSliderCircle) {
				if (staminaAmount <= closeToEmptyStaminaAmount) {
					circleStaminaSlider.color = closeToEmptyStaminaColor;
				}

				circleStaminaSlider.fillAmount = staminaAmount / originalStaminaAmount;
			} else {
				regularStaminaSlider.value = staminaAmount;
			}

			if (staminaAmountText) {
				staminaAmountText.text = Mathf.RoundToInt (staminaAmount).ToString ();
			}
		}

		auxStaminaAmount = staminaAmount;

		if (staminaAmount <= 0) {
			setStaminaRefilledOrEmptyOnStateList (false);

			staminaEmpty = true;

			if (!anyStaminaStateInUse ()) {
				refillStamina (refillStaminaDelayAfterEmpty);
			}
		}
	}

	public void disableStaminaState (string staminaName)
	{
		if (!useStaminaEnabled) {
			return;
		}

		for (int i = 0; i < staminaStateInfoList.Count; i++) {
			staminaStateInfo currentStaminaStateInfoToCheck = staminaStateInfoList [i];

			if (currentStaminaStateInfoToCheck.stateEnabled) {
				if (currentStaminaStateInfoToCheck.Name.Equals (staminaName)) {
					currentStaminaStateInfoToCheck.staminaStateInUse = false;

					totalStaminaRateAmount -= currentStaminaStateInfoToCheck.staminaUseRate;

					if (currentStaminaStateInfoToCheck.useCustomRefillStaminaDelayAfterUse) {
						if (currentStaminaStateInfoToCheck.customRefillStaminaDelayAfterUse > customRefillStaminaDelayAfterUse) {
							customRefillStaminaDelayAfterUse = currentStaminaStateInfoToCheck.customRefillStaminaDelayAfterUse;
						}
					} else {
						customRefillStaminaDelayAfterUse = 0;
					}
				}
			}
		}

		if (totalStaminaRateAmount <= 0) {
			totalStaminaRateAmount = 0;
			usingStamina = false;
		}
	}

	public void disableCurrentStaminaState ()
	{
		if (!useStaminaEnabled) {
			return;
		}

		if (usingStamina || anyStaminaStateInUse ()) {

			for (int i = 0; i < staminaStateInfoList.Count; i++) {
				if (staminaStateInfoList [i].staminaStateInUse) {
					disableStaminaState (staminaStateInfoList [i].Name);

					return;
				}
			}
		}
	}

	public void refillStamina (float refillDelay)
	{
		stopRefillCoroutine ();

		refillCoroutine = StartCoroutine (refillStaminaCoroutine (refillDelay));
	}

	public void stopRefillCoroutine ()
	{
		if (refillCoroutine != null) {
			StopCoroutine (refillCoroutine);
		}
	}

	IEnumerator refillStaminaCoroutine (float refillDelay)
	{
		refillingStamina = true;

		yield return new WaitForSeconds (refillDelay);

		if (!waitToStaminaFilledToUseAgain) {
			setStaminaRefilledOrEmptyOnStateList (true);
		}

		while (staminaAmount < originalStaminaAmount) {
			staminaAmount += Time.deltaTime * refillStaminaRate;

			if (UIElementsAssigned) {
				if (useSliderCircle) {
					circleStaminaSlider.fillAmount = staminaAmount / originalStaminaAmount;
				} else {
					regularStaminaSlider.value = staminaAmount;
				}
			}

			updateStaminaState ();

			yield return null;
		}

		if (waitToStaminaFilledToUseAgain) {
			setStaminaRefilledOrEmptyOnStateList (true);
		}

		refillingStamina = false;

		lastTimeStaminaRefilled = Time.time;

		staminaRefilled = true;

		staminaEmpty = false;

		totalStaminaRateAmount = 0;
	}

	public void getStamina (float amount)
	{
		if (!useStaminaEnabled) {
			return;
		}

		if (!waitToStaminaFilledToUseAgain) {
			setStaminaRefilledOrEmptyOnStateList (true);
		}

		staminaAmount += amount;

		updateStaminaState ();

		if (waitToStaminaFilledToUseAgain) {
			setStaminaRefilledOrEmptyOnStateList (true);
		}

		staminaEmpty = false;
	}

	public void refillFullStamina ()
	{
		if (!useStaminaEnabled) {
			return;
		}

		getStamina (originalStaminaAmount);
	}

	void updateStaminaState ()
	{
		if (!useStaminaEnabled) {
			return;
		}

		if (UIElementsAssigned) {
			if (useSliderCircle) {
				circleStaminaSlider.fillAmount = staminaAmount / originalStaminaAmount;
			} else {
				regularStaminaSlider.value = staminaAmount;
			}

			if (staminaAmountText) {
				staminaAmountText.text = Mathf.RoundToInt (staminaAmount).ToString ();
			}

			if (staminaAmount > closeToEmptyStaminaAmount) {
				circleStaminaSlider.color = regularStaminaColor;
			}
		}

		if (staminaAmount >= originalStaminaAmount) {
			staminaAmount = originalStaminaAmount;
		}

		auxStaminaAmount = staminaAmount;
	}

	public void setStaminaSliderAlphaValue (float value)
	{
		if (UIElementsAssigned) {
			staminaCanvasGroup.alpha = value;
		}
	}

	public void increaseTotalStaminaAmount (float extraValue)
	{
		staminaAmount = originalStaminaAmount + extraValue;

		originalStaminaAmount = staminaAmount;

		auxStaminaAmount = staminaAmount;

		if (UIElementsAssigned) {
			if (!useSliderCircle) {
				regularStaminaSlider.maxValue = originalStaminaAmount;
			}
		}
	}

	public void initializeStaminaAmount (float newValue)
	{
		staminaAmount = newValue;
	}

	public void increaseTotalRefillStaminaRate (float extraValue)
	{
		refillStaminaRate = refillStaminaRate + extraValue;
	}

	public void initializeRefillStaminaRate (float newValue)
	{
		refillStaminaRate = newValue;
	}

	public void setStaminaUsePausedState (bool state)
	{
		staminaUsePaused = state;
	}

	public void setStaminaRefilledOrEmptyOnStateList (bool state)
	{
		for (int i = 0; i < staminaStateInfoList.Count; i++) {
			staminaStateInfo currentStaminaStateInfoToCheck = staminaStateInfoList [i];

			if (currentStaminaStateInfoToCheck.stateEnabled) {
				if (state) {
					currentStaminaStateInfoToCheck.eventOnStaminaRefilled.Invoke ();
				} else {
					currentStaminaStateInfoToCheck.eventOnStaminaEmpty.Invoke ();
				}
			}
		}
	}

	public float getStaminaAmountToLimit ()
	{
		return originalStaminaAmount - auxStaminaAmount;
	}

	public void addAuxStaminaAmount (float amount)
	{
		auxStaminaAmount += amount;
	}

	public void setUseStaminaEnabledState (bool state)
	{
		useStaminaEnabled = state;
	}

	[System.Serializable]
	public class staminaStateInfo
	{
		[Header ("Main Settings")]
		[Space]

		public string Name;
		public bool stateEnabled = true;
		public float staminaUseRate;

		public float staminaAmountToUseOnce;

		[Space]
		[Header ("Other Settings")]
		[Space]

		public bool useCustomRefillStaminaDelayAfterUse;
		public float customRefillStaminaDelayAfterUse;

		public bool stopStaminaStateIfPaused;

		[Space]
		[Header ("Debug")]
		[Space]

		public bool staminaStateInUse;
		public bool stateInPause;
	
		[Space]
		[Header ("Event Settings")]
		[Space]

		public UnityEvent eventOnStaminaEmpty;
		public UnityEvent eventOnStaminaRefilled;
	}
}
