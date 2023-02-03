using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(eventTriggerSystem))]
[CanEditMultipleObjects]
public class eventTriggerSystemEditor : Editor
{
	SerializedProperty useSameFunctionInList;
	SerializedProperty sameFunctionList;
	SerializedProperty useSameObjectToCall;
	SerializedProperty callThisObject;
	SerializedProperty sameObjectToCall;
	SerializedProperty useObjectToTrigger;
	SerializedProperty objectNeededToTrigger;
	SerializedProperty useTagToTrigger;
	SerializedProperty useTagList;
	SerializedProperty tagList;
	SerializedProperty tagNeededToTrigger;
	SerializedProperty dontUseDelay;
	SerializedProperty useSameDelay;
	SerializedProperty generalDelay;
	SerializedProperty useRandomDelay;
	SerializedProperty randomDelayRange;
	SerializedProperty triggeredByButton;
	SerializedProperty triggerEventType;
	SerializedProperty useLayerMask;
	SerializedProperty layerMask;
	SerializedProperty justCallOnTrigger;
	SerializedProperty callFunctionEveryTimeTriggered;
	SerializedProperty eventTriggered;
	SerializedProperty coroutineActive;
	SerializedProperty setParentToNull;
	SerializedProperty triggerEventAtStart;
	SerializedProperty eventList;
	SerializedProperty enterEventList;
	SerializedProperty exitEventList;

	eventTriggerSystem manager;
	bool expanded;

	GUIStyle buttonStyle = new GUIStyle ();

	void OnEnable ()
	{
		useSameFunctionInList = serializedObject.FindProperty ("useSameFunctionInList");
		sameFunctionList = serializedObject.FindProperty ("sameFunctionList");
		useSameObjectToCall = serializedObject.FindProperty ("useSameObjectToCall");
		callThisObject = serializedObject.FindProperty ("callThisObject");
		sameObjectToCall = serializedObject.FindProperty ("sameObjectToCall");
		useObjectToTrigger = serializedObject.FindProperty ("useObjectToTrigger");
		objectNeededToTrigger = serializedObject.FindProperty ("objectNeededToTrigger");
		useTagToTrigger = serializedObject.FindProperty ("useTagToTrigger");
		useTagList = serializedObject.FindProperty ("useTagList");
		tagList = serializedObject.FindProperty ("tagList");
		tagNeededToTrigger = serializedObject.FindProperty ("tagNeededToTrigger");
		dontUseDelay = serializedObject.FindProperty ("dontUseDelay");
		useSameDelay = serializedObject.FindProperty ("useSameDelay");
		generalDelay = serializedObject.FindProperty ("generalDelay");
		useRandomDelay = serializedObject.FindProperty ("useRandomDelay");
		randomDelayRange = serializedObject.FindProperty ("randomDelayRange");
		triggeredByButton = serializedObject.FindProperty ("triggeredByButton");
		triggerEventType = serializedObject.FindProperty ("triggerEventType");
		useLayerMask = serializedObject.FindProperty ("useLayerMask");
		layerMask = serializedObject.FindProperty ("layerMask");
		justCallOnTrigger = serializedObject.FindProperty ("justCallOnTrigger");
		callFunctionEveryTimeTriggered = serializedObject.FindProperty ("callFunctionEveryTimeTriggered");
		eventTriggered = serializedObject.FindProperty ("eventTriggered");
		coroutineActive = serializedObject.FindProperty ("coroutineActive");
		setParentToNull = serializedObject.FindProperty ("setParentToNull");
		triggerEventAtStart = serializedObject.FindProperty ("triggerEventAtStart");
		eventList = serializedObject.FindProperty ("eventList");
		enterEventList = serializedObject.FindProperty ("enterEventList");
		exitEventList = serializedObject.FindProperty ("exitEventList");

		manager = (eventTriggerSystem)target;
	}

	public override void OnInspectorGUI ()
	{
		GUILayout.BeginVertical (GUILayout.Height (30));

		EditorGUILayout.Space ();

		buttonStyle = new GUIStyle (GUI.skin.button);

		buttonStyle.fontStyle = FontStyle.Bold;
		buttonStyle.fontSize = 12;

		GUILayout.BeginVertical ("Use Same Function To Call Settings", "window");
		EditorGUILayout.PropertyField (useSameFunctionInList);
		if (useSameFunctionInList.boolValue) {
			
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Same Function List", "window");
			showSimpleList (sameFunctionList, "Function");
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Use Same Object To Call Settings", "window");
		EditorGUILayout.PropertyField (useSameObjectToCall);
		if (useSameObjectToCall.boolValue) {
			EditorGUILayout.PropertyField (callThisObject);
			if (!callThisObject.boolValue) {
				EditorGUILayout.PropertyField (sameObjectToCall);
			}
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Use Object To Trigger Settings", "window");
		EditorGUILayout.PropertyField (useObjectToTrigger);
		if (useObjectToTrigger.boolValue) {
			EditorGUILayout.PropertyField (objectNeededToTrigger);
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Tag Settings", "window");
		EditorGUILayout.PropertyField (useTagToTrigger);
		if (useTagToTrigger.boolValue) {
			EditorGUILayout.PropertyField (useTagList);
			if (useTagList.boolValue) {
				showSimpleList (tagList, "Tag");
			} else {
				EditorGUILayout.PropertyField (tagNeededToTrigger);
			}
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Same Delay Settings", "window");
		EditorGUILayout.PropertyField (dontUseDelay);

		EditorGUILayout.PropertyField (useSameDelay);
		if (useSameDelay.boolValue) {
			EditorGUILayout.PropertyField (generalDelay);

			EditorGUILayout.PropertyField (useRandomDelay);	
			if (useRandomDelay.boolValue) {
				EditorGUILayout.PropertyField (randomDelayRange);	
			}	
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Trigger Settings", "window");
		EditorGUILayout.PropertyField (triggeredByButton);
		if (!triggeredByButton.boolValue) {
			EditorGUILayout.PropertyField (triggerEventType);
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Layer Settings", "window");
		EditorGUILayout.PropertyField (useLayerMask);
		if (useLayerMask.boolValue) {
			EditorGUILayout.PropertyField (layerMask);
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Other Settings", "window");
		EditorGUILayout.PropertyField (justCallOnTrigger);
		EditorGUILayout.PropertyField (callFunctionEveryTimeTriggered);
		EditorGUILayout.PropertyField (setParentToNull);
		EditorGUILayout.PropertyField (triggerEventAtStart);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Debug State", "window");
		if (!callFunctionEveryTimeTriggered.boolValue) {
			GUILayout.Label ("Event Triggered \t" + eventTriggered.boolValue);
		}
		GUILayout.Label ("Coroutine Active \t" + coroutineActive.boolValue);

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Activate Event (Ingame)", buttonStyle)) {
			if (Application.isPlaying) {
				manager.activateEvent ();
			}
		}

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		if (triggerEventType.enumValueIndex == 0) {
			GUILayout.BeginVertical ("Event Trigger List", "window");
			showList (eventList, "Event Trigger List");
			GUILayout.EndVertical ();
		} else if (triggerEventType.enumValueIndex == 1) {
			GUILayout.BeginVertical ("Exit Event Trigger List", "window");
			showList (exitEventList, "Exit Event Trigger List");
			GUILayout.EndVertical ();
		} else {
			GUILayout.BeginVertical ("Enter Event Trigger List", "window");
			showList (enterEventList, "Enter Event Trigger List");
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Exit Event Trigger List", "window");
			showList (exitEventList, "Exit Event Trigger List");
			GUILayout.EndVertical ();
		}

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();
		if (GUI.changed) {
			serializedObject.ApplyModifiedProperties ();
		}
	}

	void showEventInfo (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("name"));

		if (!useSameObjectToCall.boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("objectToCall"));	
		}

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Event Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useEventFunction"));
		if (!list.FindPropertyRelative ("useEventFunction").boolValue) {
			
			if (!useSameFunctionInList.boolValue) {
				showSimpleList (list.FindPropertyRelative ("functionNameList"), "Function");

				EditorGUILayout.PropertyField (list.FindPropertyRelative ("useBroadcastMessage"));	
				if (list.FindPropertyRelative ("useBroadcastMessage").boolValue) {
					showSimpleList (list.FindPropertyRelative ("broadcastMessageStringList"), "Message");
				}
			}
		} else {
			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventFunction"));
		}
		GUILayout.EndVertical ();
	
		if (!useSameDelay.boolValue && !dontUseDelay.boolValue) {
			
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Delay Settings", "window");
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("secondsDelay"));

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useRandomDelay"));	
			if (list.FindPropertyRelative ("useRandomDelay").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("randomDelayRange"));	
			}	
			GUILayout.EndVertical ();
		}

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Send Object Detected Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("sendGameObject"));	
		if (list.FindPropertyRelative ("sendGameObject").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("objectToSend"));
		}
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("sendObjectDetected"));	

		if (list.FindPropertyRelative ("sendObjectDetected").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("sendObjectDetectedByEvent"));	
			if (list.FindPropertyRelative ("sendObjectDetectedByEvent").boolValue) {

				EditorGUILayout.Space ();

				EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventToSendObjectDetected"));
			} else {
				if (list.FindPropertyRelative ("sendObjectDetected").boolValue) {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("sendObjectDetectedFunction"));
				}
			}
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Remote Events Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useRemoteEvent"));	
		if (list.FindPropertyRelative ("useRemoteEvent").boolValue) {

			EditorGUILayout.Space ();

			showSimpleList (list.FindPropertyRelative ("remoteEventNameList"), "Remote Event");
		}
		GUILayout.EndVertical ();
		GUILayout.EndVertical ();
	}

	void showList (SerializedProperty list, string listName)
	{
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + listName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {
			
			EditorGUILayout.Space ();

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Events: \t" + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Event")) {
				list.arraySize++;
			}
			if (GUILayout.Button ("Clear")) {
				list.arraySize = 0;
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Expand All")) {
				for (int i = 0; i < list.arraySize; i++) {
					list.GetArrayElementAtIndex (i).isExpanded = true;
				}
			}
			if (GUILayout.Button ("Collapse All")) {
				for (int i = 0; i < list.arraySize; i++) {
					list.GetArrayElementAtIndex (i).isExpanded = false;
				}
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			for (int i = 0; i < list.arraySize; i++) {
				expanded = false;
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), false);
					if (list.GetArrayElementAtIndex (i).isExpanded) {
						expanded = true;
						showEventInfo (list.GetArrayElementAtIndex (i));
					}

					EditorGUILayout.Space ();

					GUILayout.EndVertical ();
				}
				GUILayout.EndHorizontal ();
				if (expanded) {
					GUILayout.BeginVertical ();
				} else {
					GUILayout.BeginHorizontal ();
				}
				if (GUILayout.Button ("x")) {
					list.DeleteArrayElementAtIndex (i);
				}
				if (GUILayout.Button ("+")) {
					manager.InsertEventAtIndex (i);
				}
				if (GUILayout.Button ("v")) {
					if (i >= 0) {
						list.MoveArrayElement (i, i + 1);
					}
				}
				if (GUILayout.Button ("^")) {
					if (i < list.arraySize) {
						list.MoveArrayElement (i, i - 1);
					}
				}
				if (expanded) {
					GUILayout.EndVertical ();
				} else {
					GUILayout.EndHorizontal ();
				}
				GUILayout.EndHorizontal ();
			}
		}       
	}

	void showSimpleList (SerializedProperty list, string listName)
	{
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + listName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {
			GUILayout.BeginVertical ("box");

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of " + listName + "s: \t" + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add " + listName)) {
				list.arraySize++;
			}
			if (GUILayout.Button ("Clear")) {
				list.arraySize = 0;
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			for (int i = 0; i < list.arraySize; i++) {
				GUILayout.BeginHorizontal ();
				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), new GUIContent ("", null, ""), false);
				}
				GUILayout.BeginHorizontal ();
				if (GUILayout.Button ("x")) {
					list.DeleteArrayElementAtIndex (i);
				}
				if (GUILayout.Button ("v")) {
					if (i >= 0) {
						list.MoveArrayElement (i, i + 1);
					}
				}
				if (GUILayout.Button ("^")) {
					if (i < list.arraySize) {
						list.MoveArrayElement (i, i - 1);
					}
				}
				GUILayout.EndHorizontal ();
				GUILayout.EndHorizontal ();

				EditorGUILayout.Space ();

			}
			GUILayout.EndVertical ();
		}       
	}
}
#endif
