using UnityEngine;
using System.Collections;

public class laser : MonoBehaviour
{
	[Header ("Laser Settings")]
	[Space]

	public float scrollSpeed = 0.09f;
	public float pulseSpeed = 0.28f;
	public float noiseSize = 0.19f;
	public float maxWidth = 0.1f;
	public float minWidth = 0.2f;

	public bool animateLaserEnabled = true;

	[Space]
	[Header ("Components")]
	[Space]

	public LineRenderer lRenderer;
	public Renderer mainRenderer;

	[HideInInspector] public float aniDir;
	[HideInInspector] public float laserDistance;

	float aniFactor;

	void Awake ()
	{
		if (lRenderer == null) {
			lRenderer = GetComponent<LineRenderer> ();	
		}

		if (mainRenderer == null) {
			mainRenderer = GetComponent<Renderer> ();
		}
	}

	public void animateLaser ()
	{
		if (animateLaserEnabled) {
			mainRenderer.material.mainTextureOffset += new Vector2 (Time.deltaTime * aniDir * scrollSpeed, 0);

			aniFactor = Mathf.PingPong (Time.time * pulseSpeed, 1);
			aniFactor = Mathf.Max (minWidth, aniFactor) * maxWidth;

			lRenderer.startWidth = aniFactor;
			lRenderer.endWidth = aniFactor;

			mainRenderer.material.mainTextureScale = new Vector2 (0.1f * (laserDistance), mainRenderer.material.mainTextureScale.y);
		}
	}

	public IEnumerator laserAnimation ()
	{
		//just a configuration to animate the laser beam 
		aniDir = aniDir * 0.9f + Random.Range (0.5f, 1.5f) * 0.1f;

		yield return null;

		minWidth = minWidth * 0.8f + Random.Range (0.1f, 1) * 0.2f;
		yield return new WaitForSeconds (1 + Random.value * 2 - 1);	
	}
}