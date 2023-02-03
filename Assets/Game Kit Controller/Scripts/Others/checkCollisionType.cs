using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class checkCollisionType : MonoBehaviour
{
	[Space]

	[Header ("Main Settings")]

	public bool checkCollisionsEnabled = true;

	public bool onCollisionEnter;
	public bool onCollisionExit;
	public bool onTriggerEnter;
	public bool onTriggerExit;
	public bool onTriggerStay;

	[Space]

	[Header ("Functions Settings")]

	public string onCollisionEnterFunctionName;
	public string onCollisionExitFunctionName;
	public string onTriggerEnterFunctionName;
	public string onTriggerExitFunctionName;
	public string onTriggerStayFunctionName;

	public GameObject parent;
	public bool active;
	public GameObject objectToCollide;

	[Space]

	[Header ("Regular Event Settings")]

	public bool useEvents;

	public UnityEvent onCollisionEnterEvent = new UnityEvent ();
	public UnityEvent onCollisionExitEvent = new UnityEvent ();
	public UnityEvent onTriggerEnterEvent = new UnityEvent ();
	public UnityEvent onTriggerExitEvent = new UnityEvent ();
	public UnityEvent onTriggerStayEvent = new UnityEvent ();

	[Space]

	[Header ("Events With Objects Settings")]

	public bool useOnCollisionEnterEventWithObject;
	public eventParameters.eventToCallWithGameObject onCollisionEnterEventWithObject;

	public bool useOnCollisionExitEventWithObject;
	public eventParameters.eventToCallWithGameObject onCollisionExitEventWithObject;

	public bool useOnTriggerEnterEventWithObject;
	public eventParameters.eventToCallWithGameObject onTriggerEnterEventWithObject;

	public bool useOnTriggerExitEventWithObject;
	public eventParameters.eventToCallWithGameObject onTriggerExitEventWithObject;

	public bool useOnTriggerStayEventWithObject;
	public eventParameters.eventToCallWithGameObject onTriggerStayEventWithObject;

	//a script to check all the type of collisions of an object, and in that case, send a message to another object according to the type of collision
	//if you want to use a collision enter, check the bool onCollisionEnter in the editor, set the funcion called in the onCollisionEnterFunctionName string
	//and finally set the parent, the object which will receive the function
	//also, you can set an specific object to check a collision with that object
	//the variable active can be used to check when the collision happens

	void Start ()
	{
		if (parent == null) {
			parent = gameObject;
		}
	}

	void OnCollisionEnter (Collision col)
	{
		if (!checkCollisionsEnabled) {
			return;
		}

		checkOnCollision (col, true);
	}

	void OnCollisionExit (Collision col)
	{
		if (!checkCollisionsEnabled) {
			return;
		}

		checkOnCollision (col, false);
	}

	public void checkOnCollision (Collision col, bool isEnter)
	{
		if (isEnter) {
			if (onCollisionEnter) {
				if (objectToCollide) {
					if (col.gameObject == objectToCollide) {
						if (useEvents) {
							callEvent (onCollisionEnterEvent);

							if (useOnCollisionEnterEventWithObject) {
								callEventWithObject (onCollisionEnterEventWithObject, col.gameObject);
							}
						} 

						if (onCollisionEnterFunctionName != "") {
							parent.SendMessage (onCollisionEnterFunctionName, col.gameObject, SendMessageOptions.DontRequireReceiver);
						} 
							
						active = true;
					}
				} else {
					if (useEvents) {
						callEvent (onCollisionEnterEvent);

						if (useOnCollisionEnterEventWithObject) {
							callEventWithObject (onCollisionEnterEventWithObject, col.gameObject);
						}
					} 

					if (onCollisionEnterFunctionName != "") {
						parent.SendMessage (onCollisionEnterFunctionName, col.gameObject, SendMessageOptions.DontRequireReceiver);
					}

					active = true;
				}
			}
		} else {
			if (onCollisionExit) {
				active = true;

				if (useEvents) {
					callEvent (onCollisionExitEvent);

					if (useOnCollisionExitEventWithObject) {
						callEventWithObject (onCollisionExitEventWithObject, col.gameObject);
					}
				} 

				if (onCollisionExitFunctionName != "") {
					parent.SendMessage (onCollisionExitFunctionName, col.gameObject, SendMessageOptions.DontRequireReceiver);
				}
			}
		}
	}

	void OnTriggerEnter (Collider col)
	{
		if (!checkCollisionsEnabled) {
			return;
		}

		checkTrigger (col, true);
	}

	void OnTriggerExit (Collider col)
	{
		if (!checkCollisionsEnabled) {
			return;
		}

		checkTrigger (col, false);
	}

	public void checkTrigger (Collider col, bool isEnter)
	{
		if (isEnter) {
			if (onTriggerEnter) {
				if (objectToCollide) {
					if (col.gameObject == objectToCollide) {
						if (useEvents) {
							callEvent (onTriggerEnterEvent);

							if (useOnTriggerEnterEventWithObject) {
								callEventWithObject (onTriggerEnterEventWithObject, col.gameObject);
							}
						}

						if (onTriggerEnterFunctionName != "") {
							parent.SendMessage (onTriggerEnterFunctionName, col.gameObject, SendMessageOptions.DontRequireReceiver);
						}

						active = true;
					}
				} else {
					if (useEvents) {
						callEvent (onTriggerEnterEvent);

						if (useOnTriggerEnterEventWithObject) {
							callEventWithObject (onTriggerEnterEventWithObject, col.gameObject);
						}
					} 
			
					if (onTriggerEnterFunctionName != "") {		
						parent.SendMessage (onTriggerEnterFunctionName, col.gameObject, SendMessageOptions.DontRequireReceiver);
					}

					active = true;
				}
			}
		} else {
			if (onTriggerExit) {
				active = true;

				if (useEvents) {
					callEvent (onTriggerExitEvent);

					if (useOnTriggerExitEventWithObject) {
						callEventWithObject (onTriggerExitEventWithObject, col.gameObject);
					}
				} 

				if (onTriggerExitFunctionName != "") {
					parent.SendMessage (onTriggerExitFunctionName, col.gameObject, SendMessageOptions.DontRequireReceiver);
				}
			}
		}
	}

	void OnTriggerStay (Collider col)
	{
		if (!checkCollisionsEnabled) {
			return;
		}

		checkTriggerOnState (col);
	}

	public void checkTriggerOnState (Collider col)
	{
		if (onTriggerStay) {
			if (objectToCollide) {
				if (col.gameObject == objectToCollide) {
					if (useEvents) {
						callEvent (onTriggerStayEvent);

						if (useOnTriggerStayEventWithObject) {
							callEventWithObject (onTriggerStayEventWithObject, col.gameObject);
						}
					} 

					if (onTriggerStayFunctionName != "") {
						parent.SendMessage (onTriggerStayFunctionName, col.gameObject, SendMessageOptions.DontRequireReceiver);
					}

					active = true;
				}
			} else {
				if (useEvents) {
					callEvent (onTriggerStayEvent);

					if (useOnTriggerStayEventWithObject) {
						callEventWithObject (onTriggerStayEventWithObject, col.gameObject);
					}
				} 

				if (onTriggerStayFunctionName != "") {
					parent.SendMessage (onTriggerStayFunctionName, col.gameObject, SendMessageOptions.DontRequireReceiver);
				}

				active = true;
			}
		}
	}

	public void checkTriggerWithGameObject (GameObject obj, bool isEnter)
	{
		if (isEnter) {
			if (onTriggerEnter) {
				if (objectToCollide) {
					if (obj == objectToCollide) {
						if (useEvents) {
							callEvent (onTriggerEnterEvent);

							if (useOnTriggerEnterEventWithObject) {
								callEventWithObject (onTriggerEnterEventWithObject, obj);
							}
						}

						if (onTriggerEnterFunctionName != "") {
							parent.SendMessage (onTriggerEnterFunctionName, obj, SendMessageOptions.DontRequireReceiver);
						}

						active = true;
					}
				} else {
					if (useEvents) {
						callEvent (onTriggerEnterEvent);

						if (useOnTriggerEnterEventWithObject) {
							callEventWithObject (onTriggerEnterEventWithObject, obj);
						}
					}
					if (onTriggerEnterFunctionName != "") {	
						parent.SendMessage (onTriggerEnterFunctionName, obj, SendMessageOptions.DontRequireReceiver);
					}

					active = true;
				}
			}
		} else {
			if (onTriggerExit) {
				active = true;

				if (useEvents) {
					callEvent (onTriggerExitEvent);

					if (useOnTriggerExitEventWithObject) {
						callEventWithObject (onTriggerExitEventWithObject, obj);
					}
				} 

				if (onTriggerExitFunctionName != "") {
					parent.SendMessage (onTriggerExitFunctionName, obj, SendMessageOptions.DontRequireReceiver);
				}
			}
		}
	}

	public void callEvent (UnityEvent eventToCall)
	{
		eventToCall.Invoke ();
	}

	public void callEventWithObject (eventParameters.eventToCallWithGameObject eventToCall, GameObject objectToSend)
	{
		eventToCall.Invoke (objectToSend);
	}

	public void setCheckCollisionsEnabledState (bool state)
	{
		checkCollisionsEnabled = state;
	}
}