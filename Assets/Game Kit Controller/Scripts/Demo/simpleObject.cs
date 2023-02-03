using UnityEngine;
using System.Collections;

public class simpleObject : MonoBehaviour
{
	public string animationName = "piston";
	public bool working;

	Animation mainAnimation;

	void Start ()
	{
		mainAnimation = GetComponent<Animation> ();
	}


	void Update ()
	{
		if (working) {
			if (!mainAnimation.IsPlaying (animationName)) {
				if (mainAnimation [animationName].speed == -1) {
					mainAnimation [animationName].speed = 1;
				} else {
					mainAnimation [animationName].speed = -1; 
					mainAnimation [animationName].time = mainAnimation [animationName].length;
				}
				mainAnimation.Play (animationName);
			}
		}
	}

	public void activate ()
	{
		working = true;
	}

	public void changeState ()
	{
		working = !working;
	}
}
