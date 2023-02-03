using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class damageScreenSystem : MonoBehaviour
{
	[Header ("Main Setting")]
	[Space]

	public bool damageScreenEnabled;

	public Color damageColor;

	public float maxAlphaDamage = 0.6f;
	public float fadeToDamageColorSpeed;
	public float fadeToTransparentSpeed;
	public float timeToStartToHeal;
	public bool showDamageDirection;
	public bool showDamagePositionWhenEnemyVisible;
	public bool showAllDamageDirections;

	public bool checkHealthAmountOnStart;

	public bool usedByAI;

	[Space]
	[Header ("Components")]
	[Space]

	public GameObject damageScreen;
	public GameObject damageDirectionIcon;
	public GameObject damagePositionIcon;
	public GameObject playerControllerGameObject;
	public playerCamera mainPlayerCamera;
	public Camera mainCamera;
	public RawImage damageImage;
	public health healtManager;

	[Space]
	[Header ("Debug")]
	[Space]

	public List<damageInfo> enemiesDamageList = new List<damageInfo> ();

	bool wounding;
	bool healWounds;

	int i, j;

	Vector2 mainCanvasSizeDelta;
	Vector2 halfMainCanvasSizeDelta;

	Vector2 iconPosition2d;
	bool usingScreenSpaceCamera;

	bool targetOnScreen;
	Vector3 screenPoint;
	float angle;
	Vector3 screenCenter;

	bool showDamageImage;

	float screenWidth;
	float screenHeight;

	Vector3 currentPosition;

	damageInfo currentDamageInfo;

	float lastTimeWounded;

	bool canStartToHeal;

	bool noAttackerFound;

	void Start ()
	{
		if (usedByAI) {
			return;
		}

		damageImage.color = damageColor;

		mainCanvasSizeDelta = mainPlayerCamera.getMainCanvasSizeDelta ();
		halfMainCanvasSizeDelta = mainCanvasSizeDelta * 0.5f;
		usingScreenSpaceCamera = mainPlayerCamera.isUsingScreenSpaceCamera ();

		if (checkHealthAmountOnStart) {
			checkHealthState ();
		}

		if (mainCamera == null) {
			mainCamera = mainPlayerCamera.getMainCamera ();
		}
	}

	void FixedUpdate ()
	{
		if (usedByAI) {
			return;
		}

		if (damageScreenEnabled) {
			//if the player is wounded, then activate the icon that aims to the enemy position, so the player can see the origin of the damage
			//also, the screen color changes to red, setting the alpha value of a panel in the hud
			if (wounding) {
				if (showDamageImage) {
					Color alpha = damageImage.color;

					if (alpha.a < maxAlphaDamage) {
						float alphaValue = 1 - healtManager.getCurrentHealthAmount () / healtManager.getMaxHealthAmount ();
						alpha.a = Mathf.Lerp (alpha.a, alphaValue, Time.deltaTime * fadeToDamageColorSpeed);
					} else {
						alpha.a = maxAlphaDamage;
					}

					damageImage.color = alpha;
				}

				if (showDamageDirection) {

					if (!usingScreenSpaceCamera) {
						updateScreenValues ();
					}

					for (i = 0; i < enemiesDamageList.Count; i++) {
						currentDamageInfo = enemiesDamageList [i];

						if (currentDamageInfo.enemy != null && currentDamageInfo.enemy != playerControllerGameObject) {
							//get the target position from global to local in the screen

							currentPosition = currentDamageInfo.enemy.transform.position;

							if (usingScreenSpaceCamera) {
								screenPoint = mainCamera.WorldToViewportPoint (currentPosition);
								targetOnScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1; 
							} else {
								screenPoint = mainCamera.WorldToScreenPoint (currentPosition);
								targetOnScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < screenWidth && screenPoint.y > 0 && screenPoint.y < screenHeight;
							}

							//if the target is visible in the screen, disable the arrow
							if (targetOnScreen) {
								if (currentDamageInfo.damageDirection.activeSelf) {
									currentDamageInfo.damageDirection.SetActive (false);
								}

								if (showDamagePositionWhenEnemyVisible) {
									if (!currentDamageInfo.damagePosition.activeSelf) {
										currentDamageInfo.damagePosition.SetActive (true);
									}
											
									if (usingScreenSpaceCamera) {
										iconPosition2d = new Vector2 ((screenPoint.x * mainCanvasSizeDelta.x) - halfMainCanvasSizeDelta.x,
											(screenPoint.y * mainCanvasSizeDelta.y) - halfMainCanvasSizeDelta.y);

										currentDamageInfo.damagePositionRectTransform.anchoredPosition = iconPosition2d;
									} else {
										currentDamageInfo.damagePosition.transform.position = new Vector3 (screenPoint.x, screenPoint.y, 0);
									}
								}
							} else {
								//if the target is off screen, rotate the arrow to the target direction
								if (!currentDamageInfo.damageDirection.activeSelf) {
									currentDamageInfo.damageDirection.SetActive (true);
									currentDamageInfo.damagePosition.SetActive (false);
								}

								if (usingScreenSpaceCamera) {
									iconPosition2d = new Vector2 ((screenPoint.x * mainCanvasSizeDelta.x) - halfMainCanvasSizeDelta.x, (screenPoint.y * mainCanvasSizeDelta.y) - halfMainCanvasSizeDelta.y);

									if (screenPoint.z < 0) {
										iconPosition2d *= -1;
									}

									angle = Mathf.Atan2 (iconPosition2d.y, iconPosition2d.x);
									angle -= 90 * Mathf.Deg2Rad;
								} else {
									if (screenPoint.z < 0) {
										screenPoint *= -1;
									}

									screenCenter = new Vector3 (screenWidth, screenHeight, 0) / 2;
									screenPoint -= screenCenter;
									angle = Mathf.Atan2 (screenPoint.y, screenPoint.x);
									angle -= 90 * Mathf.Deg2Rad;
								}

								currentDamageInfo.damageDirection.transform.rotation = Quaternion.Euler (0, 0, angle * Mathf.Rad2Deg);
							}

							//if the player is not damaged for a while, disable the arrow
							if (Time.time > currentDamageInfo.woundTime + timeToStartToHeal) {
								Destroy (currentDamageInfo.damageDirection);
								Destroy (currentDamageInfo.damagePosition);

								enemiesDamageList.RemoveAt (i);
							}
						} else {
							enemiesDamageList.RemoveAt (i);
						}
					}
				}
			} 

			if (wounding) {
				canStartToHeal = (Time.time > lastTimeWounded + timeToStartToHeal);

				if ((!showAllDamageDirections && canStartToHeal) ||
				    (showAllDamageDirections &&	enemiesDamageList.Count == 0)) {

					if (!noAttackerFound || canStartToHeal) {
						healWounds = true;
						wounding = false;
					}
				}
			}

			//if the player is not reciving damage for a while, then set alpha of the red color of the background to 0
			if (healWounds || (wounding && enemiesDamageList.Count == 0 && showAllDamageDirections)) {
				if (showDamageImage) {
					Color alpha = damageImage.color;
					alpha.a -= Time.deltaTime * fadeToTransparentSpeed;
					damageImage.color = alpha;

					if (alpha.a <= 0) {
						damageScreen.SetActive (false);
						healWounds = false;
					}
				} else {
					damageScreen.SetActive (false);
					healWounds = false;
				}
			}
		}
	}

	public void checkHealthState ()
	{
		if (!damageScreenEnabled) {
			return;
		}

		if (healtManager.getCurrentHealthAmount () < healtManager.getMaxHealthAmount ()) {
			lastTimeWounded = Time.time;

			showDamageImage = true;

			wounding = true;

			if (!damageScreen.activeSelf) {
				damageScreen.SetActive (true);
			}
		}
	}

	public void setDamageDirectionWithDamageScreen (GameObject enemy)
	{
		setDamageDir (enemy, true);
	}

	public void setDamageDirectionWithoutDamageScreen (GameObject enemy)
	{
		setDamageDir (enemy, false);
	}

	//set the direction of the damage arrow to see the enemy that injured the player
	public void setDamageDir (GameObject enemy, bool showDamageImageValue)
	{
		if (showAllDamageDirections) {
			bool enemyFound = false;
			int index = -1;

			if (enemy == null) {
				noAttackerFound = true;
			}

			for (j = 0; j < enemiesDamageList.Count; j++) {
				if (enemiesDamageList [j].enemy == enemy) {
					index = j;
					enemyFound = true;

					noAttackerFound = false;
				}
			}

			if (!enemyFound) {
				damageInfo newEnemy = new damageInfo ();
				newEnemy.enemy = enemy;

				GameObject newDirection = (GameObject)Instantiate (damageDirectionIcon, Vector3.zero, Quaternion.identity, damageScreen.transform);

				newDirection.transform.localScale = Vector3.one;
				newDirection.transform.localPosition = Vector3.zero;
				newEnemy.damageDirection = newDirection;

				GameObject newPosition = (GameObject)Instantiate (damagePositionIcon, Vector3.zero, Quaternion.identity, damageScreen.transform);

				newPosition.transform.localScale = Vector3.one;
				newPosition.transform.localPosition = Vector3.zero;
				newEnemy.damagePosition = newPosition;
				newEnemy.woundTime = Time.time;

				newEnemy.damagePositionRectTransform = newPosition.GetComponent<RectTransform> ();
				enemiesDamageList.Add (newEnemy);
			} else {
				if (index != -1) {
					enemiesDamageList [index].woundTime = Time.time;
				}
			}
		}

		lastTimeWounded = Time.time;

		showDamageImage = showDamageImageValue;

		wounding = true;

		if (!damageScreen.activeSelf) {
			damageScreen.SetActive (true);
		}
	}

	public void setUsedByAIState (bool state)
	{
		usedByAI = state;
	}

	public void setDamageScreenEnabledState (bool state)
	{
		damageScreenEnabled = state;
	}

	public void updateScreenValues ()
	{
		screenWidth = Screen.width;
		screenHeight = Screen.height;
	}

	public void setDamageScreenEnabledStateFromEditor (bool state)
	{
		damageScreenEnabled = state;

		GKC_Utils.updateComponent (this);
	}

	[System.Serializable]
	public class damageInfo
	{
		public GameObject enemy;
		public GameObject damageDirection;
		public GameObject damagePosition;
		public float woundTime;
		public RectTransform damagePositionRectTransform;
	}
}