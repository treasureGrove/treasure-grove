using UnityEngine;
using System.Collections;

public class rechargerStation : MonoBehaviour
{
	public float healSpeed;
	public string animationName;
	public AudioClip sound;
	public GameObject button;

	GameObject player;
	bool healing;
	bool fullyHealed;
	bool inside;
	bool playingAnimationForward;
	float healthAmount;
	float maxHealthAmount;
	float powerAmount;
	float maxPowerAmount;
	otherPowers powerManager;
	health currentHealth;
	Animation mainAnimation;
	AudioSource mainAudioSource;
	Collider buttonCollider;

	//this station reloads the player's health and energy, just in case that any of their values are lower that their max values
	void Start ()
	{
		//disable the button trigger
		mainAudioSource = GetComponent<AudioSource> ();
		mainAudioSource.clip = sound;

		buttonCollider = button.GetComponent<Collider> ();
		buttonCollider.enabled = false;

		mainAnimation = GetComponent<Animation> ();
	}

	void Update ()
	{
		//if the station is healing the player
		if (healing && !fullyHealed) {
			//refill health and energy
			healthAmount = applyDamage.getCurrentHealthAmount (player);
			maxHealthAmount = applyDamage.getMaxHealthAmount (player);
			powerAmount = powerManager.getCurrentEnergyAmount ();
			maxPowerAmount = powerManager.getMaxEnergyAmount ();

			if (healthAmount < maxHealthAmount) {
				currentHealth.getHealth (Time.deltaTime * healSpeed);
			}

			if (powerAmount < maxPowerAmount) {
				powerManager.getEnergy (Time.deltaTime * healSpeed);
			}

			//if the healht and energy are both refilled, stop the station
			if (healthAmount >= maxHealthAmount && powerAmount >= maxPowerAmount) {
				stopHealing ();
			}
		}

		//if the player enters in the station and the button is not enabled
		if (inside && !healing && !buttonCollider.enabled) {
			//check the health and energy values
			healthAmount = applyDamage.getCurrentHealthAmount (player);
			maxHealthAmount = applyDamage.getMaxHealthAmount (player);
			powerAmount = powerManager.getCurrentEnergyAmount ();
			maxPowerAmount = powerManager.getMaxEnergyAmount ();

			if (healthAmount < maxHealthAmount || powerAmount < maxPowerAmount) {
				//if they are not fulled, enable the button trigger
				fullyHealed = false;

				buttonCollider.enabled = true;
			}
		}

		if (mainAnimation) {
			//if the player is full of energy and health, and the animation in the station is not being playing, then disable the station and play the disable animation
			if (playingAnimationForward && !mainAnimation.IsPlaying (animationName) && fullyHealed) {
				playingAnimationForward = false;

				mainAnimation [animationName].speed = -1; 
				mainAnimation [animationName].time = mainAnimation [animationName].length;
				mainAnimation.Play (animationName);
			}
		}
	}

	//this function is called when the button is pressed
	public void healPlayer ()
	{
		//check if the player is inside the station and his health or energy is not fulled
		if (inside && !fullyHealed) {
			//start to heal him
			healing = true;
			playingAnimationForward = true;

			//play the station animation and the heal sound
			mainAnimation [animationName].speed = 1; 
			mainAnimation.Play (animationName);

			mainAudioSource.Play ();
			mainAudioSource.loop = true;

			buttonCollider.enabled = false;
		}
	}

	//stop the station
	public void stopHealing ()
	{
		healing = false;
		fullyHealed = true;

		mainAudioSource.loop = false;
	}

	void OnTriggerEnter (Collider col)
	{
		//check if the player is inside the station
		if (col.gameObject.CompareTag ("Player")) {
			player = col.gameObject;

			powerManager = player.GetComponent<otherPowers> ();
			currentHealth = player.GetComponent<health> ();

			inside = true;
		}
	}

	void OnTriggerExit (Collider col)
	{
		//if the player exits from the station and he was being healing, stop the station
		if (col.gameObject.CompareTag ("Player")) {
			inside = false;

			if (healing) {
				stopHealing ();
			}
		}
	}
}