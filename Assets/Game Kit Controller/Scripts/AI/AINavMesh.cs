using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

using UnityEngine.Events;

public class AINavMesh : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public float walkSpeed = 1;

	public float targetChangeTolerance = 1;

	public float patrolSpeed;

	public float maxDistanceToRecalculatePath = 1;

	public bool smoothSpeedWhenCloseToTarget = true;

	public float updateNavMeshPositionRate = 0.5f;
	public float calculatePathRate = 0.5f;

	public bool getClosestPositionIfTargetCantBeReached = true;

	public bool ignoreLookAtTargetDirection;

	public float minDistanceToTargetNotVisibleMultiplier = 0.5f;
	public float minDistanceToTargetNotVisibleClamp = 1.5f;

	public bool useMinDistanceOffsetItTargetReached;
	public float currentDistanceToTargetOffset;

	[Space]
	[Header ("Regular Min Distances Settings")]
	[Space]

	public float minDistanceToFriend;
	public float minDistanceToEnemy;
	public float minDistanceToObjects;

	public float minDistanceToMoveBack;

	[Space]
	[Header ("Jump Settings")]
	[Space]

	public bool checkOffMeshLinkJumpsEnabled = true;

	public bool checkDistanceOnOffMeshLink;

	public float minDistanceOnOffMeshLink;

	public bool checkHorizontalDistanceOnOffMeshLink;

	public float minHorizontalDistanceOnOffMeshLink;

	public bool checkVerticalDistanceOnOffMeshLink;

	public float minVerticalDistanceOnOffMeshLink;

	public bool adjustJumpPowerToDistance = true;

	public float regularJumpPower = 6;

	public float maxJumpPower = 15;

	public float jumpMultiplierPercentage = 0.5f;

	[Space]
	[Header ("Run From Target Settings")]
	[Space]

	public float minDistanceToRunFromTarget = 10;

	[Space]
	[Header ("Inital Target Settings")]
	[Space]

	public bool setInitialTarget;
	public Transform initialTargetTransform;
	public bool setMainPlayerAsTarget;

	[Space]
	[Header ("Partner Settings")]
	[Space]

	public string addAIToFriendListManagerEventName = "Add AI To Friend List Manager";
	public string removeAIToFriendListManagerEventName = "Remove AI From Friend List Manager";

	public bool checkToLeaveSpaceToPartner;
	public float minDistanceWithPartner;
	public float maxDistanceToReturnWithPartner;

	public bool addAIAsPlayerPartner = true;

	[Space]
	[Header ("Guide Partner Settings")]
	[Space]

	public float maxDistanceToPartnerWhenGuidingHim = 10;
	public float targetToGuideOutOfRangeLerpSpeed = 10;
	public float distanceOffsetToSetGuideBackToRange = 1;
	public bool returnToTargetToGuideWhenOutOfRange;
	public float maxDistanceToReturnToTargetToGuide;

	[Space]
	[Header ("Dynamic Obstacle Detection Settings")]
	[Space]

	public bool useDynamicObstacleDetection;
	public LayerMask dynamicObstacleLayer;
	public float dynamicAvoidSpeed = 4;
	public float dynamicRaycastDistanceForward = 3;
	public float dynamicRaycastDistanceDiagonal = 0.5f;
	public float dynamicRaycastDistanceSides = 1;
	public float dynamicRaycastDistanceAround = 4;

	[Space]

	public float minObstacleRotation = 45;
	public float mediumObstacleRotation = 65;
	public float maximumObstacleRotation = 85;

	[Space]
	[Header ("Event Settings")]
	[Space]

	public UnityEvent eventToPauseAI;
	public UnityEvent eventToResumeAI;

	public UnityEvent eventOnPartnerFound;

	public UnityEvent eventOnDeath;

	public UnityEvent eventOnAlive;

	[Space]
	[Header ("AI State")]
	[Space]

	public Transform currentTarget;

	public bool targetIsFriendly;
	public bool targetIsObject;

	public bool navMeshPaused;
	public bool patrolling;
	public bool patrollingPaused;

	public bool attacking;
	public bool following;
	public bool waiting;
	public bool hiding;

	public bool followingTarget;

	public bool usingSamplePosition;

	public bool checkingToResumeNavmesh;

	public bool onGround;

	public bool canReachCurrentTarget;

	public float currentDistanceToTarget;

	public float currentMinDistance;

	public bool useCustomMinDistance;
	public float customMinDistance;

	public bool followingPartner;

	public bool partnerLocated;

	public bool leavingSpaceToPartner;

	public bool AIDead;

	public bool obstacleOnForward;
	public bool obstacleOnRight;
	public bool obstacleOnLeft;
	public bool obstacleOnRightForward;
	public bool obstacleOnLeftForward;

	public int numberOfObstacleDirectionDetected;

	public float obstacleDistance;

	public bool moveNavMeshPaused;

	public float navSpeed = 1;

	public bool useCustomNavMeshSpeed;
	public float customNavMeshSpeed;

	public bool strafeModeActive;

	public AINavMeshMoveInfo AIMoveInput = new AINavMeshMoveInfo ();

	public bool useHalfMinDistance;

	public bool runFromTarget;

	public bool rangeToTargetReached;

	[Space]
	[Header ("Guide Target Debug")]
	[Space]

	public bool guidingPartnerActive;
	public bool partnerToGuideOnRange;
	public float currentDistanceToPartnerToGuide;
	public bool returningToTargetToGuideActive;
	public Transform currentPositionTransformToGuide;

	public Transform temporalTargetPositionTransform;

	[Space]
	[Header ("Debug Settings")]
	[Space]

	public bool showDynamicObstacleDebugPrint;

	public bool pauseMovementToTarget;

	[Space]
	[Header ("Gizmo Settings")]
	[Space]

	public bool showGizmo;
	public bool showPathRenderer;

	[Space]
	[Header ("Element Settings")]
	[Space]

	public NavMeshAgent agent;
	public playerController playerControllerManager;
	public playerCamera playerCameraManager;

	public Transform rayCastPosition;
	public GameObject AIGameObject;

	Transform AITransform;

	Vector3 currentTargetPosition;
	Color currentLineRendererColor = Color.white;

	//OffMeshLinkData _currLink;
	bool lookingPathAfterJump;

	float lastTimeJumpDetected;

	LineRenderer lineRenderer;
	bool lineRendererAdded;
	bool lineRendererActive;

	Transform partner;
	Vector3 targetOffset;

	float originalMinDistanceToFriend;
	float originalMinDistanceToEnemy;

	Vector3 lastTargetPosition;
	NavMeshPath currentNavMeshPath;

	public NavMeshPathStatus currentNavMeshPathStatus;


	List<Vector3> pathCorners = new List<Vector3> ();
	Vector3 lastPathTargetPosition;
	NavMeshHit hit;
	Vector3 currentDestinationPosition;
	Vector3 samplePosition;
	float positionOffset = 0.1f;
	Coroutine waitToResumeCoroutine;
	float lastTimeCalculatePath;
	float lastTimeUpdateNavMeshPosition;

	Vector3 currentPosition;
	Vector3 targetCurrentPosition;

	Vector3 targetTransformUp;

	Vector3 currentUp;
	Vector3 currentForward;

	remoteEventSystem partnerRemoteEventSystem;

	RaycastHit rayhit;

	bool obstacleDetected;
	bool moveToRight = true;

	Vector3 avoidDirection;

	Vector3 currentCheckObstalceDirection;

	Vector3 newMoveDirection;
	Vector3 currentMoveDirection;

	bool checkForPartnerToGuidDistance;
	Vector3 lastMoveDirection;

	Vector3 desiredVelocity;

	bool originalUseDynamicObstacleDetection;

	Vector3 currentMovementDirection;
	bool calculateMoveValueWithDirectionToTargetActive;

	bool warpPositionAssigned;

	bool moveToTargetActive;

	Vector3 Vector3Zero = Vector3.zero;

	Vector3 Vector3Up = Vector3.up;

	float originalAIWalkSpeed;

	int pathCornersCount;

	Vector3 currentNavmeshOffLinkEndPos = Vector3.zero;

	void Start ()
	{
		if (AIGameObject == null) {
			AIGameObject = gameObject;
		}

		AITransform = AIGameObject.transform;

		if (showPathRenderer) {
			addLineRendererGizmo ();
		}

		originalMinDistanceToFriend = minDistanceToFriend;
		originalMinDistanceToEnemy = minDistanceToEnemy;

		originalUseDynamicObstacleDetection = useDynamicObstacleDetection;

		agent.Warp (AITransform.position);

		originalAIWalkSpeed = walkSpeed;
	}

	bool initialTargetChecked;

	void Update ()
	{
		if (!navMeshPaused) {

			if (!initialTargetChecked) {
				if (setInitialTarget) {
					Transform newTarget = initialTargetTransform;

					if (setMainPlayerAsTarget) {
						newTarget = GKC_Utils.findMainPlayerTransformOnScene ();
					} 

					if (newTarget != null) {
						setTarget (newTarget);
					}
				}
		
				initialTargetChecked = true;
			}

			if (onGround) {
				if (lookingPathAfterJump) {
					if (Time.time > lastTimeJumpDetected + 0.6f) {
						lookingPathAfterJump = false;

						jumpEnded ();
					}
				}
			}

			if (checkingToResumeNavmesh) {
				if (onGround) {
					agent.isStopped = false;
					checkingToResumeNavmesh = false;
					usingSamplePosition = false;

					return;
				} else {
					return;
				}
			}

			if (guidingPartnerActive && !returningToTargetToGuideActive && !attacking && (targetIsFriendly || targetIsObject)) {
				if (currentPositionTransformToGuide != null && (currentTarget == null || currentTarget != currentPositionTransformToGuide)) {
					setTarget (currentPositionTransformToGuide);
				}
			}

			if (currentTarget != null && agent.enabled) {

				if (partnerLocated) {
					if (currentTarget == partner) {
						followingPartner = true;
					} else {
						followingPartner = false;
					}
				} else {
					followingPartner = false;
				}

				currentPosition = AITransform.position;

				targetCurrentPosition = currentTarget.position;

				targetTransformUp = currentTarget.up;

				currentDistanceToTarget = GKC_Utils.distance (targetCurrentPosition, currentPosition);

				if (followingPartner) {

					if (checkToLeaveSpaceToPartner) {
			
						if (currentDistanceToTarget < minDistanceWithPartner) {

							if (!leavingSpaceToPartner) {

								bool temporalPositionFound = false;

								float randomDirection = Random.Range (0, 2);

								if (randomDirection == 1) {
									randomDirection = 1;
								} else {
									randomDirection = -1;
								}

								Vector3 temporalPosition = partner.position + (3 * randomDirection) * partner.right + partner.forward * 1.5f;

								updateCurrentNavMeshPath (temporalPosition);

								if (temporalTargetPositionTransform == null) {
									GameObject newTemporalTargetPositionGameObject = new GameObject ();
									newTemporalTargetPositionGameObject.name = "Temporal Target Position Transform";

									temporalTargetPositionTransform = newTemporalTargetPositionGameObject.transform;
								}

								currentNavMeshPathStatus = currentNavMeshPath.status;
						
								if (currentNavMeshPath != null && currentNavMeshPathStatus == NavMeshPathStatus.PathComplete) {
									temporalTargetPositionTransform.position = temporalPosition;

									temporalPositionFound = true;
								} else {
									temporalPosition = partner.position - (3 * randomDirection) * partner.right + partner.forward * 1.5f;

									updateCurrentNavMeshPath (temporalPosition);

									if (currentNavMeshPath != null && currentNavMeshPathStatus == NavMeshPathStatus.PathComplete) {
										temporalTargetPositionTransform.position = temporalPosition;

										temporalPositionFound = true;
									}
								}
									
								if (temporalPositionFound) {
									leavingSpaceToPartner = true;
								}
							}
						}

						if (leavingSpaceToPartner) {
							targetCurrentPosition = temporalTargetPositionTransform.position;

							targetTransformUp = temporalTargetPositionTransform.up;

							if (currentDistanceToTarget > maxDistanceToReturnWithPartner || GKC_Utils.distance (targetCurrentPosition, currentPosition) < 0.3f) {
								leavingSpaceToPartner = false;
							}
						}
					}
				}

				if (patrolling) {
					currentTargetPosition = targetCurrentPosition + targetTransformUp * positionOffset;

					setFollowingTargetState (true);

					if (patrollingPaused) {
						navSpeed = 0;
					} else {
						navSpeed = patrolSpeed;
					}
				} else {
					if (runFromTarget) {
						Vector3 direction = currentPosition - targetCurrentPosition;
						direction = direction / direction.magnitude;
					
						currentTargetPosition = currentPosition + direction * minDistanceToRunFromTarget;

						// use the values to move the character
						if (smoothSpeedWhenCloseToTarget) {
							navSpeed = 20 / currentDistanceToTarget;
							navSpeed = Mathf.Clamp (navSpeed, 0.1f, 1);
						} else {
							navSpeed = 1;
						}

						setFollowingTargetState (true);
					} else {
						if (targetIsObject) {
							currentMinDistance = minDistanceToObjects;
						} else {
							if (targetIsFriendly) {
								currentMinDistance = minDistanceToFriend;
							} else {
								currentMinDistance = minDistanceToEnemy;
							}
						}

						if (useCustomMinDistance) {
							currentMinDistance = customMinDistance;
						}

						if (useHalfMinDistance) {
							currentMinDistance *= minDistanceToTargetNotVisibleMultiplier;

							if (currentMinDistance < minDistanceToTargetNotVisibleClamp) {
								currentMinDistance = minDistanceToTargetNotVisibleClamp;
							}

							useHalfMinDistance = false;
						}

						moveToTargetActive = false;

						if (rangeToTargetReached) {
							if (currentDistanceToTarget > (currentMinDistance + currentDistanceToTargetOffset)) {
								moveToTargetActive = true;
							}
						} else {
							if (currentDistanceToTarget > currentMinDistance) {
								moveToTargetActive = true;
							}
						}

						if (moveToTargetActive || leavingSpaceToPartner) {
							// update the progress if the character has made it to the previous target
							//if ((target.position - targetPos).magnitude > targetChangeTolerance) {
							currentTargetPosition = targetCurrentPosition + targetTransformUp * positionOffset;

							// use the values to move the character
							if (smoothSpeedWhenCloseToTarget && !leavingSpaceToPartner) {
								navSpeed = currentDistanceToTarget / 20;
								navSpeed = Mathf.Clamp (navSpeed, 0.1f, 1);
							} else {
								navSpeed = 1;
							}

							setFollowingTargetState (true);

							rangeToTargetReached = false;

						} else if (currentDistanceToTarget < minDistanceToMoveBack) {
							// update the progress if the character has made it to the previous target
							Vector3 direction = currentPosition - targetCurrentPosition;
							targetOffset = direction / currentDistanceToTarget;
							currentTargetPosition = targetCurrentPosition + targetTransformUp * positionOffset + (minDistanceToMoveBack + 1) * targetOffset;

							// use the values to move the character
							navSpeed = currentDistanceToTarget / 20;
							navSpeed = Mathf.Clamp (navSpeed, 0.1f, 1);

							setFollowingTargetState (true);
						} else {
							// We still need to call the character's move function, but we send zeroed input as the move param.
							moveNavMesh (Vector3Zero, false, false);

							setFollowingTargetState (false);

							usingSamplePosition = false;

							if (targetIsObject) {
								removeTarget ();
							}

							if (useMinDistanceOffsetItTargetReached) {
								rangeToTargetReached = true;
							}
						}
					}
				}

				if (followingTarget) {
					if ((!targetIsFriendly || targetIsObject) && !patrolling) {
						navSpeed = 1;
					}

					if (useCustomNavMeshSpeed) {
						navSpeed = customNavMeshSpeed;
					}

					if (lookingPathAfterJump) {
						currentTargetPosition = currentNavmeshOffLinkEndPos;

						navSpeed = 1;
					}

					if (!usingSamplePosition) {
						setAgentDestination (currentTargetPosition);
						currentDestinationPosition = currentTargetPosition;
					}
				
					if (!updateCurrentNavMeshPath (currentDestinationPosition) || !canReachCurrentTarget) {
						//print ("first path search not complete");

						bool getClosestPosition = false;

						if (currentNavMeshPath.status != NavMeshPathStatus.PathComplete) {
							getClosestPosition = true;
						}

						if (getClosestPosition && getClosestPositionIfTargetCantBeReached) {
							//get closest position to target that can be reached
							//maybe a for bucle checking every position ofthe corner and get the latest reachable

							Vector3 positionToGet = currentDestinationPosition;

							if (pathCorners.Count > 1) {
								positionToGet = pathCorners [pathCorners.Count - 2];
							}

							usingSamplePosition = true;
						
							if (NavMesh.SamplePosition (positionToGet + Vector3Up, out hit, 4, agent.areaMask)) {
								if (samplePosition != hit.position) {
									samplePosition = hit.position;
									currentDestinationPosition = hit.position;
									updateCurrentNavMeshPath (hit.position);
								}
							}

							lastPathTargetPosition = samplePosition;
						} else {
							usingSamplePosition = false;
						}
					}

					if (usingSamplePosition) {
//						agent.SetDestination (lastPathTargetPosition);
//
//						agent.transform.position = currentPosition;

						setAgentDestination (lastPathTargetPosition);

						updateCurrentNavMeshPath (lastPathTargetPosition);

						if (agent.remainingDistance < 0.1f || GKC_Utils.distance (lastPathTargetPosition, currentTargetPosition) > maxDistanceToRecalculatePath) {
							usingSamplePosition = false;
						}
					}

					if (currentNavMeshPath != null) {
						currentNavMeshPathStatus = currentNavMeshPath.status;

						if (currentNavMeshPathStatus == NavMeshPathStatus.PathComplete) {
							desiredVelocity = agent.desiredVelocity;

							moveNavMesh (desiredVelocity * navSpeed, false, false);

							currentLineRendererColor = Color.white;

							canReachCurrentTarget = true;
						} else if (currentNavMeshPathStatus == NavMeshPathStatus.PathPartial) {
							currentLineRendererColor = Color.yellow;

							if (GKC_Utils.distance (currentPosition, pathCorners [pathCorners.Count - 1]) > 2) {
								desiredVelocity = agent.desiredVelocity;

								moveNavMesh (desiredVelocity * navSpeed, false, false);
							} else {
								moveNavMesh (Vector3Zero, false, false);
							}

							canReachCurrentTarget = false;
						} else if (currentNavMeshPathStatus == NavMeshPathStatus.PathInvalid) {
							currentLineRendererColor = Color.red;

							canReachCurrentTarget = false;
						}
				
						if (checkOffMeshLinkJumpsEnabled) {
							if (agent.isOnOffMeshLink && !lookingPathAfterJump) {
								bool canActivateJump = true;

								currentNavmeshOffLinkEndPos = agent.currentOffMeshLinkData.endPos;

								Vector3 currentOffMeshLinkStartPos = agent.currentOffMeshLinkData.startPos;

								float offMeshLinkDistance = GKC_Utils.distance (currentNavmeshOffLinkEndPos, currentOffMeshLinkStartPos);

								float horizontalOffMeshLinkDistance = 
									GKC_Utils.distance (new Vector3 (currentNavmeshOffLinkEndPos.x, 0, currentNavmeshOffLinkEndPos.z),
										new Vector3 (currentOffMeshLinkStartPos.x, 0, currentOffMeshLinkStartPos.z));

								float verticalOffMeshLinkDistance = Mathf.Abs (currentNavmeshOffLinkEndPos.y - currentOffMeshLinkStartPos.y);

						
//								print (offMeshLinkDistance + " " + verticalOffMeshLinkDistance + " " + horizontalOffMeshLinkDistance);

							
								if (checkDistanceOnOffMeshLink) {
									if (offMeshLinkDistance < minDistanceOnOffMeshLink) {
										canActivateJump = false;
									}
								}

								if (checkVerticalDistanceOnOffMeshLink) {
									if (verticalOffMeshLinkDistance < minVerticalDistanceOnOffMeshLink) {
										canActivateJump = false;
									}
								}

								if (checkHorizontalDistanceOnOffMeshLink) {
									if (horizontalOffMeshLinkDistance < minHorizontalDistanceOnOffMeshLink) {
										canActivateJump = false;
									}
								}

								if (canActivateJump) {
									Debug.DrawRay (currentNavmeshOffLinkEndPos, Vector3.up * 5, Color.white, 5);
									//_currLink = agent.currentOffMeshLinkData;
									lookingPathAfterJump = true;

									desiredVelocity = agent.desiredVelocity;

									agent.autoTraverseOffMeshLink = false;


									if (adjustJumpPowerToDistance) {
										float newJumpPower = regularJumpPower;

										newJumpPower += offMeshLinkDistance * jumpMultiplierPercentage;

										newJumpPower = Mathf.Clamp (newJumpPower, 0, maxJumpPower);

										playerControllerManager.setJumpPower (newJumpPower);
									}


									moveNavMesh (desiredVelocity * navSpeed, false, true);

//									print ("jump detected");

									lastTimeJumpDetected = Time.time;
								}
							} 
						}
					}

					if (showPathRenderer) {
						if (lineRendererAdded) {

							if (!lineRendererActive) {
								lineRenderer.enabled = true;

								lineRendererActive = true;
							}

							lineRenderer.startColor = currentLineRendererColor;
							lineRenderer.endColor = currentLineRendererColor;

							int currentPartCornersCount = pathCorners.Count;

							lineRenderer.positionCount = currentPartCornersCount;

							for (int i = 0; i < currentPartCornersCount; i++) {
								lineRenderer.SetPosition (i, pathCorners [i]);
							}
						} else {
							addLineRendererGizmo ();
						}
					}
				} else {
					if (showPathRenderer) {
						if (lineRendererAdded) {
							if (lineRendererActive) {
								lineRenderer.enabled = false;

								lineRendererActive = false;
							}
						} else {
							addLineRendererGizmo ();
						}
					}
				}
			} else {
				moveNavMesh (Vector3Zero, false, false);

				usingSamplePosition = false;

				if (followingTarget) {
					if (agent.enabled) {
						agent.ResetPath ();
					}

					setFollowingTargetState (false);
				}

				if (showPathRenderer) {
					if (lineRendererAdded) {
						if (lineRendererActive) {
							lineRenderer.enabled = false;

							lineRendererActive = false;
						}
					} else {
						addLineRendererGizmo ();
					}
				}
			}
		} else {
			usingSamplePosition = false;

			if (followingTarget) {
				if (agent.enabled) {
					agent.ResetPath ();
				}

				setFollowingTargetState (false);
			}

			if (showPathRenderer) {
				if (lineRendererAdded) {
					if (lineRendererActive) {
						lineRenderer.enabled = false;

						lineRendererActive = false;
					}
				} else {
					addLineRendererGizmo ();
				}
			}
		}
	}

	void addLineRendererGizmo ()
	{
		if (!lineRendererAdded) {
			lineRenderer = AIGameObject.GetComponent<LineRenderer> ();

			if (lineRenderer == null) {
				lineRenderer = AIGameObject.AddComponent<LineRenderer> ();
				lineRenderer.material = new Material (Shader.Find ("Sprites/Default")) { color = currentLineRendererColor };
				lineRenderer.startWidth = 0.5f;
				lineRenderer.endWidth = 0.5f;
				lineRenderer.startColor = currentLineRendererColor;
				lineRenderer.endColor = currentLineRendererColor;

				lineRendererAdded = true;

				lineRendererActive = true;
			}
		}
	}

	public void setAgentDestination (Vector3 targetPosition)
	{
		if (GKC_Utils.distance (lastTargetPosition, targetPosition) > maxDistanceToRecalculatePath || Time.time > lastTimeUpdateNavMeshPosition + updateNavMeshPositionRate) {
			if (!warpPositionAssigned) {
				agent.Warp (AITransform.position);
				warpPositionAssigned = true;
			}

			lastTargetPosition = targetPosition;
			agent.SetDestination (targetPosition);
			lastTimeUpdateNavMeshPosition = Time.time;
		}

		agent.transform.position = AITransform.position;
	}

	public bool updateCurrentNavMeshPath (Vector3 targetPosition)
	{
		bool hasFoundPath = true;

		if (GKC_Utils.distance (lastPathTargetPosition, targetPosition) > maxDistanceToRecalculatePath || pathCorners.Count == 0 || Time.time > calculatePathRate + lastTimeCalculatePath) {
			lastPathTargetPosition = targetPosition;

			setAgentDestination (lastPathTargetPosition);

			currentNavMeshPath = new NavMeshPath ();

			pathCorners.Clear ();

			hasFoundPath = agent.CalculatePath (lastPathTargetPosition, currentNavMeshPath);

			pathCorners.AddRange (currentNavMeshPath.corners);

			lastTimeCalculatePath = Time.time;
		}

		return hasFoundPath;
	}

	public void setOnGroundState (bool state)
	{
		onGround = state;
	}

	public bool getCanReachCurrentTargetValue ()
	{
		return canReachCurrentTarget;
	}

	public float getRemainingDistanceToTarget ()
	{
		if (followingTarget) {
			pathCornersCount = pathCorners.Count;

			if (pathCornersCount > 1) {
				float currentDistance = GKC_Utils.distance (AITransform.position, pathCorners [1]);

				if (pathCornersCount > 2) {
					for (int i = 2; i < pathCornersCount; ++i) {
						currentDistance += GKC_Utils.distance (pathCorners [i - 1], pathCorners [i]);
					}
				}

				return currentDistance;
			} else {
				return -100;
			}
		} else {
			return -100;
		}
	}

	public Vector3 getMovementDirection ()
	{
		if (pathCorners.Count > 0) {
			if (GKC_Utils.distance (pathCorners [0], AITransform.position) < 0.1f) {
				pathCorners.RemoveAt (0);
			}

			if (pathCorners.Count > 0) {
				Vector3 direction = pathCorners [0] - AITransform.position;
				return direction.normalized;
			}
		}

		return Vector3Zero;
	}

	public Vector3 getCurrentMovementDirection ()
	{
		return currentMovementDirection;
	}

	public void setPauseMoveNavMeshState (bool state)
	{
		moveNavMeshPaused = state;
	}

	public void setNavmeshPausedState (bool state)
	{
		navMeshPaused = state;

//		print ("nav mesh " + navMeshPaused);
	}

	public void setCalculateMoveValueWithDirectionToTargetActiveState (bool state)
	{
		calculateMoveValueWithDirectionToTargetActive = state;
	}

	public bool showMoveInputDebugPrint;

	public virtual void moveNavMesh (Vector3 moveInput, bool crouch, bool jump)
	{
		if (useDynamicObstacleDetection && moveInput != Vector3Zero) {
			currentUp = AITransform.up;
			currentForward = AITransform.forward;

			if (moveNavMeshPaused) {
				moveInput = Vector3Zero;
			}

			if (moveInput != Vector3Zero) {

				Vector3 currentRaycastPosition = AITransform.position + currentUp;

				if (!obstacleDetected) {
					if (Physics.Raycast (currentRaycastPosition, currentForward, out rayhit, dynamicRaycastDistanceForward, dynamicObstacleLayer)) {
						obstacleDetected = true;

						avoidDirection = moveInput;

						currentCheckObstalceDirection = Quaternion.Euler (currentUp * 60) * currentForward;
		
						if (Physics.Raycast (currentRaycastPosition, currentCheckObstalceDirection, out rayhit, dynamicRaycastDistanceForward, dynamicObstacleLayer)) {

							moveToRight = false;
						}
					} else {
						currentCheckObstalceDirection = Quaternion.Euler (currentUp * 60) * currentForward;

						if (Physics.Raycast (currentRaycastPosition, currentCheckObstalceDirection, out rayhit, dynamicRaycastDistanceDiagonal, dynamicObstacleLayer)) {
						
							obstacleDetected = true;

							avoidDirection = moveInput;

							moveToRight = false;
						} else {
							currentCheckObstalceDirection = Quaternion.Euler (-currentUp * 60) * currentForward;

							if (Physics.Raycast (currentRaycastPosition, currentCheckObstalceDirection, out rayhit, dynamicRaycastDistanceDiagonal, dynamicObstacleLayer)) {

								obstacleDetected = true;

								avoidDirection = moveInput;
							
							} else {
								currentCheckObstalceDirection = Quaternion.Euler (currentUp * 90) * currentForward;

								if (Physics.Raycast (currentRaycastPosition, currentCheckObstalceDirection, out rayhit, dynamicRaycastDistanceSides, dynamicObstacleLayer)) {

									obstacleDetected = true;

									avoidDirection = moveInput;

									moveToRight = false;
								} else {
									currentCheckObstalceDirection = Quaternion.Euler (-currentUp * 90) * currentForward;

									if (Physics.Raycast (currentRaycastPosition, currentCheckObstalceDirection, out rayhit, dynamicRaycastDistanceSides, dynamicObstacleLayer)) {

										obstacleDetected = true;

										avoidDirection = moveInput;
									}
								}
							}
						}
					}
				}

				if (obstacleDetected) {
					obstacleOnForward = false;
					obstacleOnRight = false;
					obstacleOnLeft = false;
					obstacleOnRightForward = false;
					obstacleOnLeftForward = false;

					numberOfObstacleDirectionDetected = 0;

					obstacleDistance = 3;

					bool newObstacleFound = false;

					if (pathCorners.Count > 0) {
						currentForward = (pathCorners [pathCorners.Count - 1] - AITransform.position);

						currentForward.Normalize ();
					} else {
						currentForward = moveInput;

						currentForward.Normalize ();
					}

					Vector3 rayDirection = currentForward;

					if (Physics.Raycast (currentRaycastPosition, rayDirection, out rayhit, dynamicRaycastDistanceForward, dynamicObstacleLayer)) {
					
						obstacleDistance = rayhit.distance;

						newObstacleFound = true;

						if (showGizmo) {
							Debug.DrawRay (currentRaycastPosition, rayDirection, Color.red, 2);
						}

						obstacleOnForward = true;

						numberOfObstacleDirectionDetected++;
					} else {
						if (showGizmo) {
							Debug.DrawRay (currentRaycastPosition, rayDirection, Color.green, 2);
						}
					}
					
					rayDirection = Quaternion.Euler (currentUp * 45) * currentForward;

					if (Physics.Raycast (currentRaycastPosition, rayDirection, out rayhit, dynamicRaycastDistanceAround, dynamicObstacleLayer)) {

						newObstacleFound = true;

						if (rayhit.distance < obstacleDistance) {
							obstacleDistance = rayhit.distance;
						}

						if (showGizmo) {
							Debug.DrawRay (currentRaycastPosition, rayDirection, Color.red, 2);
						}

						obstacleOnRightForward = true;

						numberOfObstacleDirectionDetected++;
					} else {
						if (showGizmo) {
							Debug.DrawRay (currentRaycastPosition, rayDirection, Color.green, 2);
						}
					}

					rayDirection = Quaternion.Euler (-currentUp * 45) * currentForward;

					if (Physics.Raycast (currentRaycastPosition, rayDirection, out rayhit, dynamicRaycastDistanceAround, dynamicObstacleLayer)) {

						newObstacleFound = true;

						if (rayhit.distance < obstacleDistance) {
							obstacleDistance = rayhit.distance;
						}

						if (showGizmo) {
							Debug.DrawRay (currentRaycastPosition, rayDirection, Color.red, 2);
						}

						obstacleOnLeftForward = true;

						numberOfObstacleDirectionDetected++;
					} else {
						if (showGizmo) {
							Debug.DrawRay (currentRaycastPosition, rayDirection, Color.green, 2);
						}
					}

					rayDirection = Quaternion.Euler (currentUp * 90) * currentForward;

					if (Physics.Raycast (currentRaycastPosition, rayDirection, out rayhit, dynamicRaycastDistanceAround, dynamicObstacleLayer)) {

						newObstacleFound = true;

						if (rayhit.distance < obstacleDistance) {
							obstacleDistance = rayhit.distance;
						}

						if (showGizmo) {
							Debug.DrawRay (currentRaycastPosition, rayDirection, Color.red, 2);
						}

						obstacleOnRight = true;

						numberOfObstacleDirectionDetected++;
					} else {
						if (showGizmo) {
							Debug.DrawRay (currentRaycastPosition, rayDirection, Color.green, 2);
						}
					}

					rayDirection = Quaternion.Euler (-currentUp * 90) * currentForward;

					if (Physics.Raycast (currentRaycastPosition, rayDirection, out rayhit, dynamicRaycastDistanceAround, dynamicObstacleLayer)) {

						newObstacleFound = true;

						if (rayhit.distance < obstacleDistance) {
							obstacleDistance = rayhit.distance;
						}

						if (showGizmo) {
							Debug.DrawRay (currentRaycastPosition, rayDirection, Color.red, 2);
						}

						obstacleOnLeft = true;

						numberOfObstacleDirectionDetected++;
					} else {
						if (showGizmo) {
							Debug.DrawRay (currentRaycastPosition, rayDirection, Color.green, 2);
						}
					}

					if (obstacleOnLeft || obstacleOnLeftForward || obstacleOnForward) {
						moveToRight = true;
					} else {
						moveToRight = false;
					}


					if (!newObstacleFound) {
						obstacleDetected = false;

					} else {
						float avoidAngle = 30;

						if (obstacleDistance < 3) {
							avoidAngle = minObstacleRotation;
						} 

						if (obstacleDistance < 2) {
							avoidAngle = mediumObstacleRotation;
						} 

						if (obstacleDistance < 1) {
							avoidAngle = maximumObstacleRotation;
						}

						if (!moveToRight) {
							avoidAngle = -avoidAngle;
						}

						Vector3 newDirection = Quaternion.Euler (currentUp * avoidAngle) * avoidDirection;

						newMoveDirection = newDirection;

//						newMoveDirection *= navSpeed;

						if (showDynamicObstacleDebugPrint) {
							print ("obstacle detected, moving to the right is " + moveToRight);
							print ("avoid angle is " + avoidAngle);
						}
					}
				} else {
					newMoveDirection = moveInput;

					if (showDynamicObstacleDebugPrint) {
						print ("no obstacle detected");
					}
				}
			} else {
				newMoveDirection = moveInput;

				if (showDynamicObstacleDebugPrint) {
					print ("no obstacle detected");
				}
			}

			if (currentDistanceToTarget > currentMinDistance + 3) {
				if (newMoveDirection.magnitude > 1) {
					newMoveDirection.Normalize ();
				}
			}

			currentMoveDirection = Vector3.Lerp (currentMoveDirection, newMoveDirection, Time.fixedDeltaTime * dynamicAvoidSpeed);

			moveInput = currentMoveDirection;
		} else {

			if (moveNavMeshPaused) {
				moveInput = Vector3Zero;
			}
		}

		if (guidingPartnerActive && partnerLocated && !attacking && (targetIsFriendly || targetIsObject)) {
			currentDistanceToPartnerToGuide = GKC_Utils.distance (partner.position, currentPosition);

			if (currentTarget != partner) {
				if (!checkForPartnerToGuidDistance) {
					partnerToGuideOnRange = currentDistanceToPartnerToGuide < maxDistanceToPartnerWhenGuidingHim;

					lastMoveDirection = moveInput.normalized;
				}

				if (!partnerToGuideOnRange) {
					lastMoveDirection = Vector3.Lerp (lastMoveDirection, Vector3Zero, Time.deltaTime * targetToGuideOutOfRangeLerpSpeed);

					moveInput = lastMoveDirection;

					checkForPartnerToGuidDistance = true;

					if (currentDistanceToPartnerToGuide < (maxDistanceToPartnerWhenGuidingHim - distanceOffsetToSetGuideBackToRange)) {
						checkForPartnerToGuidDistance = false;
					}

					if (returnToTargetToGuideWhenOutOfRange) {
						if (currentDistanceToPartnerToGuide > maxDistanceToReturnToTargetToGuide) {
							returningToTargetToGuideActive = true;
							setTarget (partner);

							lastMoveDirection = Vector3Zero;
						}
					}
				}
			} else {
				if (currentDistanceToPartnerToGuide < (maxDistanceToPartnerWhenGuidingHim + 0.5f)) {
					returningToTargetToGuideActive = false;

					setTarget (currentPositionTransformToGuide);
				}
			}
		}

		if (moveInput.magnitude < 0.01f) {
			moveInput = Vector3Zero;
		}

		if (pauseMovementToTarget) {
			moveInput = Vector3Zero;
		}

		currentMovementDirection = moveInput;

//		if (moveInput == Vector3Zero) {
//			print ("ZERO");
//		}

		if (showMoveInputDebugPrint) {
			print (moveInput);
		}

		AIMoveInput.moveInput = moveInput * walkSpeed;
		AIMoveInput.crouchInput = crouch;
		AIMoveInput.jumpInput = jump;

		AIMoveInput.AIEnableInputMovementOnStrafe = calculateMoveValueWithDirectionToTargetActive;

		updateAIControllerInputValues ();

		updateAICameraInputValues ();
	}

	public virtual void updateAIControllerInputValues ()
	{
		playerControllerManager.Move (AIMoveInput);
	}

	public virtual void updateAICameraInputValues ()
	{
		playerCameraManager.Rotate (rayCastPosition.forward);
	}

	public void setAIWalkSpeed (float newValue)
	{
		walkSpeed = newValue;
	}

	public void setOriginalAIWalkSpeed (float newValue)
	{
		setAIWalkSpeed (originalAIWalkSpeed);
	}

	public void setResetVerticalCameraRotationActiveState (bool state)
	{
		playerCameraManager.setResetVerticalCameraRotationActiveState (state);
	}

	public void pauseAI (bool state)
	{
		if (AIDead && !state) {
			print ("WARNING: trying to resume a dead AI");

			return;
		}

//		print ("setting AI pause state " + state);

		setNavmeshPausedState (state);

		if (navMeshPaused) {
			if (agent != null) {
				if (agent.enabled) {
					agent.Warp (AITransform.position);

					agent.isStopped = true;
					agent.enabled = false;
				}
			}

			lookAtTaget (false);

			moveNavMesh (Vector3Zero, false, false);
		} else {
			if (!agent.enabled) {
				agent.enabled = true;

				if (waitToResumeCoroutine != null) {
					StopCoroutine (waitToResumeCoroutine);
				}
				waitToResumeCoroutine = StartCoroutine (resumeCoroutine ());
			}
		}

		if (navMeshPaused) {
			eventToPauseAI.Invoke ();
		} else {
			eventToResumeAI.Invoke ();
		}

		if (showPathRenderer) {
			if (lineRendererAdded) {
				if (!state) {
					lineRenderer.enabled = !state;

					lineRendererActive = !state;
				}
			} else {
				addLineRendererGizmo ();
			}
		}

		rangeToTargetReached = false;
	}

	IEnumerator resumeCoroutine ()
	{
		yield return new WaitForSeconds (0.0001f);

		if (agent.enabled) {
			agent.isStopped = false;
		}

		checkingToResumeNavmesh = true;
	}

	//disable ai when it dies
	public void setAIStateToDead ()
	{
		AIDead = true;

		eventOnDeath.Invoke ();
	}

	public void setAIStateToAlive ()
	{
		AIDead = false;

		eventOnAlive.Invoke ();

		agent.Warp (AITransform.position);
	}

	public bool isAIDead ()
	{
		return AIDead;
	}

	public void recalculatePath ()
	{
		if (agent.enabled) {
			agent.isStopped = false;
		}
	}

	public void jumpStart ()
	{
		
	}

	public void jumpEnded ()
	{
		agent.enabled = true;

		agent.CompleteOffMeshLink ();

		//Resume normal navmesh behaviour
		agent.isStopped = false;

		lookingPathAfterJump = false;

		setNavmeshPausedState (false);

		agent.Warp (AITransform.position);

		agent.transform.position = AITransform.position;
	}

	public void setPatrolTarget (Transform newTarget)
	{
		if (patrollingPaused) {
			return;
		}

//		print ("setting patrol point " + newTarget.name);

		setTarget (newTarget);
	}

	public void setTarget (Transform newTarget)
	{
		currentTarget = newTarget;

//		if (currentTarget) {
//			print ("SETTING NEW TARGET ON AI NAVMESH " + newTarget.name);
//		} else {
//			print ("target NULL");
//		}

		rangeToTargetReached = false;
	}

	public Transform getCurrentTarget ()
	{
		return currentTarget;
	}

	public void avoidTarget (Transform targetToAvoid)
	{
		setTarget (targetToAvoid);
	}

	public void setAvoidTargetState (bool state)
	{
		runFromTarget = state;
	}

	public void removeTarget ()
	{
		setTarget (null);
	}

	public void resetCustomSpeedAndDistanceValues ()
	{
		disableCustomNavMeshSpeed ();

		disableCustomMinDistance ();
	}

	public bool anyTargetToReach ()
	{
		return currentTarget != null;
	}

	public void setGuidingPartnerActiveState (bool state)
	{
		guidingPartnerActive = state;

		if (guidingPartnerActive && currentTarget != null) {
			currentPositionTransformToGuide = currentTarget;
		}

		checkForPartnerToGuidDistance = false;
	}

	public void partnerFound (Transform currentPartner)
	{
		partner = currentPartner;

		setTarget (partner);

		setPartnerFoundInfo (partner);
	}

	public void setPartnerFoundInfo (Transform currentPartner)
	{
		partner = currentPartner;

		if (patrolling) {
			patrolling = false;
		}

		eventOnPartnerFound.Invoke ();

		if (addAIAsPlayerPartner) {
			partnerRemoteEventSystem = partner.GetComponent<remoteEventSystem> ();

			if (partnerRemoteEventSystem != null) {
				partnerRemoteEventSystem.callRemoteEventWithGameObject (addAIToFriendListManagerEventName, AIGameObject);
			}
		}

		if (partner != null) {
			partnerLocated = true;
		} else {
			partnerLocated = false;
		}
	}

	public Transform getCurrentPartner ()
	{
		return partner;
	}

	public void removeFromPartnerList ()
	{
		if (addAIAsPlayerPartner) {
			if (partner != null) {
				if (partnerRemoteEventSystem != null) {
					partnerRemoteEventSystem.callRemoteEventWithTransform (removeAIToFriendListManagerEventName, AITransform);
				}
			}
		}
	}

	public void removePartner ()
	{
		if (partner != null) {
			removeFromPartnerList ();

			if (currentTarget == partner) {
				setTarget (null);
			}

			partner = null;

			partnerLocated = false;

			followingPartner = false;
		}
	}

	public void lookAtTaget (bool state)
	{
		if (ignoreLookAtTargetDirection) {
			return;
		}

		//make the character to look or not to look towars its target, pointing the camera towards it
		AIMoveInput.lookAtTarget = state;
	}

	public void setStrafeModeActiveState (bool state)
	{
		AIMoveInput.strafeModeActive = state;
	}

	public void setPatrolState (bool state)
	{
		patrolling = state;
	}

	public void setPatrolPauseState (bool state)
	{
		patrollingPaused = state;
	}

	public bool isPatrolPaused ()
	{
		return patrollingPaused;
	}

	public void setPatrolSpeed (float value)
	{
		patrolSpeed = value;
	}

	public void attack (Transform newTarget)
	{
		setTarget (newTarget);

		setCharacterStates (true, false, false, false);
	}

	public void follow (Transform newTarget)
	{
		setFollowingTargetState (true);

		setTarget (newTarget);

		setCharacterStates (false, true, false, false);
	}

	public void wait (Transform newTarget)
	{
		removeTarget ();

		setCharacterStates (false, false, true, false);
	}

	public void hide (Transform newTarget)
	{
		setTarget (newTarget);

		setCharacterStates (false, false, false, true);
	}

	public void setFollowingTargetState (bool state)
	{
		followingTarget = state;
	}

	public bool isCharacterAttacking ()
	{
		return attacking;
	}

	public bool isCharacterFollowing ()
	{
		return following;
	}

	public bool isCharacterWaiting ()
	{
		return waiting;
	}

	public bool isCharacterHiding ()
	{
		return hiding;
	}

	public void setCharacterStates (bool attackValue, bool followValue, bool waitValue, bool hideValue)
	{
		attacking = attackValue;
		following = followValue;
		waiting = waitValue;
		hiding = hideValue;
	}

	public void setTargetType (bool isFriendly, bool isObject)
	{
		targetIsFriendly = isFriendly;
		targetIsObject = isObject;

		if (targetIsFriendly) {
			setCharacterStates (false, false, false, false);
		}
	}

	public void setExtraMinDistanceState (bool state, float extraDistance)
	{
		if (state) {
			minDistanceToFriend = originalMinDistanceToFriend + extraDistance;
			minDistanceToEnemy = originalMinDistanceToEnemy + extraDistance;
		} else {
			minDistanceToFriend = originalMinDistanceToFriend;
			minDistanceToEnemy = originalMinDistanceToEnemy;
		}
	}

	public bool isFollowingTarget ()
	{
		return followingTarget;
	}

	public void startOverride ()
	{
		overrideTurretControlState (true);
	}

	public void stopOverride ()
	{
		overrideTurretControlState (false);
	}

	public void overrideTurretControlState (bool state)
	{
		pauseAI (state);
	}

	public void setDistanceToEnemy (float newDistance)
	{
		minDistanceToEnemy = newDistance;

		originalMinDistanceToEnemy = minDistanceToEnemy;
	}

	public void setUseDynamicObstacleDetectionState (bool state)
	{
		useDynamicObstacleDetection = state;
	}

	public void setOriginalUseDynamicObstacleDetection ()
	{
		setUseDynamicObstacleDetectionState (originalUseDynamicObstacleDetection);
	}

	public void setMinObstacleRotation (float newValue)
	{
		minObstacleRotation = newValue;
	}

	public void setMediumObstacleRotation (float newValue)
	{
		mediumObstacleRotation = newValue;
	}

	public void setMaximumObstacleRotation (float newValue)
	{
		maximumObstacleRotation = newValue;
	}

	public void enableCustomNavMeshSpeed (float newValue)
	{
		customNavMeshSpeed = newValue;

		useCustomNavMeshSpeed = true;
	}

	public void disableCustomNavMeshSpeed ()
	{
		useCustomNavMeshSpeed = false;
	}

	public void enableCustomMinDistance (float newValue)
	{
		useCustomMinDistance = true;

		customMinDistance = newValue;
	}

	public void enableHalfMinDistance ()
	{
		useHalfMinDistance = true;
	}

	public void disableCustomMinDistance ()
	{
		useCustomMinDistance = false;
	}

	public void setUseMinDistanceOffsetItTargetReachedState (bool state)
	{
		useMinDistanceOffsetItTargetReached = state;
	}

	public void setMinDistanceToEnemyFromEditor (float newDistance)
	{
		minDistanceToEnemy = newDistance;

		updateComponent ();
	}

	public void updateComponent ()
	{
		GKC_Utils.updateComponent (this);
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

	//draw the pivot and the final positions of every door
	void DrawGizmos ()
	{
		if (showGizmo) {
			Gizmos.color = Color.red;
			Gizmos.DrawSphere (lastPathTargetPosition, 1);
			Gizmos.color = Color.green;
			Gizmos.DrawSphere (samplePosition, 1);

			Gizmos.color = Color.gray;

			Gizmos.DrawSphere (currentTargetPosition, 0.2f);
		}
	}
}