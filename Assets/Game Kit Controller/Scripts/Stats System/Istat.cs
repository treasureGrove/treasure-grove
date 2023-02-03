using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Istat
{
	void eventToInitializeStat (float newValue);

	void eventToIncreaseStat (float newValue);

	void eventToUseStat (float newValue);

	void eventToAddAmount (float newValue);

	void eventToInitializeBoolStat (bool newValue);

	void eventToActivateBoolStat (bool newValue);

	void eventToInitializeStatOnComponent ();

	void eventToInitializeBoolStatOnComponent ();
}
