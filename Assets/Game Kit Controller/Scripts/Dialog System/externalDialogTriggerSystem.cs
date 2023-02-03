using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class externalDialogTriggerSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool setNewDialogIndex;
	public int newDialogIndex;

	public bool checkDialogContentSystemID;
	public int IDToCheck;

	[Space]
	[Header ("Components")]
	[Space]

	public dialogContentSystem mainDialogContentSystem;
	public electronicDevice mainElectronicDevice;

	[Space]
	[Header ("Set Object Manually Settings")]
	[Space]

	public bool assignObjectManually;
	public GameObject playerToConfigure;

	public bool searchPlayerOnSceneIfNotAssigned;


	public void activateDialogByCharacter (GameObject newCharacter)
	{
		characterDialogContentSystem currentCharacterDialogContentSystem = newCharacter.GetComponent<characterDialogContentSystem> ();

		if (currentCharacterDialogContentSystem != null) {
			mainDialogContentSystem = currentCharacterDialogContentSystem.mainDialogContentSystem;

			if (!checkDialogContentSystemID || mainDialogContentSystem.dialogContentID == IDToCheck) {

				mainElectronicDevice = currentCharacterDialogContentSystem.mainElectronicDevice;

				activateDialog ();
			}
		}
	}

	public void activateDialog ()
	{
		if (assignObjectManually) {
			if (playerToConfigure == null) {
				if (searchPlayerOnSceneIfNotAssigned) {
					findPlayerOnScene ();
				}
			}

			if (playerToConfigure == null) {
				print ("WARNING: no object has been assigned manually on external dialog trigger system");

				return;
			}
		}

		startDialog ();
	}

	public void activateDialogToPlayer (GameObject playerToCheck)
	{
		playerToConfigure = playerToCheck;

		if (playerToConfigure == null) {
			return;
		}

		startDialog ();
	}
		
	void startDialog ()
	{
		if (mainDialogContentSystem != null) {
			mainDialogContentSystem.setPlayingExternalDialogState (true);

			if (setNewDialogIndex) {
				mainDialogContentSystem.setCompleteDialogIndex (newDialogIndex);
			}
		}

		if (mainElectronicDevice != null) {
			mainElectronicDevice.setPlayerManually (playerToConfigure);

			if (mainDialogContentSystem.dialogInProcess) {
				playerComponentsManager currentPlayerComponentsManager = playerToConfigure.GetComponent<playerComponentsManager> ();

				if (currentPlayerComponentsManager != null) {
					dialogSystem currentDialogSystem = currentPlayerComponentsManager.getMainDialogSystem ();

					if (currentDialogSystem != null) {
						currentDialogSystem.inputPlayNextDialogWioutPausingPlayer ();
					}
				}
			} else {
				mainElectronicDevice.activateDevice ();
			}
		}
	}

	public void findPlayerOnScene ()
	{
		if (searchPlayerOnSceneIfNotAssigned) {
			playerToConfigure = GKC_Utils.findMainPlayerOnScene ();
		}
	}
}
