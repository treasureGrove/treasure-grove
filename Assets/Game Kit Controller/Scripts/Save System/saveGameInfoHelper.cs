using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class saveGameInfoHelper : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool checkSaveStateOnStartEnabled = true;

	[Space]
	[Header ("Components Settings")]
	[Space]

	public saveGameSystem mainSaveGameSystem;

	[Space]
	[Header ("Events Settings")]
	[Space]

	public UnityEvent eventOnHomeMenuOpened;

	public UnityEvent eventOnGameSavesLocated;
	public UnityEvent eventOnGameSavesNotLocated;


	int currentSaveNumberOnPlayerPrefs = -1;

	void Start ()
	{
		if (!checkSaveStateOnStartEnabled) {
			return;
		}

		checkIfGameCanContinue ();
	}

	public void checkIfGameCanContinue ()
	{
		eventOnHomeMenuOpened.Invoke ();

		mainSaveGameSystem.startGameSystem ();

		if (mainSaveGameSystem.isThereSaveSlotsLoaded ()) {
			eventOnGameSavesLocated.Invoke ();
		} else {
			eventOnGameSavesNotLocated.Invoke ();
		}
	}

	public void storeSaveNumberOnPlayerPrefs (int newValue)
	{
		PlayerPrefs.SetInt ("saveNumber", newValue);

		currentSaveNumberOnPlayerPrefs = newValue;

		PlayerPrefs.SetInt ("Delete Player Prefs Active", 1);
	}

	public void removeSaveNumberOnPlayerPrefs ()
	{
		if (PlayerPrefs.HasKey ("saveNumber") && currentSaveNumberOnPlayerPrefs != -1) {

			if (currentSaveNumberOnPlayerPrefs == PlayerPrefs.GetInt ("saveNumber")) {

				PlayerPrefs.DeleteKey ("saveNumber");

				currentSaveNumberOnPlayerPrefs = -1;


				PlayerPrefs.DeleteKey ("Delete Player Prefs Active");
			}
		}
	}
}
