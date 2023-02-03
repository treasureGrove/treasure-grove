using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animatorTriggerEvent : StateMachineBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool eventEnabled = true;

	[Space]

	public string eventMessage;

	[Space]

	public bool useMessageParameter = true;
	public string eventMessageParameter;

	[Space]
	[Header ("Remote Animator Trigger Event Settings")]
	[Space]

	public bool useRemoteAnimatorTriggerEventSystem;
	public string RemoteAnimatorTriggerEventName;

	// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	public override void OnStateEnter (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (eventEnabled) {
			if (useRemoteAnimatorTriggerEventSystem) {
				remoteAnimatorEventTriggerSystem currentRemoteAnimatorEventTriggerSystem = animator.GetComponent<remoteAnimatorEventTriggerSystem> ();

				if (currentRemoteAnimatorEventTriggerSystem != null) {
					currentRemoteAnimatorEventTriggerSystem.callRemoteEvent (RemoteAnimatorTriggerEventName);
				}
			} else {
				if (useMessageParameter) {
					animator.SendMessage (eventMessage, eventMessageParameter, SendMessageOptions.DontRequireReceiver);
				} else {
					animator.SendMessage (eventMessage, SendMessageOptions.DontRequireReceiver);
				}
			}
		}
	}

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	//override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	//override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
	//override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
	//override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}
}
