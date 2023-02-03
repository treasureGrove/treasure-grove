using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class customCameraEffect : cameraEffect
{
	public Material mainMaterial;

	public override void renderEffect (RenderTexture source, RenderTexture destination, Camera mainCamera)
	{
		Graphics.Blit (source, destination, mainMaterial);
	}
}
