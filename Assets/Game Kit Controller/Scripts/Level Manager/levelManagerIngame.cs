using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

public class levelManagerIngame : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool changeSceneEnabled = true;

	public int sceneNumberToLoad;

	public int levelManagerID;

	public int levelManagerIDToLoad;

	public Transform spawPlayerPositionTransform;

	public bool changeLevelWithDelay;
	public float delayToChangeLevel = 0.1f;

	public bool autoSaveGameOnSceneChangeEnabled = true;

	[Space]
	[Header ("Gizmo Settings")]
	[Space]

	public bool showGizmo;
	public Color gizmoColor = Color.red;
	public float gizmoRadius = 0.06f;

	[Space]
	[Header ("Other Elements")]
	[Space]

	public string sceneFolderPath = "Assets//Game Kit Controller/Scenes/";
	public string sceneToChangeName = "New Scene";

	[Header ("Player Manual Settings")]
	public bool setPlayerManually;
	public bool searchPlayerByTagIfNotAssigned = true;
	public GameObject playerToConfigure;

	[Space]
	[Space]

	[TextArea (5, 20)]public string explanation = "Configure the Scene Number where the player will move using the level manager.\n\n" +
	                                              "Then, inside that new scene, you have another level manager object which has a Level Manager ID value, so set that value\n" +
	                                              "in the field Level Manager ID To Load of this component, so once the player moves to the new scene, it will appear in the level manager" +
	                                              "object with that ID.\n\n " +
	                                              "It is like an address system, in order to know to which scene to move and where spawn the player once he is there.";
	

	GameObject currentPlayer;

	saveGameSystem currentSaveGameSystem;

	public void setCurrentPlayer (GameObject newPlayer)
	{
		currentPlayer = newPlayer;
	}

	public void setCurrentPlayerAndActivateChangeOfLevel (GameObject newPlayer)
	{
		currentPlayer = newPlayer;

		activateChangeOfLevel ();
	}

	public void activateChangeOfLevel ()
	{
		if (setPlayerManually) {
			currentPlayer = playerToConfigure;
		}

		if (currentPlayer == null) {
			if (searchPlayerByTagIfNotAssigned) {

				currentPlayer = GKC_Utils.findMainPlayerOnScene ();

				if (currentPlayer == null) {
					return;
				}
			} else {
				return;
			}
		}

		bool playerIsOnVehicle = false;

		if (applyDamage.isVehicle (currentPlayer)) {

			GameObject vehicleDriver = applyDamage.getVehicleDriver (currentPlayer);

			if (vehicleDriver != null) {
				currentPlayer = vehicleDriver;

				playerIsOnVehicle = true;
			}
		}

		playerComponentsManager currentPlayerComponentsManager = currentPlayer.GetComponent<playerComponentsManager> (); 

		if (currentPlayerComponentsManager != null) {
			if (playerIsOnVehicle && !currentPlayerComponentsManager.getPlayerController ().isPlayerDriving ()) {
				return;
			}

			currentSaveGameSystem = currentPlayerComponentsManager.getSaveGameSystem ();

			currentSaveGameSystem.saveGameCheckpoint (spawPlayerPositionTransform, levelManagerIDToLoad, sceneNumberToLoad, true, true, autoSaveGameOnSceneChangeEnabled);

			if (changeSceneEnabled) {
				if (changeLevelWithDelay) {
					StartCoroutine (loadNextLevel ());
				} else {
					currentSaveGameSystem.activateAutoLoad ();
				}
			}
		}
	}

	IEnumerator loadNextLevel ()
	{
		yield return new WaitForSeconds (delayToChangeLevel);

		currentSaveGameSystem.activateAutoLoad ();
	}

	public void setChangeSceneEnabledState (bool state)
	{
		changeSceneEnabled = state;
	}

	public void openNextSceneOnEditor ()
	{
		string newSceneName = sceneFolderPath + sceneToChangeName + ".unity";
		if (System.IO.File.Exists (newSceneName)) {
			#if UNITY_EDITOR
			EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo ();
			EditorSceneManager.OpenScene (newSceneName);
			#endif
		} else {
			print ("WARNING: Scene called " + sceneToChangeName + " is not located in the path " + sceneFolderPath + ".\n " +
			"Make sure to configure the path or place the scene file in the current path configured");
		}
	}

	void OnDrawGizmos ()
	{
		if (!showGizmo) {
			return;
		}

		if (GKC_Utils.isCurrentSelectionActiveGameObject (gameObject)) {
			DrawGizmos ();
		}
	}

	void OnDrawGizmosSelected ()
	{
		DrawGizmos ();
	}

	void DrawGizmos ()
	{
		if (showGizmo) {
			Gizmos.color = gizmoColor;
			if (spawPlayerPositionTransform != null) {
				Gizmos.DrawSphere (spawPlayerPositionTransform.position, gizmoRadius);
			} else {
				Gizmos.DrawSphere (transform.position, gizmoRadius);
			}
		}
	}
}
