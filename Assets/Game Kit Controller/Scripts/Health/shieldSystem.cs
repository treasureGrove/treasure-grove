using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class shieldSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool useShield;
	public float maxShieldAmount;
	public float shieldAmount = 100;
	public bool regenerateShield;
	public bool constantShieldRegenerate;
	public float regenerateShieldSpeed = 0;
	public float regenerateShieldTime;
	public float regenerateShieldAmount;

	public bool useHealthBarManager = true;

	public string shieldStatName = "Current Shield";

	public string mainManagerName = "Health Bar Manager";

	[Space]
	[Header ("Shield Slider Setting")]
	[Space]

	public bool hideSliderWhenNotDamageReceived;
	public float timeToHideSliderAfterDamage;
	public RectTransform mainSliderParent;
	public RectTransform hiddenSliderParent;
	public RectTransform mainSliderTransform;

	Coroutine hideSliderCoroutine;

	float lastTimeHideSliderChecked;

	[Space]
	[Header ("Events Setting")]
	[Space]

	public UnityEvent eventOnStartShield;
	public UnityEvent eventOnDamageShield;
	public eventParameters.eventToCallWithGameObject eventOnDamageShieldWithAttacker;
	public UnityEvent eventOnShieldDestroyed;
	public UnityEvent eventOnStartRegenerateShield;

	public UnityEvent eventToCheckIfShieldIsActive;
	public UnityEvent eventToCheckIfShieldIsNotActive;
	public UnityEvent eventToCheckIfShieldIsDestroyed;

	[Space]
	[Header ("Components")]
	[Space]

	public GameObject shieldPanel;
	public Slider shieldSlider;
	public Text shieldSliderText;

	public health mainHealth;
	public healthBarManagementSystem healthBarManager;

	public playerStatsSystem playerStatsManager;

	bool hasPlayerStatsManager;

	float auxShieldAmount;
	float lastShieldDamageTime = 0;

	bool shieldPreviouslyDestroyed;

	bool hasShieldSlider;

	bool dead;

	bool originalUseShield;

	public void startShieldComponents ()
	{
		if (healthBarManager == null) {
			GKC_Utils.instantiateMainManagerOnSceneWithType (mainManagerName, typeof(healthBarManagementSystem));

			healthBarManager = FindObjectOfType<healthBarManagementSystem> ();
		}

		if (maxShieldAmount == 0) {
			maxShieldAmount = shieldAmount;
		}

		auxShieldAmount = shieldAmount;

		if (shieldSlider != null) {

			hasShieldSlider = true;

			shieldSlider.maxValue = maxShieldAmount;
			shieldSlider.value = shieldAmount;

			if (shieldSliderText != null) {
				shieldSliderText.text = maxShieldAmount.ToString ();
			}

			if (hideSliderWhenNotDamageReceived) {
				checkSetSliderParent (false);
			}
		}

		if (useHealthBarManager) {
			hasShieldSlider = true;
		}

		if (playerStatsManager != null) {
			hasPlayerStatsManager = true;
		}

		eventOnStartShield.Invoke ();

		originalUseShield = useShield;
	}

	public void disableShield ()
	{
		setUseShieldState (false);
	}

	public void updateSliderState ()
	{
		if (!useShield) {
			return;
		}

		if (!dead) {
			if (regenerateShield) {
				if (constantShieldRegenerate) {
					if (regenerateShieldSpeed > 0 && shieldAmount < maxShieldAmount) {
						if (Time.time > lastShieldDamageTime + regenerateShieldTime && Time.time > mainHealth.getLastDamageTime () + regenerateShieldTime) {
							getShield (regenerateShieldSpeed * Time.deltaTime);
						}
					}
				} else {
					if (shieldAmount < maxShieldAmount) {
						if (Time.time > lastShieldDamageTime + regenerateShieldTime && Time.time > mainHealth.getLastDamageTime () + regenerateShieldTime) {
							getShield (regenerateShieldAmount);
							lastShieldDamageTime = Time.time;
						}
					}
				}
			}
		}
	}

	public void updateShieldSlider (float value)
	{
		if (hasShieldSlider) {
			if (shieldSlider != null) {
				shieldSlider.value = value;

				if (shieldSliderText != null) {
					shieldSliderText.text = Mathf.RoundToInt (value).ToString ();
				}

				checkHideSliderParent ();
			} 

			if (useHealthBarManager) {
				healthBarManager.setShieldSliderAmount (mainHealth.getHealthID (), value); 
			}

			if (hasPlayerStatsManager) {
				playerStatsManager.updateStatValue (shieldStatName, shieldAmount);
			}
		}
	}

	public void updateShieldSliderMaxValue (float newMaxValue)
	{
		if (hasShieldSlider) {
			if (shieldSlider != null) {
				shieldSlider.maxValue = newMaxValue;
			} 

			if (useHealthBarManager) {
				healthBarManager.updateShieldSliderMaxValue (mainHealth.getHealthID (), newMaxValue); 
			}

			checkHideSliderParent ();
		}
	}

	public void checkHideSliderParent ()
	{
		if (hideSliderWhenNotDamageReceived) {
			stopCheckHideSliderCoroutine ();

			lastTimeHideSliderChecked = Time.time;

			hideSliderCoroutine = StartCoroutine (checkHideSliderCoroutine ());
		}
	}

	void stopCheckHideSliderCoroutine ()
	{
		if (hideSliderCoroutine != null) {
			StopCoroutine (hideSliderCoroutine);
		}
	}

	IEnumerator checkHideSliderCoroutine ()
	{
		checkSetSliderParent (true);

		bool targetReached = false;

		while (!targetReached) {

			if (Time.time > timeToHideSliderAfterDamage + lastTimeHideSliderChecked) {
				targetReached = true;
			}

			if (mainHealth.isPlayerMenuActive ()) {
				targetReached = true;
			}

			yield return null;
		}

		checkSetSliderParent (false);
	}

	void checkSetSliderParent (bool setOnMainParent)
	{
		if (hideSliderWhenNotDamageReceived) {
			if (setOnMainParent) {
				mainSliderTransform.transform.SetParent (mainSliderParent);
			} else {
				mainSliderTransform.transform.SetParent (hiddenSliderParent);

			}

			mainSliderTransform.transform.localPosition = Vector3.zero;
			mainSliderTransform.transform.localRotation = Quaternion.identity;
		}
	}

	public void getShield (float amount)
	{
		if (!dead) {

			shieldPreviouslyDestroyed = false;

			if (shieldAmount == 0) {
				shieldPreviouslyDestroyed = true;
			}

			shieldAmount += amount;

			//check that the shield amount is not higher that the shield max value of the slider
			if (shieldAmount >= maxShieldAmount) {
				shieldAmount = maxShieldAmount;
			}

			updateShieldSlider (shieldAmount);

			if (shieldPreviouslyDestroyed && shieldAmount > 0) {
				eventOnStartRegenerateShield.Invoke ();
			}
		}

		auxShieldAmount = shieldAmount;
	}

	public void setDeadState (bool state)
	{
		dead = state;
	}

	public bool receiveDamage (float damageAmount, GameObject attacker)
	{
		if (useShield && shieldAmount > 0) {

			if (damageAmount > shieldAmount) {
				damageAmount = shieldAmount;
			}

			shieldAmount -= damageAmount;
			auxShieldAmount = shieldAmount;

			eventOnDamageShield.Invoke ();

			eventOnDamageShieldWithAttacker.Invoke (attacker);

			if (shieldAmount <= 0) {
				shieldAmount = 0;

				eventOnShieldDestroyed.Invoke ();
			}

			updateShieldSlider (shieldAmount);

			lastShieldDamageTime = Time.time;

			return true;
		} else {
			return false;
		}
	}

	public float getShieldAmountToLimit ()
	{
		return maxShieldAmount - auxShieldAmount;
	}

	public void addAuxShieldAmount (float amount)
	{
		auxShieldAmount += amount;
	}

	public void increaseMaxShieldAmount (float newAmount)
	{
		maxShieldAmount += newAmount;

		updateShieldSliderMaxValue (maxShieldAmount);
	}

	public void setShieldAmountOnMaxValue ()
	{
		getShield (maxShieldAmount - shieldAmount);
	}

	public void increaseRegenerateShieldSpeed (float newAmount)
	{
		regenerateShieldSpeed += newAmount;
	}

	public void initializeShieldAmount (float newValue)
	{
		shieldAmount = newValue;
	}

	public void initializeMaxShieldAmount (float newValue)
	{
		maxShieldAmount = newValue;
	}

	public void initializeRegenerateShieldSpeed (float newValue)
	{
		regenerateShieldSpeed = newValue;
	}

	public void increaseMaxShieldAmountByMultiplier (float amountMultiplier)
	{
		maxShieldAmount *= amountMultiplier;

		updateShieldSliderMaxValue (maxShieldAmount);
	}

	public void setUseShieldState (bool state)
	{
		useShield = state;

		if (shieldPanel != null) {
			if (shieldPanel.activeSelf != useShield) {
				shieldPanel.SetActive (useShield);
			}
		}
	}

	public void setOriginalUseShieldState ()
	{
		setUseShieldState (originalUseShield);
	}

	public void checkEventsForShieldState ()
	{
		if (useShield) {
			if (shieldAmount > 0) {
				eventToCheckIfShieldIsActive.Invoke ();
			} else {
				eventToCheckIfShieldIsDestroyed.Invoke ();
			}
		} else {
			eventToCheckIfShieldIsNotActive.Invoke ();
		}
	}

	public float getCurrentShieldAmount ()
	{
		return shieldAmount;
	}
}