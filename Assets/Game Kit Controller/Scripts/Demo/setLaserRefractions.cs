using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class setLaserRefractions : MonoBehaviour
{
	public GameObject objectToChange;
	public Text textValue;

	public string shieldAbilityName = "Shield";

	playerShieldSystem currentPlayerShieldSystem;

	//change a value in the object to change
	public void getSliderValue (Slider info)
	{
		int value = (int)info.value;

		currentPlayerShieldSystem.setLaserRefractionLimit (value);

		textValue.text = info.value.ToString ("#");
	}

	public void setCurrentPlayer (GameObject newPlayer)
	{
		objectToChange = newPlayer;

		playerComponentsManager currentPlayerComponentsManager = objectToChange.GetComponent<playerComponentsManager> ();

		if (currentPlayerComponentsManager != null) {
			playerAbilitiesSystem currentPlayerAbilitiesSystem = currentPlayerComponentsManager.getPlayerAbilitiesSystem ();

			if (currentPlayerAbilitiesSystem != null) {
				currentPlayerShieldSystem = (playerShieldSystem)currentPlayerAbilitiesSystem.getAbilityByName (shieldAbilityName);
			}
		}
	}
}
