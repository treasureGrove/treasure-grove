using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class putGear : MonoBehaviour
{
	[Header ("Debug")]
	[Space]

	public bool disposed = false;
	public bool engaged = false;

	[Space]
	[Header ("Objects Settings")]
	[Space]

	public GameObject initPos;
	public GameObject finalPos;
	public GameObject parent;

	[Space]
	[Header ("Events Settings")]
	[Space]

	public UnityEvent eventOnGearPlaced;

	GameObject gear;
	Vector3 movePos;
	grabbedObjectState currentGrabbedObject;

	void Start ()
	{
		movePos = initPos.transform.position;
	}

	//it set the green gear of the scene in the gap of the engine
	void Update ()
	{
		//move the gear from its position to the position inside the mechanism
		if (disposed) {
			gear.transform.position = Vector3.MoveTowards (gear.transform.position, movePos, Time.deltaTime * 5);

			if (gear.transform.position == movePos && !engaged) {
				movePos = finalPos.transform.position;
				engaged = true;
			}

			if (gear.transform.position == movePos && engaged) {
				engaged = false;

				this.enabled = false;
			}
		}
	}

	public void checkToDropObjectOnRail ()
	{
		currentGrabbedObject = gear.GetComponent<grabbedObjectState> ();

		if (currentGrabbedObject != null) {
			GKC_Utils.dropObject (currentGrabbedObject.getCurrentHolder (), gear);
		}
	}

	//when the gear touchs the trigger,
	void OnTriggerEnter (Collider Other)
	{
		rotatoryGear currentRotatoryGear = Other.GetComponent<rotatoryGear> ();

		if (currentRotatoryGear != null) {
			//drop the gear and move and rotate smoothly the gear inside the mechanism
			gear = Other.gameObject;

			checkToDropObjectOnRail ();

//			gear.tag = "Untagged";

			Destroy (gear.GetComponent<Rigidbody> ());

			gear.transform.SetParent (parent.transform);

			eventOnGearPlaced.Invoke ();

			StartCoroutine (rotateGear ());
		}
	}

	//rotate the gear to the corret position inside the mechanism
	IEnumerator rotateGear ()
	{
		for (float t = 0; t < 1;) {
			t += Time.deltaTime;

			gear.transform.rotation = Quaternion.Slerp (gear.transform.rotation, finalPos.transform.rotation, t);
		}

		yield return null;

		disposed = true;
	}
}