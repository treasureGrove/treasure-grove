using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GKC_SimplePoolPreloadSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool preloadObjectsEnabled = true;

	[Space]
	[Header ("Object List Settings")]
	[Space]

	public List<poolObjectPreloadInfo> poolObjectPreloadInfoList = new List<poolObjectPreloadInfo> ();

	void Start ()
	{
		if (preloadObjectsEnabled) {
			preloadObjects ();
		}
	}

	public void preloadObjects ()
	{
		int poolObjectPreloadInfoListCount = poolObjectPreloadInfoList.Count;

		for (int i = 0; i < poolObjectPreloadInfoListCount; i++) {
			GKC_PoolingSystem.Preload (poolObjectPreloadInfoList [i].prefabObject, poolObjectPreloadInfoList [i].objectAmount);
		}
	}

	[System.Serializable]
	public class poolObjectPreloadInfo
	{
		public GameObject prefabObject;
		public int objectAmount;
	}
}
