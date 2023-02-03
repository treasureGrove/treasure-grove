using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class playerDamageOnScreenInfoSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool showDamageActive = true;
	public bool showDamagePaused;

	public int damageOnScreenId;

	public bool placeMarkAboveDamagedTargets;

	public bool useRandomDirection;
	public bool useProjectileDirection;
	public float movementSpeed;
	public float movementAmount;
	public float maxRadiusToInstantiate;
	public float downMovementAmount = 2;

	public bool checkDistanceToTarget;
	public float distanceMultiplierAmount;

	public string criticalDamageText = "CRITICAL!";

	public string mainManagerName = "Damage On Screen Info Manager";

	[Space]
	[Header ("Text Color Settings")]
	[Space]

	public bool useRandomColor;
	public float randomColorAlpha;
	public Color damageColor = Color.red;
	public Color healColor = Color.green;

	[Space]
	[Header ("Debug")]
	[Space]

	public List<damageOnScreenInfoSystem.targetInfo> targetInfoList = new List<damageOnScreenInfoSystem.targetInfo> ();

	[Space]
	[Header ("Components")]
	[Space]

	public GameObject player;

	public Transform damageNumberParent;
	public GameObject damageNumberTargetParent;
	public GameObject damageNumberText;

	public Camera mainCamera;
	public playerCamera mainPlayerCamera;

	public RawImage markAboveDamagedTargets;

	public damageOnScreenInfoSystem damageOnScreenInfoManager;

	Vector3 currenMapObjectPosition;
	Vector3 screenPoint;

	bool targetOnScreen;

	damageOnScreenInfoSystem.targetInfo currentTargetInfo;

	Vector2 mainCanvasSizeDelta;
	Vector2 halfMainCanvasSizeDelta;

	Vector2 iconPosition2d;
	bool usingScreenSpaceCamera;

	int targetInfoListCount;

	void Awake ()
	{
		markAboveDamagedTargets.enabled = placeMarkAboveDamagedTargets;

		if (damageOnScreenInfoManager == null) {
			GKC_Utils.instantiateMainManagerOnSceneWithType (mainManagerName, typeof(damageOnScreenInfoSystem));

			damageOnScreenInfoManager = FindObjectOfType<damageOnScreenInfoSystem> ();
		} 

		if (damageOnScreenInfoManager != null) {
			damageOnScreenInfoManager.addNewPlayer (this);
		} else {
			showDamageActive = false;
		}

		if (mainCamera == null) {
			mainCamera = mainPlayerCamera.getMainCamera ();
		}
	}

	void Start ()
	{
		mainCanvasSizeDelta = mainPlayerCamera.getMainCanvasSizeDelta ();
		halfMainCanvasSizeDelta = mainCanvasSizeDelta * 0.5f;
		usingScreenSpaceCamera = mainPlayerCamera.isUsingScreenSpaceCamera ();
	}

	void FixedUpdate ()
	{
		if (!showDamageActive || showDamagePaused) { 
			return;
		}

		targetInfoListCount = targetInfoList.Count;

		if (targetInfoListCount == 0) {
			return;
		}

		for (int i = 0; i < targetInfoListCount; i++) {
			currentTargetInfo = targetInfoList [i];

			if (currentTargetInfo.containsNumberToShow) {
				if (currentTargetInfo.target != null && currentTargetInfo.targetRectTransform != null) {
					
					currenMapObjectPosition = currentTargetInfo.target.position;

					if (currentTargetInfo.useIconOffset) {
						currenMapObjectPosition += currentTargetInfo.iconOffset;
					}

					if (usingScreenSpaceCamera) {
						screenPoint = mainCamera.WorldToViewportPoint (currenMapObjectPosition);
					} else {
						screenPoint = mainCamera.WorldToScreenPoint (currenMapObjectPosition);
					}

					targetOnScreen = screenPoint.z > 0;
						
					if (targetOnScreen) {
						if (!currentTargetInfo.iconActive) {
							if (!currentTargetInfo.targetRectTransformGameObject.activeSelf) {
								currentTargetInfo.targetRectTransformGameObject.SetActive (true);
							}

							currentTargetInfo.iconActive = true;
						}

						if (usingScreenSpaceCamera) {
							iconPosition2d = new Vector2 ((screenPoint.x * mainCanvasSizeDelta.x) - halfMainCanvasSizeDelta.x, (screenPoint.y * mainCanvasSizeDelta.y) - halfMainCanvasSizeDelta.y);
							currentTargetInfo.targetRectTransform.anchoredPosition = iconPosition2d;
						} else {
							currentTargetInfo.targetRectTransform.position = new Vector3 (screenPoint.x, screenPoint.y, 0);
						}
					} else {
						if (currentTargetInfo.iconActive) {
							if (currentTargetInfo.targetRectTransformGameObject.activeSelf) {
								currentTargetInfo.targetRectTransformGameObject.SetActive (false);
							}

							currentTargetInfo.iconActive = false;
						}
					}

					for (int j = 0; j < currentTargetInfo.damageNumberInfoList.Count; j++) {
						if (currentTargetInfo.damageNumberInfoList [j].damageNumberRectTransform == null) {
							currentTargetInfo.damageNumberInfoList.RemoveAt (j);

							j--;
						}
					}

					if (currentTargetInfo.damageNumberInfoList.Count == 0) {
						currentTargetInfo.containsNumberToShow = false;
					}
				} else {
					removeElementFromListByPlayer (currentTargetInfo.ID);

					i--;
				}
			} else {
				if (currentTargetInfo.iconActive) {
					if (currentTargetInfo.targetRectTransformGameObject.activeSelf) {
						currentTargetInfo.targetRectTransformGameObject.SetActive (false);
					}

					currentTargetInfo.iconActive = false;
				}

				if (currentTargetInfo.isDead && !currentTargetInfo.containsNumberToShow && currentTargetInfo.removeDamageInScreenOnDeath) {
					removeElementFromListByPlayer (currentTargetInfo.ID);

					i--;
				}
			} 
		}
	}

	public void addNewTarget (damageOnScreenInfoSystem.targetInfo newTarget)
	{
		damageOnScreenInfoSystem.targetInfo newTargetInfo = new damageOnScreenInfoSystem.targetInfo ();
		newTargetInfo.Name = newTarget.Name;
		newTargetInfo.target = newTarget.target;
		newTargetInfo.iconOffset = newTarget.iconOffset;

		newTargetInfo.useIconOffset = newTarget.useIconOffset;

		newTargetInfo.ID = newTarget.ID;
		newTargetInfo.removeDamageInScreenOnDeath = newTarget.removeDamageInScreenOnDeath;

		GameObject newDamageNumberTargetParent = (GameObject)Instantiate (damageNumberTargetParent, Vector3.zero, Quaternion.identity, damageNumberParent);

		newDamageNumberTargetParent.transform.localScale = Vector3.one;
		newDamageNumberTargetParent.transform.localPosition = Vector3.zero;

		newTargetInfo.targetRectTransformGameObject = newDamageNumberTargetParent;
		newTargetInfo.targetRectTransform = newDamageNumberTargetParent.GetComponent<RectTransform> ();

		if (newTargetInfo.targetRectTransformGameObject.activeSelf) {
			newTargetInfo.targetRectTransformGameObject.SetActive (false);
		}

		newTargetInfo.iconActive = false;

		targetInfoList.Add (newTargetInfo);

		targetInfoListCount = targetInfoList.Count;
	}

	public void setDamageInfo (int targetIndex, float amount, bool isDamage, Vector3 direction, float healthAmount, float criticalDamageProbability)
	{
		damageOnScreenInfoSystem.targetInfo currentTargetInfoToCheck = targetInfoList [targetIndex];

		if (currentTargetInfoToCheck != null) {

			if (currentTargetInfoToCheck.isDead) {
				if (healthAmount > 0) {
					currentTargetInfoToCheck.isDead = false;
				} else {
					return;
				}
			}

			GameObject newDamageNumberText = (GameObject)Instantiate (damageNumberText, Vector3.zero, Quaternion.identity, currentTargetInfoToCheck.targetRectTransform);

			if (!newDamageNumberText.activeSelf) {
				newDamageNumberText.SetActive (true);
			}

			newDamageNumberText.transform.localScale = Vector3.one;
			newDamageNumberText.transform.localPosition = Vector3.zero;

			damageOnScreenInfoSystem.damageNumberInfo newDamageNumberInfo = new damageOnScreenInfoSystem.damageNumberInfo ();

			newDamageNumberInfo.damageNumberText = newDamageNumberText.GetComponent<Text> ();
			newDamageNumberInfo.damageNumberRectTransform = newDamageNumberText.GetComponent<RectTransform> ();

			currentTargetInfoToCheck.damageNumberInfoList.Add (newDamageNumberInfo);

			currentTargetInfoToCheck.containsNumberToShow = true;

			string text = "";

			if (useRandomColor) {
				if (isDamage) {
					text = "-";
				} else {
					text = "+";
				}

				newDamageNumberInfo.damageNumberText.color = new Vector4 (Random.Range (0f, 1f), Random.Range (0f, 1f), Random.Range (0f, 1f), randomColorAlpha);
			} else {
				if (isDamage) {
					newDamageNumberInfo.damageNumberText.color = damageColor;
				} else {
					newDamageNumberInfo.damageNumberText.color = healColor;
				}
			}

			if (amount >= 1) {
				text += amount.ToString ("0");
			} else {
				if (amount < 0.1 && amount > 0) {
					amount = 0.1f;
				}
				text += amount.ToString ("F1");
			}

			if (criticalDamageProbability == 1) {
				createCriticalDamageText (currentTargetInfoToCheck, -direction);
			}

			newDamageNumberInfo.damageNumberText.text = text;

			newDamageNumberInfo.movementCoroutine = StartCoroutine (moveNumber (currentTargetInfoToCheck.target, newDamageNumberInfo.damageNumberRectTransform, isDamage, direction));

			if (healthAmount <= 0) {
				currentTargetInfoToCheck.isDead = true;
			}
		}
	}

	public void createCriticalDamageText (damageOnScreenInfoSystem.targetInfo currentTargetInfoToCheck, Vector3 movementDirection)
	{
		GameObject newDamageNumberText = (GameObject)Instantiate (damageNumberText, Vector3.zero, Quaternion.identity, currentTargetInfoToCheck.targetRectTransform);

		if (!newDamageNumberText.activeSelf) {
			newDamageNumberText.SetActive (true);
		}

		newDamageNumberText.transform.localScale = Vector3.one;
		newDamageNumberText.transform.localPosition = Vector3.zero;

		damageOnScreenInfoSystem.damageNumberInfo newDamageNumberInfo = new damageOnScreenInfoSystem.damageNumberInfo ();

		newDamageNumberInfo.damageNumberText = newDamageNumberText.GetComponent<Text> ();
		newDamageNumberInfo.damageNumberRectTransform = newDamageNumberText.GetComponent<RectTransform> ();

		currentTargetInfoToCheck.damageNumberInfoList.Add (newDamageNumberInfo);
	
		newDamageNumberInfo.damageNumberText.color = damageColor;

		newDamageNumberInfo.damageNumberText.text = criticalDamageText;
		newDamageNumberInfo.damageNumberText.fontStyle = FontStyle.Bold;

		newDamageNumberInfo.movementCoroutine = StartCoroutine (moveNumber (currentTargetInfoToCheck.target, newDamageNumberInfo.damageNumberRectTransform, true, movementDirection));
	}

	public void removeElementFromList (int objectID)
	{
		for (int i = 0; i < targetInfoList.Count; i++) {
			if (targetInfoList [i].ID == objectID) {
				if (targetInfoList [i].targetRectTransformGameObject != null) {
					Destroy (targetInfoList [i].targetRectTransformGameObject);
				}

				targetInfoList.RemoveAt (i);

				targetInfoListCount = targetInfoList.Count;

				return;
			}
		}
	}

	public void removeElementFromListByPlayer (int objectID)
	{
		damageOnScreenInfoManager.removeElementFromTargetListCalledByPlayer (objectID, player);

		removeElementFromList (objectID);
	}

	IEnumerator moveNumber (Transform targetTransform, RectTransform damageNumberRectTransform, bool damage, Vector2 direction)
	{
		float currentDistance = 0;

		float newMovementAmount = movementAmount;
		float newMaxRadiusToInstantiate = maxRadiusToInstantiate;

		if (checkDistanceToTarget) {
			
			currentDistance = GKC_Utils.distance (transform.position, targetTransform.position);

			newMovementAmount = newMovementAmount - currentDistance * distanceMultiplierAmount;

			newMovementAmount = Mathf.Abs (newMovementAmount);

			newMaxRadiusToInstantiate = newMaxRadiusToInstantiate - currentDistance * distanceMultiplierAmount;

			newMaxRadiusToInstantiate = Mathf.Abs (newMaxRadiusToInstantiate);
		}

		if (!useRandomDirection) {
			damageNumberRectTransform.anchoredPosition += Random.insideUnitCircle * newMaxRadiusToInstantiate;
		}

		Vector2 currentPosition = damageNumberRectTransform.anchoredPosition;
		Vector2 targetPosition = currentPosition + Vector2.up * newMovementAmount;

		if (useRandomDirection) {
			targetPosition = currentPosition + getRandomDirection () * newMovementAmount;
		}

		if (useProjectileDirection && damage) {
			targetPosition = currentPosition + direction * newMovementAmount;
		}

		bool targetReached = false;

		currentDistance = 0;

		while (!targetReached) {
			if (damageNumberRectTransform != null) {
				damageNumberRectTransform.anchoredPosition = Vector2.MoveTowards (damageNumberRectTransform.anchoredPosition, targetPosition, Time.deltaTime * movementSpeed);

				currentDistance = GKC_Utils.distance (damageNumberRectTransform.anchoredPosition, targetPosition);

				if (currentDistance < 0.1f) {
					targetReached = true;
				}
			} else {
				targetReached = true;
			}

			yield return null;
		}

		if (!useRandomDirection) {
			targetReached = false;

			currentDistance = 0;

			if (damageNumberRectTransform != null) {
				currentPosition = damageNumberRectTransform.anchoredPosition;
				targetPosition = currentPosition - Vector2.up * (newMovementAmount * downMovementAmount);
			}

			while (!targetReached) {
				if (damageNumberRectTransform != null) {						
					damageNumberRectTransform.anchoredPosition = Vector2.MoveTowards (damageNumberRectTransform.anchoredPosition, targetPosition, Time.deltaTime * movementSpeed);

					currentDistance = GKC_Utils.distance (damageNumberRectTransform.anchoredPosition, targetPosition);

					if (currentDistance < 0.1f) {
						targetReached = true;
					}
				} else {
					targetReached = true;
				}

				yield return null;
			}
		}

		if (damageNumberRectTransform != null) {
			Destroy (damageNumberRectTransform.gameObject);
		}
	}

	public Vector2 getRandomDirection ()
	{
		return new Vector2 (Random.Range (-1f, 1f), Random.Range (-1f, 1f));
	}
}